$(function() {
    $(".role-bar").on("click", function() {
        var section = $(this).next(".role-users");
        section.slideToggle(); // 押すと展開/折りたたみ
    });
});

document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".edit-btn").forEach(btn => {
        btn.addEventListener("click", function () {
            document.querySelectorAll(".edit-form").forEach(f => f.remove());
            // 既存の選択解除
            document.querySelectorAll(".user-block.selected").forEach(el => {
                el.classList.remove("selected");
                let form = el.querySelector(".edit-form");
                if (form) form.remove();
            });

            // このユーザーを選択
            let block = btn.closest(".user-block");
            block.classList.add("selected");

            // 編集フォームを作成
            let userId = btn.dataset.userid;
            let formHtml = `
                <div class="edit-form">
                    <form action="/Account/EditUser" method="post">
                        <input type="hidden" name="UserId" value="${userId}" />
                        <div class="modern-input edit-input">
                            <label style="font-weight: bolder;">ユーザー名</label>
                            <input class="modern-control" type="text" name="Username" value="${block.querySelector('.scor').innerText.replace('ユーザー名: ', '')}" />
                        </div>
                        <div class="modern-input edit-input">
                            <label style="font-weight: bolder;">学年</label>
                            <select class="modern-control" name="Grade"> 
                                <option value="">選択してください</option> 
                                <option value="中学生">中学生</option> 
                                <option value="高校1年生">高校1年生</option> 
                                <option value="高校2年生">高校2年生</option> 
                                <option value="高校3年生">高校3年生</option> 
                                <option value="浪人生">浪人生</option> 
                                <option value="大学生以上">大学生以上</option> 
                            </select>
                        </div>
                        <div class="modern-input edit-input">
                            <label style="font-weight: bolder;">ロール</label>
                            <input class="modern-control" type="text" name="Role" value="${block.querySelectorAll('.scor')[2].innerText.replace('ロール: ', '')}" />
                        </div>
                        <button type="submit" class="send">保存</button>
                    </form>
                </div>
            `;
            block.insertAdjacentHTML("afterend", formHtml);
            let form = block.nextElementSibling;

            const gradeValue = block.querySelectorAll('.scor')[1].innerText.replace('学年: ', ''); 
            form.querySelector('select[name="Grade"]').value = gradeValue;

            requestAnimationFrame(() => {
                form.classList.add("open");
            });
        });
    });
});
