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
                    //window.location.href = 'login.html';
                }
            } catch (error) {
                //window.location.href = 'login.html';
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
            const serviceToDelete = this.services.find(service => service.id === id);
            if (!serviceToDelete) {
                console.error('Service was not found:', id);
                return;
            }

            try {
                await fetch(`http://localhost:8080/services/${id}`, {
                    method: 'DELETE'
                });
                this.services = this.services.filter(service => service.id !== id);
            } catch (error) {
                console.error('Couldnt delete a service:', error);
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
                const response = await fetch('http://localhost:8080/services', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(this.newService)
                });

                if (response.ok) {
                    const newService = await response.json();
                    this.services.push(newService);
                    this.newService = {
                        name: '',
                        price: '',
                        description: '',
                        duration: '',
                    };
                }
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
            try {
                const response = await fetch(`http://localhost:8080/services/${service.id}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(this.editingService)
                });

                if (response.ok) {
                    const updatedService = await response.json();
                    const index = this.services.findIndex(r => r.id === service.id);
                    if (index !== -1) {
                        this.services[index] = updatedService;
                    }

                    this.isEditing = false;
                    this.editingService = null;
                }
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
    }
}).mount('#app');