document.addEventListener("DOMContentLoaded", function () {
    const toggleBtn = document.getElementById("jobInfoToggle");
    const jobInfo = document.getElementById("jobInfo");

    if (!toggleBtn || !jobInfo) {
        console.error("元素没找到");
        return;
    }

    toggleBtn.addEventListener("click", function () {
        console.log("按钮被点击了"); // 调试用
        jobInfo.classList.toggle("show");
    });
});