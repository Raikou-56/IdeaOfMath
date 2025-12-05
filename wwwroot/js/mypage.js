const editBtn = document.getElementById("editBtn");
const form = document.getElementById("editForm");
const gear = document.querySelector(".gear-icon");

editBtn.addEventListener("click", () => {
    gear.classList.toggle("rotate");
    btnText.textContent = isOpen ? "プロフィールを編集（開く）" : "プロフィールを編集（閉じる）";
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
