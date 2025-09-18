async function loadUnreadCount() {
    try {
        const res = await fetch('/Notification/UnreadCount');
        const data = await res.json();
        const badge = document.getElementById('unread-count');

        if (data.count > 0) {
            badge.textContent = data.count;
            badge.style.display = 'inline-block';
        } else {
            badge.style.display = 'none';
        }
    } catch (err) {
        console.error('加载未读数量失败:', err);
    }
}

// 页面加载时调用
document.addEventListener('DOMContentLoaded', loadUnreadCount);

// 标记通知为已读
document.addEventListener("DOMContentLoaded", () => {
    const token = document.querySelector('#csrf-form input[name="__RequestVerificationToken"]');
    if (!token)
        return;

    const token_value = token.value;
    const badge = document.getElementById("unread-count");

    // 更新铃铛未读数
    async function updateUnreadCount() {
        try {
            const res = await fetch("/Notification/UnreadCount");
            const data = await res.json();
            if (data.count > 0) {
                badge.textContent = data.count;
                badge.style.display = "inline-block";
            } else {
                badge.style.display = "none";
            }
        } catch (err) {
            console.error("未读数量更新失败:", err);
        }
    }

    // 标记为已读
    async function markAsRead(id, element) {
        try {
            const res = await fetch(`/Notification/MarkAsRead/${id}`, {
                method: "POST",
                headers: {
                    "RequestVerificationToken": token_value
                }
            });

            if (res.ok) {
                element.classList.remove("unread");
                element.classList.add("read");
                element.dataset.unread = "false";
                await updateUnreadCount();
            }
        } catch (err) {
            console.error("标记已读失败:", err);
        }
    }

    // 监听元素进入视口
    const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting && entry.target.dataset.unread === "true") {
                const id = entry.target.dataset.id;
                markAsRead(id, entry.target);
            }
        });
    }, { threshold: 0.5 }); // 50% 可见时触发

    // 给所有未读的通知加观察器
    //document.querySelectorAll(".notification-item.unread")
    //    .forEach(item => observer.observe(item));
    document.querySelectorAll(".notification-card.unread")
        .forEach(item => observer.observe(item));
});