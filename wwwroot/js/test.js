let currentPage = 1;

function loadProblems() {
    fetch(`/Problem/GetProblems?page=${currentPage}&limit=5`)
        .then(response => response.json())
        .then(data => {
            data.forEach(problem => {
                const html = `
                    <div class="que under" data-field="${problem.category}" data-dif="${problem.difficulty}">
                        <div class="dif">${problem.idNumber} 難易度 ${problem.difficulty} ${problem.category} ${problem.userData ? `${problem.score}/50` : "未回答"}</div>
                        <div class="latex">${problem.latexSrc}</div>
                        <!-- ボタン類は必要に応じて生成 -->
                    </div>
                `;
                document.getElementById("problemContainer").insertAdjacentHTML("beforeend", html);
            });
            currentPage++;
        });
}

document.getElementById("loadMore").addEventListener("click", loadProblems);
window.addEventListener("DOMContentLoaded", loadProblems);
