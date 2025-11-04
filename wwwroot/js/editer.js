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

document.getElementById("isPublicToggle").addEventListener("change", function () {
    const label = document.getElementById("publicStatusLabel");
    label.textContent = this.checked ? "公開中" : "非公開";
});

// 画像アップロード処理
function extractImgTagsWithAlt(text) {
  const regex = /<img\s[^>]*alt="([^"]+)"[^>]*>/g;
  return [...text.matchAll(regex)]; // matchAllで全タグ取得
}

document.getElementById("uploadImageBtn").addEventListener("click", async () => {
    const textarea = document.getElementById("latexInput2");
    let content = textarea.value;
    const teacherName = document.getElementById("teacherNameInput")?.value?.trim();
    const problemId = document.querySelector("input[name='SerialNumber']")?.value?.trim();
    const imgTags = extractImgTagsWithAlt(content);

    if (imgTags.length === 0) {
        alert("まず <img alt=\"図1\"> のようなタグを解答欄に書いてください！");
        return;
    }

    for (let i = 0; i < imgTags.length; i++) {
        const altText = imgTags[i][1]; // "図1" など
        const originalTag = imgTags[i][0];

        const fileInput = document.createElement("input");
        fileInput.type = "file";
        fileInput.accept = "image/*";

    // ファイル選択を待つ
    let imageUrl = await new Promise((resolve) => {
        fileInput.onchange = async () => {
            const file = fileInput.files[0];
            if (!file) return resolve(null);

            const formData = new FormData();
            formData.append("file", file);
            formData.append("teacherName", teacherName);
            formData.append("problemId", problemId);
            formData.append("fileName", altText); // altからファイル名生成

            if (!teacherName || !problemId || !altText) {
                alert("必要な情報が不足しています。画像タグの alt、先生名、問題IDを確認してください。");
                return resolve(null);
            }

            const response = await fetch("/api/Image/upload", {
                method: "POST",
                body: formData
            });

            const url = await response.text();
            if (!url.startsWith("http")) {
                alert("画像のアップロードに失敗しました。サーバーから正しいURLが返ってきませんでした。");
                return;
            }
            resolve(url);
        };

        fileInput.click();
      });

      if (imageUrl) {
        // タグを src付きに置き換え
        content = content.replace(originalTag, `<img src="${imageUrl}" alt="${altText}">`);
      }
    }

    textarea.value = content;
});
