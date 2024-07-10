
<script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.11.6/dist/umd/popper.min.js"></script>



document.addEventListener('DOMContentLoaded', function () {
    // Toggle password visibility
    document.getElementById('toggleLoginPassword').addEventListener('click', function () {
        const passwordField = document.getElementById('loginPassword');
        const type = passwordField.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordField.setAttribute('type', type);
        this.classList.toggle('bx-show');
    });

    document.getElementById('toggleSignupPassword').addEventListener('click', function () {
        const passwordField = document.getElementById('signupPassword');
        const type = passwordField.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordField.setAttribute('type', type);
        this.classList.toggle('bx-show');
    });

    document.getElementById('toggleSignupConfirmPassword').addEventListener('click', function () {
        const passwordField = document.getElementById('signupConfirmPassword');
        const type = passwordField.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordField.setAttribute('type', type);
        this.classList.toggle('bx-show');
    });

    // Only one radio button should be checked at a time
    let lastChecked = null;

    const rentCar = document.getElementById('rentCar');
    const carOwner = document.getElementById('carOwner');

    rentCar.addEventListener('click', function () {
        if (lastChecked === rentCar) {
            rentCar.checked = false;
            lastChecked = null;
        } else {
            if (lastChecked) lastChecked.checked = false;
            lastChecked = rentCar;
            rentCar.checked = true;
        }
    });

    carOwner.addEventListener('click', function () {
        if (lastChecked === carOwner) {
            carOwner.checked = false;
            lastChecked = null;
        } else {
            if (lastChecked) lastChecked.checked = false;
            lastChecked = carOwner;
            carOwner.checked = true;
        }
    });

    // Form validation
    document.querySelector('form').addEventListener('submit', function (event) {
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
});

document.addEventListener("DOMContentLoaded", function () {

    let currentDate = new Date();


    let formattedDate = currentDate.toISOString().slice(0, 10);
    let formattedTime = currentDate.toTimeString().slice(0, 5);


    document.getElementById("myDateInput").value = formattedDate;
    document.getElementById("myTimeInput").value = formattedTime;
    document.getElementById("dropDateInput").value = formattedDate;
    document.getElementById("dropTimeInput").value = formattedTime;
});

