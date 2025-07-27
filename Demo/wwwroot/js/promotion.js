// wwwroot/js/promotion.js
$(document).ready(function () {
    let selectedId = null;

    $('.select-btn').click(function () {
        $('.promo-card').removeClass('selected');
        $('.select-btn').text('Apply').removeClass('selected-btn');

        const card = $(this).closest('.promo-card');
        card.addClass('selected');
        $(this).text('Selected').addClass('selected-btn');

        selectedId = card.data('id');
        $('#next-btn').prop('disabled', false).addClass('enabled');
    });

    $('#next-btn').click(function () {
        if (selectedId) {
            window.location.href = '/Home/Index?promotionId=' + selectedId;
        }
    });

    // 初始禁用按钮样式
    $('#next-btn').removeClass('enabled').prop('disabled', true);
});
