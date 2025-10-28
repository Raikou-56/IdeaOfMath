let lastSavedTime = null;

function updateAutoSaveStatus() {
    if (!lastSavedTime) return;

    const now = new Date();
    const secondsAgo = Math.floor((now - lastSavedTime) / 1000);
    const statusText = formatElapsedTime(secondsAgo);

    document.getElementById("autosaveStatus").textContent = statusText;
}

function formatElapsedTime(secondsAgo) {
    if (secondsAgo < 60) {
        return `${secondsAgo}秒前に自動保存済み`;
    } else if (secondsAgo < 3600) {
        const minutes = Math.floor(secondsAgo / 60);
        return `${minutes}分前に自動保存済み`;
    } else {
        const hours = Math.floor(secondsAgo / 3600);
        return `${hours}時間前に自動保存済み`;
    }
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
            lastSavedTime = new Date();
            updateAutoSaveStatus();
            console.log("🌧️ 自動保存しました");
        } else {
            console.warn("⚠️ 自動保存失敗");
        }
    });
}, 180000); // 3分おき

// 保存時間の表示を定期更新（毎秒）
setInterval(updateAutoSaveStatus, 1000);
