function zoomImage(imgId, factor) {
    var img = document.getElementById(imgId);
    var currentWidth = img.clientWidth;
    var newWidth = currentWidth * factor;

    // 上限・下限を設定
    if (newWidth < 100) newWidth = 100;
    if (newWidth > 800) newWidth = 800;

    img.style.width = newWidth + "px";
}
