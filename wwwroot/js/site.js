// layout で読み込み中...
function setPrintMode(mode) {
  document.body.classList.remove('print-problem', 'print-answer', 'print-all');
  document.body.classList.add('print-' + mode);
  window.print();
}

(() => {
    const btn = document.getElementById("backToMainBtn");
    if (!btn) return;

    let scrollTimer;

    const showButton = () => {
        btn.classList.add("show");
    };

    const hideButton = () => {
        btn.classList.remove("show");
    };

    // 初期状態では非表示
    hideButton();

    window.addEventListener("scroll", () => {
        hideButton();

        clearTimeout(scrollTimer);
        scrollTimer = setTimeout(() => {
            // スクロール位置に関係なく表示
            showButton();
        }, 800);
    });
})();
