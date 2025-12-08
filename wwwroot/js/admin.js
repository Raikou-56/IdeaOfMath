$(function() {
    $(".role-bar").on("click", function() {
        var section = $(this).next(".role-users");
        section.slideToggle(); // 押すと展開/折りたたみ
    });
});
