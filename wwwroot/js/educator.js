let currentPage = 1;
let isLoading = false;
let autoLoadInterval;

function renderProblems(problems) {
    const container = document.getElementById("problemContainer");
    const role = window.currentUserRole;
    const isLoggedIn = role && role !== "null" && role !== "undefined" && role !== "";

    problems.forEach(problem => {
        // 非公開とみなす条件（undefined や null も含める）
        const isHidden = problem.is_public === false || problem.is_public === undefined || problem.is_public === null;

        // ログインしてない or 生徒の場合、非公開問題はスキップ
        if ((!isLoggedIn || role === "Student" || role === "Educator") && isHidden) {
            return;
        }

        const div = document.createElement("div");
        div.className = "problem-item";

        // --- NEW 判定 ---
        let newBadge = "";
        let isNew = false;
        if (problem.publishedAt) {
            const publishedDate = new Date(problem.publishedAt);
            const now = new Date();
            const diffMs = now - publishedDate;
            const diffDays = diffMs / (1000 * 60 * 60 * 24);
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
            <div class="que under"
                data-field="${problem.category}"
                data-dif="${problem.difficulty}"
                data-new="${isNew}"
                data-hidden="${isHidden}">
                <br>
                ${newBadge}
                <div class="dif">
                    ${problem.idNumber} 難易度 ${problem.difficulty} ${problem.category}
                    ${isHidden ? "<span style='color:red;'>[非公開]</span>" : ""}
                </div>
                <div class="latex">
                    ${problem.latexSrc}
                </div>
                <div class="ans-but">
                    <form method="post" action="/Answer/LookAnswer" style="text-align: right;">
                        <button class="send" name="serial" value="${problem.serialNumber}">解答を見る</button>
                    </form>
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



