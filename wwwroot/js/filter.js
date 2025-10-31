$(document).ready(function() {
    $("#toggleButton").click(function() {
        $(this).toggleClass("active");
        toggleContainer($("#checkboxContainer1"));
        toggleContainer($("#checkboxContainer2"));
    });
    setupFilter("field1", "field");
    setupFilter("field2", "dif");
});

function toggleContainer($container) {
  const isClosed = $container.height() === 0;

  if (isClosed) {
    $container.css("display", "block");
    $container.animate({ 
      height: $container.get(0).scrollHeight + 5, 
      width: $container.get(0).scrollWidth + 5 
    }, 500);
  } else {
    $container.animate({ 
      height: 0, 
      width: 0 
    }, 500, function() {
      $container.css("display", "none");
    });
  }
}

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
  const selected1 = getSelectedFields("field1");
  const selected2 = getSelectedFields("field2");
  
  const fields1 = selected1.filter(val => val !== "all");
  const fields2 = selected2.filter(val => val !== "all");

  if (fields1.length === 0) {
    if (fields2.length === 0) {
      $(".que").show();
    }
    else {
      $('.que').each(function() {
        const field2 = $(this).data("dif");
        if (selected2.includes(field2)) {
          $(this).show();
        } else {
          $(this).hide();
        }
      });
    }
  }
  else {
    if (fields2.length === 0) {
      $('.que').each(function() {
        const field1 = $(this).data("field");
        if (selected1.includes(field1)) {
          $(this).show();
        } else {
          $(this).hide();
        }
      });
    }
    else {
      $('.que').each(function() {
        const field1 = $(this).data("field");
        const field2 = $(this).data("dif");
        if (selected1.includes(field1) && selected2.includes(field2)) {
          $(this).show();
        } else {
          $(this).hide();
        }
      });
    }
  }
}