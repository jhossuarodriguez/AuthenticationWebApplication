document.addEventListener('DOMContentLoaded', function () {

    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            const data = {
                username: loginForm.Username.value,
                password: loginForm.Password.value,
                rememberMe: loginForm.RememberMe ? loginForm.RememberMe.checked : false
            };
            const response = await fetch('/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            if (response.ok) {
                const result = await response.json();
                localStorage.setItem('jwt', result.token);
                window.location.href = '/';
            } else {
                const error = await response.text();
                alert('Error: ' + error);
            }
        });
    }

    // LOGOUT
    const logoutBtn = document.getElementById('logoutBtn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', function (e) {
            e.preventDefault();
            const jwt = localStorage.getItem('jwt');
            fetch('/logout', {
                method: 'POST',
                headers: {
                    'Authorization': 'Bearer ' + jwt
                }
            }).then(() => {
                localStorage.removeItem('jwt');
                window.location.href = '/';
            });
        });
    }

    const userNameSpan = document.getElementById('userName');
    const jwt = localStorage.getItem('jwt');
    if (userNameSpan && jwt) {
        fetch('/profile', {
            headers: {
                'Authorization': 'Bearer ' + jwt
            }
        })
        .then(res => {
            if (!res.ok) throw new Error('No autorizado');
            return res.json();
        })
        .then(profile => {
            userNameSpan.textContent = `Hola, ${profile.firstName} ${profile.lastName}`;
        })
        .catch(() => {
            userNameSpan.textContent = '';
        });
    }
});

document.addEventListener('DOMContentLoaded', function () {

    const jwt = localStorage.getItem('jwt');
    const loginLink = document.getElementById('loginLink');
    const logoutForm = document.getElementById('logoutForm');

    if (jwt) {
        if (loginLink) loginLink.style.display = 'none';
        if (logoutForm) logoutForm.style.display = '';
    } else {
        if (loginLink) loginLink.style.display = '';
        if (logoutForm) logoutForm.style.display = 'none';
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const jwt = localStorage.getItem('jwt');
    const loginContainer = document.getElementById('loginContainer');
    const welcomeContainer = document.getElementById('welcomeContainer');
    const welcomeUser = document.getElementById('welcomeUser');

    if (jwt && welcomeContainer && loginContainer) {
        // Ocultar Login y mostrar bienvenidas
        loginContainer.style.display = 'none';
        welcomeContainer.style.display = '';

        // Obtener Usuario
        fetch('/profile', {
            headers: {
                'Authorization': 'Bearer ' + jwt
            }
        })
        .then(res => res.ok ? res.json() : null)
        .then(profile => {
            if (profile && welcomeUser) {
                welcomeUser.textContent = `Hola, ${profile.firstName} ${profile.lastName}`;
            }
        });
    } else if (loginContainer && welcomeContainer) {
        // Muestra el login y oculta la bienvenida
        loginContainer.style.display = '';
        welcomeContainer.style.display = 'none';
    }
});