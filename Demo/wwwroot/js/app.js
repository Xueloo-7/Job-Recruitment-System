// ======================
// DOMContentLoaded
// ======================
const ready = fn => document.addEventListener('DOMContentLoaded', fn);

// start when DOM is ready
ready(() => {
    //// Initiate GET request (AJAX-supported)
    $(document).on('click', '[data-get]', e => {
        e.preventDefault();
        const url = e.target.dataset.get;
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