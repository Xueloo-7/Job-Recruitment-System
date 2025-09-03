document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('employer-form');

    form.addEventListener('submit', function (e) {
        e.preventDefault();

        const email = document.getElementById('email').value;

        if (!validateEmail(email)) {
            alert("Please enter a valid business email.");
            return;
        }

        alert("Registered successfully!");
        // TODO: Submit to backend or show success UI
    });

    function validateEmail(email) {
        // Basic email pattern
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email.toLowerCase());
    }
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