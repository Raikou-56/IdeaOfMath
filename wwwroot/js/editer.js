setInterval(() => {
    const data = {
        SerialNumber: document.getElementById("SerialNumber").value,
        IdNumber: document.getElementById("IdNumber").value,
        difficulty: document.getElementById("difficulty").value,
        category: document.getElementById("category").value,
        ProblemLatex: document.getElementById("ProblemLatex").value,
        AnswerLatex: document.getElementById("AnswerLatex").value,
        Teacher: document.getElementById("Teacher").value
    };

    fetch('/YourController/AutoSave', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    }).then(response => {
        if (response.ok) {
            console.log("保存しました🌧️");
        } else {
            console.warn("保存失敗…");
        }
    });
}, 60000); // 180,000ms = 3分
