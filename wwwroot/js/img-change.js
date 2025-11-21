function zoomImage(imgId, scaleFactor) {
    var img = document.getElementById(imgId);
    // 現在の拡大率を取得（なければ1）
    var currentScale = img.dataset.scale ? parseFloat(img.dataset.scale) : 1;
    var newScale = currentScale * scaleFactor;

    // 拡大率の上限・下限を設定（例: 0.5〜3倍）
    if (newScale < 0.5) newScale = 0.5;
    if (newScale > 3) newScale = 3;

    img.style.transform = "scale(" + newScale + ")";
    img.dataset.scale = newScale; // 現在の倍率を保存
}
