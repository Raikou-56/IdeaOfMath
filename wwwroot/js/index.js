let currentPage = 1;
let isLoading = false;
let autoLoadInterval;

function renderProblems(problems) {
    const container = document.getElementById("problemContainer");

    problems.forEach(problem => {
        const role = window.currentUserRole;
        const isLoggedIn = role && role !== "null" && role !== "undefined" && role !== "";

        // 非公開とみなす条件（undefined や null も含める）
        const isHidden = problem.is_public === false || problem.is_public === undefined || problem.is_public === null;

        // ログインしてない or 生徒の場合、非公開問題はスキップ
        if ((!isLoggedIn || role === "Student") && isHidden) {
            return;
        }

        const div = document.createElement("div");
        div.className = "problem-item";

        // 教師・管理者には非公開スタイルを追加
        const hiddenStyle = (problem.Is_public === false && ["Teacher", "Admin"].includes(role))
            ? "opacity: 0.5; border: 1px dashed gray;"
            : "";

        const scoreText = problem.userData ? `${problem.score}/50` : "未回答";
        div.innerHTML = `
            <div class="que under" data-field="${problem.category}" data-dif="${problem.difficulty}" style="${hiddenStyle}">
                <br>
                <div class="dif">
                    ${problem.idNumber} 難易度 ${problem.difficulty} ${problem.category} ${scoreText}
                    ${isHidden ? "<span style='color:red;'>[非公開]</span>" : ""}
                </div>
                <div class="latex">
                    ${problem.latexSrc}
                </div>
                <div class="ans-but">
                    ${getAnswerButtons(problem)}
                </div>
            </div>
        `;

        container.appendChild(div);
    });
    MathJax.typeset();
}

function getAnswerButtons(problem) {
    let buttons = `
        <form method="post" action="/Answer/LookAnswer" style="text-align: right;">
            <button class="send" name="serial" value="${problem.serialNumber}">解答を見る</button>
        </form>
    `;

    if (window.currentUserRole === "Student") {
        if (!problem.userData) {
            buttons += `
                <form method="post" action="/Answer/SendAnswer" style="text-align: right;">
                    <button class="send" name="serial" value="${problem.serialNumber}">解答を送信する</button>
                </form>
            `;
        } else {
            buttons += `
                <form method="post" action="/Answer/CheckAnswer" style="text-align: right;">
                    <input type="hidden" name="studentId" value="${window.currentStudentId}" />
                    <button class="send" name="serial" value="${problem.serialNumber}">解答を確認する</button>
                </form>
            `;
        }
    }

    if (["Teacher", "Admin"].includes(window.currentUserRole)) {
        buttons += `
            <form method="post" action="/Problem/ScoringPage" style="text-align: right;">
                <button class="send" name="serial" value="${problem.serialNumber}">解答を採点する</button>
            </form>
        `;
        if (problem.scoring) {
            buttons += `
                <img src="/img/mathimg3.png" style="width:40px; vertical-align:middle; max-height:15px;" />
            `;
        }
    }

    return buttons;
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

window.addEventListener("scroll", function() {
    const header = document.getElementById("filterHeader");
    const scrollY = window.scrollY;

    if (scrollY > 140) {  
        // layoutヘッダーが消えるタイミングに合わせて
        header.style.top = "0px";
    } else {
        // ページ最上部では少し下に配置
        header.style.top = "140px"; // layoutヘッダーの高さ
    }
});
