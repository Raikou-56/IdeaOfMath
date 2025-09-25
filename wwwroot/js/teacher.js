$(document).ready(function () {
    // mathjax即時プレビュー
    const input1 = document.getElementById('latexInput1');
    const preview1 = document.getElementById('mathPreview1');

    input1.addEventListener('input', () => {
        // 入力されたLaTeXをMathJaxで表示
        preview1.innerHTML = `${input1.value}`;
        MathJax.typesetPromise([preview1]);
    });

    const input2 = document.getElementById('latexInput2');
    const preview2 = document.getElementById('mathPreview2');

    input2.addEventListener('input', () => {
        // 入力されたLaTeXをMathJaxで表示
        preview2.innerHTML = `${input2.value}`;
        MathJax.typesetPromise([preview2]);
    });
});