let currentPage = 1;
let isLoading = false;
let autoLoadInterval;

function renderProblems(problems) {
    const container = document.getElementById("problemContainer");

    problems.forEach(problem => {
        const div = document.createElement("div");
        div.className = "problem-item";

        const scoreText = problem.userData ? `${problem.score}/50` : "未回答";
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
        })
        .catch(error => {
            console.error("取得失敗:", error.message);
            isLoading = false;
        });
}


window.addEventListener("DOMContentLoaded", () => {
    loadProblems();

    autoLoadInterval = setInterval(() => {
        if (!isLoading) {
            loadProblems();
        }
    }, 200);
});
