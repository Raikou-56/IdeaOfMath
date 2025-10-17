let offset = 0;
const limit = 5;
let loading = false;

function loadProblems() {
  if (loading) return;
  loading = true;
  document.getElementById("loading").style.display = "block";

  fetch(`/Problem/GetProblems?offset=${offset}&limit=${limit}`)
    .then(res => res.json())
    .then(data => {
      if (data.length === 0) {
        document.getElementById("loading").textContent = "これ以上ありません";
        return;
      }

      const container = document.getElementById("problem-container");
      data.forEach(p => {
        const div = document.createElement("div");
        div.className = "que under";
        div.innerHTML = `
          <div class="dif">${p.IdNumber} 難易度 ${p.Difficulty} ${p.Category} ${p.UserData ? `${p.Score}/50` : "未回答"}</div>
          <div class="latex">${p.LatexSrc}</div>
        `;
        container.appendChild(div);
      });

      offset += limit;
      loading = false;
      document.getElementById("loading").style.display = "none";
    });
}

// 最初の読み込み
loadProblems();

// スクロールで追加読み込み
window.addEventListener("scroll", () => {
  if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 100) {
    loadProblems();
  }
});
