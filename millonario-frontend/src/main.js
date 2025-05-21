// src/main.js
import 'bootstrap/dist/css/bootstrap.min.css'
import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia' // Importa createPinia

import App from './App.vue'
import router from './router'
import { useAuthStore } from './stores/auth' // Importa tu store de autenticación

const app = createApp(App)
const pinia = createPinia() // Crea la instancia de Pinia

app.use(pinia) // Usa Pinia con la aplicación Vue
app.use(router)

// Inicializa el store de autenticación después de que Pinia esté disponible
const authStore = useAuthStore();
authStore.initializeAuth(); // Llama a la acción para inicializar el estado

app.mount('#app')