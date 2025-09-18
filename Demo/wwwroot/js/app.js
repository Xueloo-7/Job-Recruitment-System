// ======================
// DOMContentLoaded
// ======================
const ready = fn => document.addEventListener('DOMContentLoaded', fn);

// start when DOM is ready
ready(() => {
    //// Initiate GET request (AJAX-supported)
    $(document).on('click', '[data-get]', e => {
        e.preventDefault();
        const url = e.currentTarget.dataset.get;
        location = url || location;
    });

    // ======================
    // Side Menu Toggle
    // ======================
    $(document).on('click', '.menu-toggle', function () {
        $('.sideMenu').addClass('open');
        $('.overlay').addClass('active');
    });

    $(document).on('click', '.closeMenu, .overlay', function () {
        $('.sideMenu').removeClass('open');
        $('.overlay').removeClass('active');
    });
});

// Photo preview
$('.upload input').on('change', e => {
    const f = e.target.files[0];
    const img = $(e.target).siblings('img')[0];

    img.dataset.src ??= img.src;

    if (f && f.type.startsWith('image/')) {
        img.onload = e => URL.revokeObjectURL(img.src);
        img.src = URL.createObjectURL(f);
    }
    else {
        img.src = img.dataset.src;
        e.target.value = '';
    }

    // Trigger input validation
    $(e.target).valid();
});

// Auto Title 
$('#nameInput').on('blur', function () {
    $(this).val(toTitleCase($(this).val()));
});

// Trim input
$('[data-trim]').on('change', e => {
    e.target.value = e.target.value.trim();
});

// Initiate GET request (AJAX-supported)
$(document).on('click', '[data-get]', e => {
    e.preventDefault();
    const url = e.target.dataset.get;
    location = url || location;
});

// Initiate POST request (AJAX-supported)
$(document).on('click', '[data-post]', e => {
    e.preventDefault();
    const url = e.target.dataset.post;
    const f = $('<form>').appendTo(document.body)[0];
    f.method = 'post';
    f.action = url || location;
    f.submit();
});

document.addEventListener("DOMContentLoaded", function () {
    const toggleButton = document.getElementById("jobInfoToggle");
    const jobInfo = document.getElementById("jobInfo");

    toggleButton.addEventListener("click", function () {
        jobInfo.classList.toggle("show");
        toggleButton.classList.toggle("active");
    });
});