const editBtn = document.getElementById("editBtn");
const form = document.getElementById("editForm");
const gear = document.querySelector(".gear-icon");
const btnText = document.getElementById("btnText");

editBtn.addEventListener("click", () => {
    const isOpen = form.classList.contains("show");
    gear.classList.toggle("rotate");
    btnText.textContent = isOpen ? "編集メニューを開く" : "編集メニューを閉じる";
    if (form.classList.contains("show")) {
        // 閉じるとき
        form.style.maxHeight = "0";
        form.classList.remove("show");
    } else {
        // 開くとき
        form.style.maxHeight = form.scrollHeight + "px";
        form.classList.add("show");
    }
});
