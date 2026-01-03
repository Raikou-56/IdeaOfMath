// layout で読み込み中...
function setPrintMode(mode) {
  document.body.classList.remove('print-problem', 'print-answer', 'print-all');
  document.body.classList.add('print-' + mode);
  window.print();
}

let scrollTimer2;
const mainBtn = document.getElementById("backToMainBtn");

window.addEventListener("scroll", () => {
    mainBtn.classList.remove("show");

    clearTimeout(scrollTimer2);
    scrollTimer2 = setTimeout(() => {
        if (window.scrollY > 200) {
            mainBtn.classList.add("show");
        }
    }, 800);
});

