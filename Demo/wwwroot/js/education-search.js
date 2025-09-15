// 简单的防抖函数
function debounce(fn, delay) {
    let timer;
    return function (...args) {
        clearTimeout(timer);
        timer = setTimeout(() => fn.apply(this, args), delay);
    };
}

function setupSearch(inputId, listId, url) {
    const inputEl = document.getElementById(inputId);
    const listEl = document.getElementById(listId);

    const search = debounce(async () => {
        const term = inputEl.value.trim();
        if (!term) {
            listEl.innerHTML = "";
            listEl.style.display = "none";
            return;
        }
        try {
            const resp = await fetch(`${url}?term=${encodeURIComponent(term)}`);
            const data = await resp.json();
            if (data.length === 0) {
                listEl.innerHTML = "";
                listEl.style.display = "none";
                return;
            }
            listEl.innerHTML = data
                .map(item => `<div class="dropdown-item" data-value="${item.name}">${item.name}</div>`)
                .join("");
            listEl.style.display = "block";
        } catch (e) {
            console.error(e);
        }
    }, 200); // 0.2秒防抖

    inputEl.addEventListener("input", search);

    // 点击候选项填入
    listEl.addEventListener("click", e => {
        if (e.target.classList.contains("dropdown-item")) {
            inputEl.value = e.target.dataset.value;
            listEl.style.display = "none";
        }
    });

    // 点击外部区域隐藏下拉
    document.addEventListener("click", e => {
        if (!listEl.contains(e.target) && e.target !== inputEl) {
            listEl.style.display = "none";
        }
    });
}

// 初始化
document.addEventListener("DOMContentLoaded", () => {
    setupSearch("qualificationInput", "qualificationList", "/Profile/SearchQualification");
    setupSearch("institutionInput", "institutionList", "/Profile/SearchInstitution");
});
