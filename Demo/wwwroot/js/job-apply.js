document.addEventListener("DOMContentLoaded", function () {
    const toggleBtn = document.getElementById("jobInfoToggle");
    const jobInfo = document.getElementById("jobInfo");

    if (!toggleBtn || !jobInfo) {
        console.error("元素没找到");
        return;
    }

    toggleBtn.addEventListener("click", function () {
        console.log("按钮被点击了"); // 调试用

        if (jobInfo.style.maxHeight && jobInfo.style.maxHeight !== "0px") {
            // 已展开 -> 收起
            jobInfo.style.maxHeight = "0px";
            jobInfo.style.paddingTop = "0px";
            jobInfo.style.paddingBottom = "0px";
            toggleBtn.classList.remove("show"); // 图标旋转复原
        } else {
            // 收起 -> 展开
            jobInfo.style.maxHeight = jobInfo.scrollHeight + "px";
            jobInfo.style.paddingTop = "15px";
            jobInfo.style.paddingBottom = "15px";
            toggleBtn.classList.add("show"); // 图标旋转
        }
    });
});
