$(function () {
    const input1 = document.getElementById('latexInput1');
    const preview1 = document.getElementById('mathPreview1');
    const previewBtn1 = document.getElementById('previewBtn1');

    previewBtn1.addEventListener('click', () => {
        preview1.innerHTML = input1.value;
        MathJax.typesetPromise([preview1]);
    });

    const input2 = document.getElementById('latexInput2');
    const preview2 = document.getElementById('mathPreview2');
    const previewBtn2 = document.getElementById('previewBtn2');

    previewBtn2.addEventListener('click', () => {
        preview2.innerHTML = input2.value;
        MathJax.typesetPromise([preview2]);
    });
});
