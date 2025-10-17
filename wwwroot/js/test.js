let currentPage = 1;

function renderProblems(problems) {
    const container = document.getElementById("problemContainer");
    container.innerHTML = "<p>表示成功</p>"; // 一旦クリア（必要なら）

    problems.forEach(problem => {
        const div = document.createElement("div");
        div.className = "problem-item";
        div.innerHTML = `
            <h4>${problem.serialNumber} - ${problem.idNumber}</h4>
            <p>カテゴリ: ${problem.category}</p>
            <p>難易度: ${problem.difficulty}</p>
            <p>式: ${problem.latexSrc}</p>
        `;
        container.appendChild(div);
    });
}

fetch('/Development/GetProblems?page=1&limit=5')
    .then(response => {
        if (!response.ok) {
            return response.text().then(text => { throw new Error(text); });
        }
        return response.json();
    })
    .then(data => {
        console.log("取得成功:", data);
        renderProblems(data);
    })
    .catch(error => {
        console.error("取得失敗:", error.message);
    });


document.getElementById("loadMore").addEventListener("click", loadProblems);
window.addEventListener("DOMContentLoaded", loadProblems);
