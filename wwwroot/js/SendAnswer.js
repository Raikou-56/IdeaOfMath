$(document).ready(function () {
    $('#fileInput').on('change', function (event) {
        const previewArea = $('#previewArea');
        previewArea.empty(); // 前のプレビューを消す

        const files = event.target.files;
        $.each(files, function (i, file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                const img = $('<img>').attr('src', e.target.result)
                                      .css({ 'max-width': '200px', 'margin': '10px' });
                previewArea.append(img);
            };
            reader.readAsDataURL(file);
        });
    });
});
