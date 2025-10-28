setInterval(() => {
    const data = {
        SerialNumber: document.querySelector('input[name="SerialNumber"]').value,
        IdNumber: document.querySelector('[name="IdNumber"]').value,
        difficulty: document.querySelector('[name="difficulty"]').value,
        category: document.querySelector('[name="category"]').value,
        ProblemLatex: document.getElementById("latexInput1").value,
        AnswerLatex: document.getElementById("latexInput2").value,
        Teacher: document.querySelector('[name="Teacher"]').value
    };

    fetch('/Problem/AutoSave', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    }).then(response => {
        if (response.ok) {
            console.log("🌧️ 自動保存しました");
        } else {
            console.warn("⚠️ 自動保存失敗");
        }
    });
}, 30000); // 3分おき
