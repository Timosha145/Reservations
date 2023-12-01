const app = Vue.createApp({
    data() {
        return {
            reservations: [],
            services: [],
            newReservation: {
                name: '',
                serviceId: '',
                time: '',
                salon: '',
                service: '',
                carNumber: '',
            },
            newService: {
                id: '',
                name: '',
                price: '1',
                description: '',
                duration: ''
            },
            user: {
                id: '',
                name: '',
                phoneNumber: '',
                email: ''
            },
            editingReservation: null,
            editingService: null,
            isEditing: false,
            isAdmin: 0,
            userId: null,
        };
    },
    async mounted() {
        await this.loadUser();
        await this.loadReservations();
        console.log(this.reservations);
        this.services = await (await fetch('https://localhost:7010/getServices')).json();
    },
    methods: {
        async loadReservations() {
            this.reservations = await (await fetch('https://localhost:7010/getReservations')).json();

            for (const reservation of this.reservations) {
                try {
                    const userData = await (await fetch(`https://localhost:7010/User/getUserData/${reservation.clientId}`)).json();
                    reservation.service = await (await fetch(`https://localhost:7010/getServiceById/${reservation.serviceId}`)).json();
                    reservation.name = userData.name;
                    reservation.phoneNumber = userData.phoneNumber;
                } catch (error) {
                    console.error('Error fetching user or service data:', error);
                }
            }
        },
        async loadUser() {
            try {
                const user = localStorage.getItem('user');
                if (user != null) {
                    const userData = JSON.parse(user);

                    this.user = userData;
                    this.isAdmin = userData.isAdmin;
                    this.userId = userData.id;
                    this.newReservation.name = userData.name;
                    this.newReservation.phoneNumber = userData.phoneNumber;
                } else {
                    window.location.href = 'login.html';
                }
            } catch (error) {
                window.location.href = 'login.html';
                console.error('Error loading user data');
            }
        },
        getUserData: async function (id) {
            try {
                return await (await fetch(`https://localhost:7010/User/getUserData/${id}`)).json();
            } catch (error) {
                console.error('Could not find user');
            }
        },
        deleteReservation: async function (id) {
            try {
                await fetch(`https://localhost:7010/deleteReservation/${id}`, {
                    method: 'DELETE'
                });
                location.reload();
            } catch (error) {
                console.error('Could not delete a reservation:', error);
            }
        },
        deleteService: async function (id) {
            try {
                await fetch(`https://localhost:7010/deleteService/${id}`, {
                    method: 'DELETE'
                });
                location.reload();
            } catch (error) {
                console.error('Could not delete a reservation:', error);
            }
        },
        async addReservation() {
            if (!this.newReservation.time || !this.newReservation.salon || !this.newReservation.service || !this.newReservation.carNumber) {
                alert('Please fill in all fields with valid data.');
                return;
            }
        
            try {
                await fetch(`https://localhost:7010/addReservation/${this.userId}/${this.newReservation.service.id}/${this.newReservation.time}/${this.newReservation.salon}/${this.newReservation.carNumber}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(this.newReservation)
                });
                alert('Reservation was added!');
                location.reload();
            } catch (error) {
                console.error('Unable to add a new reservation:', error);
            }
        },
        async addService() {
            if (!this.newService.name || !this.newService.price || !this.newService.description || !this.newService.duration) {
                alert('Please fill in all fields with valid data.');
                return;
            }

            try {
                const response = await fetch(`https://localhost:7010/addService/${encodeURIComponent(this.newService.name)}/${this.newService.price}/${encodeURIComponent(this.newService.description)}/${encodeURIComponent(this.newService.duration)}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
                alert('Service was added!');
                location.reload();
            } catch (error) {
                console.error('Unable to add a new service:', error);
            }
        },
        editReservation(reservation) {
            this.editingReservation = { ...reservation };
            this.isEditing = true;
        },
        editService(service) {
            this.editingService = { ...service };
            this.isEditing = true;
        },
        cancelEdit() {
            this.editingReservation = null;
            this.isEditing = false;
        },
        cancelEditService() {
            this.editingService = null;
            this.isEditing = false;
        },
        async saveEdit(reservation) {
            if (!this.editingReservation.service || !this.editingReservation.date || !this.editingReservation.salon || !this.editingReservation.carNumber) {
                alert('Please fill in all fields with valid data.');
                return;
            }

            try {
                const response = await fetch(`https://localhost:7010/editReservation/${reservation.id}/${this.userId}/${this.editingReservation.service.id}/${this.editingReservation.date}/${this.editingReservation.salon}/${this.editingReservation.carNumber}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(this.editingReservation)
                });

                if (response.ok) {
                    console.log(response)
                    this.isEditing = false;
                    location.reload();
                } else {
                    console.error('Failed to update reservation:', response.statusText);
                }
            } catch (error) {
                console.error('Unable to edit reservation:', error);
            }
        },
        async saveEditService(service) {
            if (!this.editingService.name || !this.editingService.price || !this.editingService.description || !this.editingService.duration) {
                alert('Please fill in all fields with valid data.');
                return;
            }

            try {
                alert(`https://localhost:7010/updateService/${service.id}/${encodeURIComponent(this.editingService.name)}/${this.editingService.price}/${encodeURIComponent(this.editingService.description)}/${encodeURIComponent(this.editingService.duration)}`);
                await fetch(`https://localhost:7010/updateService/${service.id}/${encodeURIComponent(this.editingService.name)}/${this.editingService.price}/${encodeURIComponent(this.editingService.description)}/${encodeURIComponent(this.editingService.duration)}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
                this.isEditing = false;
                this.editingService = null;
                location.reload();
            } catch (error) {
                console.error('Unable to save edit:', error);
            }
        },
        async saveUserSettings() {
            if (!this.user.name || !this.user.email || !this.user.phoneNumber) {
                alert('Please fill in all fields with valid data.');
                return;
            }

            try {
                const updatedUser = {
                    name: encodeURIComponent(this.user.name),
                    email: encodeURIComponent(this.user.email),
                    phoneNumber: encodeURIComponent(this.user.phoneNumber)
                };

                alert(`https://localhost:7010/User/updateUserData/${this.user.id}/${updatedUser.name}/${updatedUser.email}/${updatedUser.phoneNumber}`);
                await fetch(`https://localhost:7010/User/updateUserData/${this.user.id}/${updatedUser.name}/${updatedUser.email}/${updatedUser.phoneNumber}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });

                localStorage.setItem('user', JSON.stringify(this.user));

                location.reload();
            } catch (error) {
                console.error('Unable to save edit:', error);
            }
        },
        async logout() {
            localStorage.removeItem('user');
            window.location.href = 'login.html'
        },
        async goToServices() {
            window.location.href='services.html';
        },
        async goToIndex() {
            window.location.href='index.html';
        },
        openReservationModal() {
            $('#userSettingsModal').modal('show');
        },
        closeReservationModal() {
            $('#userSettingsModal').modal('hide');
        },
        formatDuration(duration) {
            const [hours, minutes] = duration.split(':');
            const formattedHours = `${String(Number(hours)).padStart(2, '0')}H`;
            const formattedMinutes = `${minutes}Min`;
            return `${formattedHours} ${formattedMinutes}`;
        },
    }
}).mount('#app');