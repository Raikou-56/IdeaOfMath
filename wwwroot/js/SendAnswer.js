$(document).ready(function () {
    $('#fileInput').on('change', function (event) {
        const previewArea = $('#previewArea');
        const submitBtn = $('#submitBtn');
        previewArea.empty(); // 前のプレビューを消す

        const files = event.target.files;

        if (files.length > 0) {
            submitBtn.prop('disabled', false); // 有効化
            $.each(files, function (i, file) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    const img = $('<img>')
                        .attr('src', e.target.result)
                        .addClass('answer-image'); // 既存のスタイルを活用
                    previewArea.append(img);
                };
                reader.readAsDataURL(file);
            });
        } else {
            submitBtn.prop('disabled', true); // 無効化
        }
    });
});
