//---Job Details---
$(document).on('click', '.job-card', function () {
    const jobId = $(this).data('job-id');
    if (jobId) {
        window.location.href = `/Job/Details/${jobId}`;
    }
});

ready(() => {
    const jobId = 
})

