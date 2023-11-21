const registrationApp = Vue.createApp({
    data() {
        return {
            id: '',
            name: '',
            email: '',
            password: '',
        };
    },
    created() {
        
    },
    methods: {
        async registerUser() {
            if (!this.name || !this.email || !this.password) {
                alert('Please fill in all fields.');
                return;
            }

            try {
                const registerResponse = await fetch(`https://localhost:7010/User/register/${encodeURIComponent(this.name)}/${encodeURIComponent(this.email)}/${encodeURIComponent(this.password)}`, {
                    method: 'POST',
                });

                if (registerResponse.ok) {
                    alert('Registration successful. You can now login.');
                    window.location.href = 'login.html';
                } else {
                    const data = await registerResponse.json();
                    alert(`Registration failed: ${data.error}`);
                }
            } catch (error) {
                console.error('Registration error:', error);
            }
        }
    }
});

registrationApp.mount('#registration-app');
