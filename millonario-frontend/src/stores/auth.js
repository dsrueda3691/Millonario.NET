// src/stores/auth.js
import { defineStore } from 'pinia';
import axios from 'axios';
import router from '@/router'; // Importa el router para redirecciones

// La URL base de tu API de ASP.NET Core
const API_URL = 'https://localhost:7254/api/Auth';

export const useAuthStore = defineStore('auth', {
    state: () => {
        const userString = localStorage.getItem('user');
        const tokenString = localStorage.getItem('token');

        let user = null;
        let token = null;

        if (userString) {
            try {
                user = JSON.parse(userString);
            } catch (e) {
                console.error("Error parsing user from localStorage:", e);
                localStorage.removeItem('user');
            }
        }

        if (tokenString) {
            token = tokenString;
        }

        return {
            user: user,     // Objeto de usuario si está logueado, de lo contrario null
            token: token,   // Token JWT si está logueado, de lo contrario null
            authError: null,
        };
    },
    getters: {
        isAuthenticated: (state) => !!state.token,
    },
    actions: {
        initializeAuth() {
            if (this.token) {
                axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`;
            }
        },

        async login(credentials) {
            this.authError = null;
            try {
                const response = await axios.post(`${API_URL}/login`, credentials);

                // --- CAMBIO CLAVE AQUÍ ---
                // Tu API devuelve 'username' y 'userId' directamente en response.data
                // No hay un objeto 'user' anidado.
                this.user = {
                    username: response.data.username,
                    userId: response.data.userId
                };
                this.token = response.data.token;

                // Guarda el usuario y el token en localStorage para persistencia
                localStorage.setItem('user', JSON.stringify(this.user)); // Guarda el objeto user completo
                localStorage.setItem('token', this.token);

                // Configura el encabezado de autorización predeterminado
                axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`;

                router.push('/game');
            } catch (error) {
                console.error('Error de login:', error.response?.data || error.message);
                this.authError = error.response?.data?.message || 'Credenciales incorrectas o error de red.';
            }
        },

        async register(userData) {
            this.authError = null;
            try {
                const response = await axios.post(`${API_URL}/register`, userData);
                console.log('Registro exitoso:', response.data);
                router.push('/login');
            } catch (error) {
                console.error('Error de registro:', error.response?.data || error.message);
                this.authError = error.response?.data?.message || 'Error en el registro. Intenta de nuevo.';
            }
        },

        logout() {
            this.user = null;
            this.token = null;
            localStorage.removeItem('user');
            localStorage.removeItem('token');
            delete axios.defaults.headers.common['Authorization'];
            router.push('/login');
        },
    },
});
