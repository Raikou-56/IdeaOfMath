let currentPage = 1;

function loadProblems() {
    fetch('/Development/GetProblems?page=1&limit=5')
    .then(response => {
        if (!response.ok) {
            return response.text().then(text => { throw new Error(text); });
        }
        return response.json();
    })
    .then(data => {
        console.log("取得成功:", data);
    })
    .catch(error => {
        console.error("取得失敗:", error.message);
    });

}

document.getElementById("loadMore").addEventListener("click", loadProblems);
window.addEventListener("DOMContentLoaded", loadProblems);
