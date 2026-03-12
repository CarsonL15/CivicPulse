<template>
  <v-container fluid class="pa-0 fill-height">
    <v-row no-gutters class="fill-height">
      <!-- Decorative Panel (desktop only) -->
      <v-col cols="12" md="5" class="d-none d-md-flex auth-panel align-center justify-center">
        <div class="text-center pa-8 animate-fade-in-left">
          <v-icon icon="mdi-pulse" size="64" color="white" class="mb-4" />
          <h2 class="text-h4 text-white font-weight-bold mb-3">Join Your Community</h2>
          <p class="text-body-1 text-white" style="opacity: 0.85; max-width: 300px; margin: 0 auto">
            Discover civic events, bookmark what matters, and stay informed.
          </p>
        </div>
      </v-col>

      <!-- Register Form -->
      <v-col cols="12" md="7" class="d-flex align-center justify-center">
        <div style="width: 100%; max-width: 420px" class="pa-6 animate-scale-in">
          <!-- Mobile logo -->
          <div class="text-center mb-6 d-md-none">
            <v-icon icon="mdi-pulse" size="40" color="primary" />
            <h1 class="text-h5 font-weight-bold text-gradient-primary">CivicPulse</h1>
          </div>

          <v-card rounded="xl" elevation="6" class="auth-card">
            <v-card-text class="pa-8">
              <h2 class="text-h5 font-weight-bold mb-1">Create Account</h2>
              <p class="text-body-2 text-grey-darken-1 mb-6">Get started with CivicPulse</p>

              <v-alert v-if="error" type="error" class="mb-4" density="compact" rounded="lg">{{ error }}</v-alert>

              <v-form @submit.prevent="handleRegister">
                <v-text-field
                  v-model="displayName"
                  label="Display Name"
                  variant="outlined"
                  prepend-inner-icon="mdi-account-outline"
                  class="mb-3"
                  :rules="[v => !!v || 'Name is required']"
                />
                <v-text-field
                  v-model="email"
                  label="Email"
                  type="email"
                  variant="outlined"
                  prepend-inner-icon="mdi-email-outline"
                  class="mb-3"
                  :rules="[v => !!v || 'Email is required']"
                />
                <v-text-field
                  v-model="password"
                  label="Password"
                  type="password"
                  variant="outlined"
                  prepend-inner-icon="mdi-lock-outline"
                  class="mb-4"
                  :rules="[v => v.length >= 6 || 'Password must be at least 6 characters']"
                />

                <!-- Role Selector Cards -->
                <p class="text-overline text-primary mb-3">I am a...</p>
                <v-row class="mb-4">
                  <v-col cols="6">
                    <v-card
                      :variant="role === 'Citizen' ? 'tonal' : 'outlined'"
                      :color="role === 'Citizen' ? 'primary' : ''"
                      :elevation="role === 'Citizen' ? 2 : 0"
                      rounded="lg"
                      class="pa-4 text-center role-card"
                      @click="role = 'Citizen'"
                    >
                      <v-icon :icon="role === 'Citizen' ? 'mdi-check-circle' : 'mdi-account'" size="32" :color="role === 'Citizen' ? 'primary' : 'grey'" class="mb-2" />
                      <p class="text-body-2 font-weight-bold">Citizen</p>
                      <p class="text-caption text-grey">Discover events</p>
                    </v-card>
                  </v-col>
                  <v-col cols="6">
                    <v-card
                      :variant="role === 'Organizer' ? 'tonal' : 'outlined'"
                      :color="role === 'Organizer' ? 'primary' : ''"
                      :elevation="role === 'Organizer' ? 2 : 0"
                      rounded="lg"
                      class="pa-4 text-center role-card"
                      @click="role = 'Organizer'"
                    >
                      <v-icon :icon="role === 'Organizer' ? 'mdi-check-circle' : 'mdi-bullhorn'" size="32" :color="role === 'Organizer' ? 'primary' : 'grey'" class="mb-2" />
                      <p class="text-body-2 font-weight-bold">Organizer</p>
                      <p class="text-caption text-grey">Post events</p>
                    </v-card>
                  </v-col>
                </v-row>

                <v-btn type="submit" color="primary" block size="large" rounded="lg" elevation="2" :loading="loading" class="auth-btn">
                  Create Account
                </v-btn>
              </v-form>

              <p class="text-center mt-6 text-body-2">
                Already have an account?
                <NuxtLink to="/login" class="text-primary font-weight-medium">Login</NuxtLink>
              </p>
            </v-card-text>
          </v-card>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
const { register } = useAuth()

const displayName = ref('')
const email = ref('')
const password = ref('')
const role = ref('Citizen')
const error = ref('')
const loading = ref(false)

async function handleRegister() {
  loading.value = true
  error.value = ''
  try {
    await register(email.value, password.value, displayName.value, role.value)
    navigateTo('/')
  } catch (e: any) {
    error.value = e.message || 'Registration failed'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.auth-panel {
  background: linear-gradient(135deg, #0f2b3e 0%, #1a4a6e 100%);
  position: relative;
  overflow: hidden;
}

.auth-panel::before {
  content: '';
  position: absolute;
  top: -30%;
  right: -20%;
  width: 400px;
  height: 400px;
  border-radius: 50%;
  background: rgba(38, 166, 154, 0.12);
}

.auth-panel::after {
  content: '';
  position: absolute;
  bottom: -20%;
  left: -10%;
  width: 300px;
  height: 300px;
  border-radius: 50%;
  background: rgba(255, 112, 67, 0.08);
}

.auth-card {
  border-top: 3px solid transparent;
  border-image: linear-gradient(90deg, #0f2b3e, #1a4a6e) 1;
}

.auth-btn {
  text-transform: none;
  font-weight: 600;
  font-size: 1rem;
  letter-spacing: 0;
}

.role-card {
  cursor: pointer;
  transition: all 0.2s ease;
}

.role-card:hover {
  transform: translateY(-2px);
}
</style>
