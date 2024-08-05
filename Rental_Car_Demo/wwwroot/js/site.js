
$(document).ready(function () {
    if ('@showModal' === 'SignIn' || '@showModal' === 'Register') {
        $('#authModal').modal('show');
    }
});


//
document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm');

    if (loginForm) {
        loginForm.addEventListener('submit', function (event) {
            let valid = true;

            const email = document.getElementById('loginEmail');
            const password = document.getElementById('loginPassword');

            const emailError = document.getElementById('loginEmailError');
            const passwordError = document.getElementById('loginPasswordError');
            const serverError = document.getElementById('serverError');

            emailError.textContent = '';
            passwordError.textContent = '';

            if (email.value.trim() === '') {
                emailError.textContent = 'This field is required';
                valid = false;
            } else {
                const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailPattern.test(email.value.trim())) {
                    emailError.textContent = 'Please enter a valid email address';
                    valid = false;
                }
            }

            if (password.value.trim() === '') {
                passwordError.textContent = 'This field is required';
                valid = false;
            }

            if (!valid) {
                event.preventDefault(); // Prevent form submission
                if (serverError) {
                    serverError.textContent = ''; // Clear server error message if any
                }
            }
        });
    }
});






