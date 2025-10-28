let lastSavedTime = null;

function updateAutoSaveStatus() {
    if (!lastSavedTime) return;

    const now = new Date();
    const secondsAgo = Math.floor((now - lastSavedTime) / 1000);
    const statusText = `${secondsAgo}秒前に自動保存済み`;

    document.getElementById("autosaveStatus").textContent = statusText;
}

// 自動保存処理（30秒おき）
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

// 保存時間の表示を定期更新（毎秒）
setInterval(updateAutoSaveStatus, 1000);
