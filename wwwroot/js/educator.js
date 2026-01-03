let currentPage = 1;
let isLoading = false;
let autoLoadInterval;

function renderProblems(problems) {
    const container = document.getElementById("problemContainer");
    const role = window.currentUserRole;
    const isLoggedIn = role && role !== "null" && role !== "undefined" && role !== "";

    problems.forEach(problem => {
        const isHidden = problem.isPublic === false || problem.isPublic === undefined || problem.isPublic === null;

        if ((!isLoggedIn || role === "Student" || role === "Educator") && isHidden) {
            return;
        }

        const div = document.createElement("div");
        div.className = "problem-item";

        // NEW 判定
        let newBadge = "";
        let isNew = false;
        if (problem.publishedAt) {
            const publishedDate = new Date(problem.publishedAt);
            const now = new Date();
            const diffDays = (now - publishedDate) / (1000 * 60 * 60 * 24);
            isNew = diffDays <= 3;
            if (isNew) {
                newBadge = `<span style="font-size:0.7em; color:orange; font-weight:bold; margin-right:4px;">NEW</span>`;
            }
        }

        div.innerHTML = `
            <label class="print-select">
                <input type="checkbox" class="print-checkbox" data-problem-id="${problem.serialNumber}" checked>
                印刷
            </label>

            <!-- 問題ブロック -->
            <div class="question que under print-enabled"
                data-problem-id="${problem.serialNumber}"
                data-field="${problem.category}"
                data-dif="${problem.difficulty}"
                data-new="${isNew}"
                data-hidden="${isHidden}">
                
                ${newBadge}
                <div class="dif">
                    ${problem.idNumber} 難易度 ${problem.difficulty} ${problem.category}
                    ${isHidden ? "<span style='color:red;'>[非公開]</span>" : ""}
                </div>

                <div class="latex problem">
                    ${problem.problemLatex ?? ""}
                </div>
                <div class="latex displaynone-latex answer">
                    ${problem.answerLatex ?? ""}
                </div>
            </div>
        `;

        container.appendChild(div);
    });

    MathJax.typeset();
}


function loadProblems() {
    isLoading = true;
    const studentId = (window.currentStudentId && window.currentStudentId !== "null" && window.currentStudentId !== "undefined")
    ? window.currentStudentId
    : "";
    fetch(`/Home/GetProblemsDetail?page=${currentPage}&limit=3&studentId=${studentId}`)
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => { throw new Error(text); });
            }
            return response.json();
        })
        .then(data => {
            if (data.length === 0) {
                clearInterval(autoLoadInterval);
                return;
            }

            renderProblems(data);
            currentPage++;
            isLoading = false;

            // 自動読み込みを即座に試みる
            setTimeout(() => {
                loadProblems();
            }, 0);
        })
        .catch(error => {
            console.error("取得失敗:", error.message);
            isLoading = false;
        });
}

window.addEventListener("DOMContentLoaded", () => {
    loadProblems(); // 最初の読み込みだけ
});

$(function() {
    var nav = $('#filterHeader');
    var navTop = nav.offset().top; // ヘッダーの初期位置を取得

    $(window).scroll(function () {
        var winTop = $(this).scrollTop();
        if (winTop >= navTop) {
            nav.removeClass('absolute').addClass('fixed');
        } else {
            nav.removeClass('fixed').addClass('absolute');
        }
    });
});


document.addEventListener("change", (e) => {
    if (!e.target.classList.contains("print-checkbox")) return;

    const id = e.target.dataset.problemId;
    const problemDiv = document.querySelector(`.que[data-problem-id="${id}"]`);

    if (e.target.checked) {
        problemDiv.classList.add("print-enabled");
    } else {
        problemDiv.classList.remove("print-enabled");
    }
});

document.addEventListener("DOMContentLoaded", () => {
    loadProblems();

    // タブクリックで切り替え
    document.querySelectorAll("#printTabs button").forEach(btn => {
        btn.addEventListener("click", () => {
            const mode = btn.dataset.mode;

            // タブの active 切り替え
            document.querySelectorAll("#printTabs button").forEach(b => {
                b.classList.toggle("active", b === btn);
            });

            // 問題/解答の表示切り替え
            toggleLatexMode(mode);
        });
    });
});

function toggleLatexMode(mode) {
    showLoading(); // まず表示

    // ここで一度イベントループに返す
    setTimeout(() => {

        const items = document.querySelectorAll("#problemContainer .problem-item");

        items.forEach(item => {
            const latexBlocks = item.querySelectorAll(".latex");

            const problemLatex = latexBlocks[0];
            const answerLatex  = latexBlocks[1];

            if (mode === "problem") {
                problemLatex.classList.remove("displaynone-latex");
                answerLatex.classList.add("displaynone-latex");
            } else {
                problemLatex.classList.add("displaynone-latex");
                answerLatex.classList.remove("displaynone-latex");
            }
        });

        MathJax.typesetPromise()
            .then(() => hideLoading())
            .catch(err => {
                console.error(err);
                hideLoading();
            });

    }, 0);
}



function showLoading() {
    document.getElementById("loadingOverlay").style.display = "flex";
}

function hideLoading() {
    document.getElementById("loadingOverlay").style.display = "none";
}



