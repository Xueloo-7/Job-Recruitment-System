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

//---Job Introduction---
document.getElementById("job-search-form").addEventListener("submit", function (e) {
    e.preventDefault();
    const keyword = document.getElementById("keyword").value;
    const location = document.getElementById("location").value;

    fetch("/Home/SearchJobs", {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded"
        },
        body: `keyword=${encodeURIComponent(keyword)}&location=${encodeURIComponent(location)}`
    })
        .then(res => res.text())
        .then(html => {
            document.getElementById("job-results").innerHTML = html;
        });
});

//---Job Details---
$(document).on('click', '.job-card', function () {
    const jobId = $(this).data('job-id');
        if (jobId) {
            window.location.href = `/Job/Details/${jobId}`;
        }
});