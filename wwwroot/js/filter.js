$(document).ready(function() {
  // 吊るし看板クリックでパネルをスライド表示
  $("#signboard").click(function() {
    console.log("吊るし看板がクリックされました");
    $("#filterPanel").slideToggle(300);
  });

  // フィルター処理は既存のまま
  setupFilter("field1");
  setupFilter("field2");
});


function setupFilter(name) {
  $(`input[name="${name}"]`).on('change', function() {
    if ($(this).val() !== "all") {
      console.log("all以外が押されました")
      $(`input[name="${name}"][value="all"]`).prop("checked", false);
    }
    else {
      $(`input[name="${name}"]`).each(function() {
        $(this).prop('checked', $(this).val() === 'all');
      });
    }
    filterProblems();
  });
}

function getSelectedFields(name) {
  return $(`input[name="${name}"]:checked`).map(function() {
    return $(this).val();
  }).get();
}

function filterProblems() {
  const selected1 = getSelectedFields("field1").filter(val => val !== "all");
  const selected2 = getSelectedFields("field2").filter(val => val !== "all");

  $('.que').each(function() {
    const field1 = $(this).data("field");
    const field2 = $(this).data("dif");

    const matchField1 = selected1.length === 0 || selected1.includes(field1);
    const matchField2 = selected2.length === 0 || selected2.includes(field2);

    if (matchField1 && matchField2) {
      $(this).show();
    } else {
      $(this).hide();
    }
  });
}
