$(function () {
    $('.tab-button').click(function () {
        console.log("sss");
        // 更新按钮状态
        $('.tab-button').removeClass('active');
        $(this).addClass('active');

        // 获取 tab 名称（从 data-tab 属性）
        var tab = $(this).data('tab');

        // 显示 Loading
        $('#tab-content').html('<p>Loading...</p>');

        // AJAX 加载对应 Partial View
        $.ajax({
            url: '/Profile/' + tab,
            method: 'GET',
            success: function (data) {
                $('#tab-content').html(data);
            },
            error: function () {
                $('#tab-content').html('<p style="color:red;">Failed to load content.</p>');
            }
        });
    });

    // 默认加载 Profile 内容（触发默认激活按钮点击）
    $('.tab-button.active').trigger('click');
});

$(document).on('click', '#edit-btn', function () {
    var userId = $(this).data('user-id');
    $.get('/Profile/EditProfilePartial', { userId: userId }, function (html) {
        $('#tab-content').html(html);
    });
});