let currentPage = 1;

function renderProblems(problems) {
    const container = document.getElementById("problemContainer");
    container.innerHTML = ""; // 必要ならクリア

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
                    <form method="post" action="/Answer/LookAnswer" style="text-align: right;">
                        <button class="send" name="serial" value="${problem.serialNumber}">解答を見る</button>
                    </form>
                    ${problem.userData === false ? `
                        <form method="post" action="/Answer/SendAnswer" style="text-align: right;">
                            <button class="send" name="serial" value="${problem.serialNumber}">解答を送信する</button>
                        </form>
                    ` : `
                        <form method="post" action="/Answer/CheckAnswer" style="text-align: right;">
                            <input type="hidden" name="studentId" value="（ここはJSでは取得できない）" />
                            <button class="send" name="serial" value="${problem.serialNumber}">解答を確認する</button>
                        </form>
                    `}
                    ${scoringIcon}
                </div>
            </div>
        `;

        container.appendChild(div);
    });
}


function loadProblems() {
    fetch(`/Development/GetProblems?page=${currentPage}&limit=5`)
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => { throw new Error(text); });
            }
            return response.json();
        })
        .then(data => {
            console.log("取得成功:", data);
            renderProblems(data);
            currentPage++; // 次のページへ進める
        })
        .catch(error => {
            console.error("取得失敗:", error.message);
        });
}


document.getElementById("loadMore").addEventListener("click", loadProblems);
window.addEventListener("DOMContentLoaded", loadProblems);
