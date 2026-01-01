let currentPage = 1;
let isLoading = false;
let autoLoadInterval;

function renderProblems(problems) {
    const container = document.getElementById("problemContainer");
    const role = window.currentUserRole;
    const isLoggedIn = role && role !== "null" && role !== "undefined" && role !== "";

    problems.forEach(problem => {
        const isHidden = problem.is_public === false || problem.is_public === undefined || problem.is_public === null;

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

                <div class="latex">
                    ${problem.problemLatex ?? ""}
                </div>
            </div>

            <!-- 解答ブロック -->
            <div class="answer print-enabled"
                data-problem-id="${problem.serialNumber}">
                <div class="latex">
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
    fetch(`/Home/GetProblems?page=${currentPage}&limit=3&studentId=${studentId}`)
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
    const tabs = document.querySelectorAll("#printTabs button");

    tabs.forEach(tab => {
        tab.addEventListener("click", () => {
            const mode = tab.dataset.mode;

            // タブの見た目切り替え
            tabs.forEach(t => t.classList.remove("active"));
            tab.classList.add("active");

            // body クラス切り替え
            if (mode === "problem") {
                document.body.classList.add("print-problem");
                document.body.classList.remove("print-answer");
            } else {
                document.body.classList.remove("print-problem");
                document.body.classList.add("print-answer");
            }
        });
    });

    // 初期状態は問題タブ
    document.body.classList.add("print-problem");
});



