$(function () {
    // Tab 按钮点击事件，只处理 UI 样式
    $('.tab-button').click(function () {
        // 移除所有按钮的 active
        $('.tab-button').removeClass('active');
        // 当前按钮加 active
        $(this).addClass('active');
    });
});


$(document).on('click', '#edit-btn', function () {
    var userId = $(this).data('user-id');
    $.get('/Profile/EditProfilePartial', { userId: userId }, function (html) {
        $('#tab-content').html(html);
    });
});