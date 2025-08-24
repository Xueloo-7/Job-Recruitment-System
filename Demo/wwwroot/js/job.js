document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.job-card').forEach(card => {
        card.addEventListener('click', () => {
            // 1. 移除所有卡片的 active 类
            document.querySelectorAll('.job-card').forEach(c => c.classList.remove('active'));

            // 2. 当前卡片添加 active 类
            card.classList.add('active');

            // 3. 加载详情页面内容
            const jobId = card.getAttribute('data-id');
            fetch(`/Job/Details/${jobId}`)
                .then(res => res.text())
                .then(html => {
                    const detailBox = document.getElementById('job-details');
                    detailBox.innerHTML = html;

                    // 4. 滚动到顶部
                    detailBox.scrollTop = 0;

                    // 处理响应式逻辑
                    const leftCardContainer = document.querySelector('.job-left');
                    leftCardContainer.classList.add('hidden')
                    detailBox.classList.add('active');


                    // 绑定返回按钮事件
                    bindBackButton();
                });
        });
    });

    function bindBackButton() {
        // 响应式逻辑
        const backBtn = document.querySelector('.back-btn');
        const leftCardContainer = document.querySelector('.job-left');
        const rightCardContainer = document.querySelector('.job-right');

        console.log(backBtn)
        console.log(leftCardContainer)
        console.log(rightCardContainer)

        if (backBtn && leftCardContainer && rightCardContainer) {
            backBtn.addEventListener('click', () => {
                leftCardContainer.classList.remove('hidden');
                rightCardContainer.classList.remove('active');
            });
        }
    }


});





