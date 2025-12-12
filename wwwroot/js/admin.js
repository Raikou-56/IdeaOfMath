$(function() {
    $(".role-bar").on("click", function() {
        var section = $(this).next(".role-users");
        section.slideToggle(); // 押すと展開/折りたたみ
    });
});

document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".edit-btn").forEach(btn => {
        btn.addEventListener("click", function () {
            // 既存の選択解除
            document.querySelectorAll(".user-block.selected").forEach(el => {
                el.classList.remove("selected");
                let form = el.querySelector(".edit-form");
                if (form) form.remove();
            });

            // このユーザーを選択
            let block = btn.closest(".user-block");
            block.classList.add("selected");

            let infoDiv = block.querySelector("div");

            // 編集フォームを作成
            let userId = btn.dataset.userid;
            let formHtml = `
                <div class="edit-form">
                    <form action="/Account/EditUser" method="post">
                        <input type="hidden" name="UserId" value="${userId}" />
                        <div>
                            <label>ユーザー名</label>
                            <input type="text" name="Username" value="${block.querySelector('.scor').innerText.replace('ユーザー名: ', '')}" />
                        </div>
                        <div>
                            <label>学年</label>
                            <input type="text" name="Grade" value="${block.querySelectorAll('.scor')[1].innerText.replace('学年: ', '')}" />
                        </div>
                        <div>
                            <label>ロール</label>
                            <input type="text" name="Role" value="${block.querySelectorAll('.scor')[2].innerText.replace('ロール: ', '')}" />
                        </div>
                        <button type="submit" class="send">保存</button>
                    </form>
                </div>
            `;
            infoDiv.insertAdjacentHTML("afterend", formHtml);
        });
    });
});
