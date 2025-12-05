const editBtn = document.getElementById("editBtn");
const form = document.getElementById("editForm");
const gear = document.querySelector(".gear-icon");

editBtn.addEventListener("click", () => {
    gear.classList.add("rotate");
    setTimeout(() => gear.classList.remove("rotate"), 600);
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
