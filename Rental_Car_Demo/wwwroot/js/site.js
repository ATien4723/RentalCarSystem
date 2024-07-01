document.getElementById('toggleLoginPassword').addEventListener('click', function () {
    const passwordField = document.getElementById('loginPassword');
    const type = passwordField.getAttribute('type') === 'password' ? 'text' : 'password';
    passwordField.setAttribute('type', type);
    this.classList.toggle('bx-show');
});

document.getElementById('toggleSignupPassword').addEventListener('click', function () {
    const passwordField = document.getElementById('Password');
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


document.addEventListener('DOMContentLoaded', function () {
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
});