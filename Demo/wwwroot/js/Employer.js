document.addEventListener('DOMContentLoaded', function () {

    // Withdraw buttons
    document.querySelectorAll('.withdraw-btn').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault(); // 阻止默认跳转
            showConfirm("Are you sure you want to withdraw this job posting?", function () {
                const jobId = btn.dataset.id;
                window.location.href = `/Job/Withdraw/${jobId}`;
            });
        });
    });

    // Delete Draft buttons
    document.querySelectorAll('.delete-draft-btn').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            showConfirm("Are you sure you want to delete this draft? (This operation cannot be undone)", function () {
                const draftId = btn.dataset.id;
                window.location.href = `/Job/DeleteDraft/${draftId}`;
            });
        });
    });

});

//EditEmployer
$(function () {
    // 点击 Edit 按钮加载表单
    $(".edit-btn").on("click", function () {
        var userId = $(this).data("id");

        $.get("/Home/EditEmployer", { id: userId }, function (html) {
            $("#employer-edit-container").html(html);

            // ⚡ 动态绑定提交事件
            $("#editEmployerForm").on("submit", function (e) {
                e.preventDefault(); // 阻止整页跳转

                $.post($(this).attr("action"), $(this).serialize(), function (html) {
                    // 用返回的 EmployerInfo 页面替换整个 employer-info 区域
                    $(".employer-info").html($(html).find(".employer-info").html());
                }).fail(function () {
                    alert("Failed to save employer.");
                });
            });
        }).fail(function () {
            alert("Failed to load employer edit form.");
        });
    });
});