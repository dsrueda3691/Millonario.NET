<template>
  <section
    class="vh-100 d-flex justify-content-center align-items-center"
    style="background-color: #0b1c3d"
  >
    <div class="container py-5">
      <div class="row justify-content-center">
        <div class="col-12 col-md-8 col-lg-6 col-xl-5">
          <div
            class="card shadow-lg"
            style="border-radius: 1rem; background-color: rgba(0, 0, 0, 0.7)"
          >
            <div class="card-body p-5 text-center">
              <h2 class="fw-bold mb-4 text-warning">Registrarse</h2>

              <form @submit.prevent="handleRegister">
                <div class="form-outline mb-4">
                  <input
                    type="text"
                    id="username"
                    v-model="username"
                    class="form-control form-control-lg text-white"
                    placeholder="Usuario"
                    required
                    style="background-color: #222; border: 1px solid #444"
                  />
                </div>

                <div class="form-outline mb-4">
                  <input
                    type="email"
                    id="email"
                    v-model="email"
                    class="form-control form-control-lg text-white"
                    placeholder="Email"
                    required
                    style="background-color: #222; border: 1px solid #444"
                  />
                </div>

                <div class="form-outline mb-4">
                  <input
                    type="password"
                    id="password"
                    v-model="password"
                    class="form-control form-control-lg text-white"
                    placeholder="Contraseña"
                    required
                    style="background-color: #222; border: 1px solid #444"
                  />
                </div>

                <p v-if="authStore.authError" class="text-danger mb-3">{{ authStore.authError }}</p>

                <button type="submit" class="btn btn-warning btn-lg w-100">Registrarse</button>
              </form>

              <div class="mt-4">
                <p class="mb-0 text-white">
                  ¿Ya tienes una cuenta?
                  <router-link to="/login" class="text-warning fw-bold"
                    >Inicia sesión aquí</router-link
                  >
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref } from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

const username = ref('')
const email = ref('')
const password = ref('')

const handleRegister = async () => {
  await authStore.register({
    username: username.value,
    email: email.value,
    password: password.value,
  })
  // El store ya maneja la redirección al login si el registro es exitoso
}
</script>

<style scoped>
.text-warning {
  color: gold !important;
}

.text-warning:hover {
  color: #ffd700 !important;
  text-decoration: underline !important;
}
.btn-warning {
  background-color: gold !important;
  border-color: gold !important;
  color: black !important;
}
.btn-warning:hover {
  background-color: #ffd700 !important;
  border-color: #ffd700 !important;
  transform: scale(1.02);
}
.card {
  background-color: rgba(0, 0, 0, 0.7);
  border: 2px solid gold;
}

.form-control-lg {
  background-color: #222 !important;
  border: 1px solid #444 !important;
  color: white !important;
  padding: 12px 15px !important;
  height: auto !important;
  font-size: 1.1em !important;
}

.form-control-lg::placeholder {
  color: #bbb !important;
  opacity: 1 !important;
}

.form-control-lg:focus {
  border-color: gold !important;
  box-shadow: 0 0 8px gold !important;
  background-color: #333 !important;
}
</style>
