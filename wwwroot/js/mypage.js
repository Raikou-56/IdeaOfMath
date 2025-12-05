const editBtn = document.getElementById("editBtn");
const form = document.getElementById("editForm");

editBtn.addEventListener("click", () => {
    form.classList.toggle("show");
    form.classList.toggle("hidden");
});