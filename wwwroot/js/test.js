let currentPage = 1;
let isLoading = false;

function renderProblems(problems) {
    const container = document.getElementById("problemContainer");

    problems.forEach(problem => {
        const div = document.createElement("div");
        div.className = "problem-item";

        // スコア表示
        const scoreText = problem.userData ? `${problem.score}/50` : "未回答";

        // 採点済みアイコン
        const scoringIcon = problem.scoring
            ? `<img src="/img/mathimg3.png" style="width:40px; vertical-align:middle; max-height:15px;" />`
            : "";

        div.innerHTML = `
            <div class="que under" data-field="${problem.category}" data-dif="${problem.difficulty}">
                <br>
                <div class="dif">
                    ${problem.idNumber} 難易度 ${problem.difficulty} ${problem.category} ${scoreText}
                </div>
                <div class="latex">
                    ${problem.latexSrc}
                </div>
                <div class="ans-but">
                    ${getAnswerButtons(problem)}
                    ${scoringIcon}
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

    if (window.currentUserRole === "Teacher" || window.currentUserRole === "Admin") {
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

    fetch(`/Development/GetProblems?page=${currentPage}&limit=5`)
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => { throw new Error(text); });
            }
            return response.json();
        })
        .then(data => {
            if (data.length === 0) {
                // もう読み込むものがない場合はイベント解除してもOK
                window.removeEventListener("scroll", scrollHandler);
                return;
            }

            renderProblems(data);
            currentPage++;
            isLoading = false;
        })
        .catch(error => {
            console.error("取得失敗:", error.message);
            isLoading = false;
        });
}

function scrollHandler() {
    const scrollPosition = window.scrollY + window.innerHeight;
    const threshold = document.body.offsetHeight - 300;

    if (scrollPosition > threshold && !isLoading) {
        loadProblems();
    }
}

window.addEventListener("scroll", scrollHandler);

document.getElementById("loadMore").addEventListener("click", loadProblems);
window.addEventListener("DOMContentLoaded", loadProblems);
