// layout で読み込み中...
function setPrintMode(mode) {
  document.body.classList.remove('print-problem', 'print-all');
  document.body.classList.add('print-' + mode);
  window.print();
}
