//// Initiate GET request (AJAX-supported)
//$(document).on('click', '[data-get]', e => {
//    e.preventDefault();
//    const url = e.target.dataset.get;
//    location = url || location;
//});


//---Side Menu---
$(document).on('click', '.menu-toggle', function () {
    $('.sideMenu').addClass('open');
    $('.overlay').addClass('active');
});

$(document).on('click', '.closeMenu, .overlay', function () {
    $('.sideMenu').removeClass('open');
    $('.overlay').removeClass('active');
});

//---Search Function---
$(function () {
    $('#job-search-form').on('submit', function (e) {
        e.preventDefault();

        $.get('@Url.Action("SearchAjax", "Job")', $(this).serialize(), function (data) {
            $('#job-results').html(data);
        });
    });
});