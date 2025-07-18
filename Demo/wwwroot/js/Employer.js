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
