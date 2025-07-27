//document.addEventListener('DOMContentLoaded', function () {
//    const buttons = document.querySelectorAll('.tab-button');
   

//    buttons.forEach(button => {
//        button.addEventListener('click', () => {
//            // change btn success state
//            buttons.forEach(btn => btn.classList.remove('active'));
//            button.classList.add('active');

//            // change content
//            const selectedTab = button.textContent.trim();
//            content.innerHTML = tabData[selectedTab];
//        });
//    });
//});

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
            url: '/User/' + tab,
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

