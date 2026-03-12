<template>
  <v-container fluid class="pa-0 fill-height">
    <v-row no-gutters class="fill-height">
      <!-- Decorative Panel (desktop only) -->
      <v-col cols="12" md="5" class="d-none d-md-flex auth-panel align-center justify-center">
        <div class="text-center pa-8 animate-fade-in-left">
          <v-icon icon="mdi-pulse" size="64" color="white" class="mb-4" />
          <h2 class="text-h4 text-white font-weight-bold mb-3">Welcome Back</h2>
          <p class="text-body-1 text-white" style="opacity: 0.85; max-width: 300px; margin: 0 auto">
            Stay connected with the civic events shaping your community.
          </p>
        </div>
      </v-col>

      <!-- Login Form -->
      <v-col cols="12" md="7" class="d-flex align-center justify-center">
        <div style="width: 100%; max-width: 420px" class="pa-6 animate-scale-in">
          <!-- Mobile logo -->
          <div class="text-center mb-6 d-md-none">
            <v-icon icon="mdi-pulse" size="40" color="primary" />
            <h1 class="text-h5 font-weight-bold text-gradient-primary">CivicPulse</h1>
          </div>

          <v-card rounded="xl" elevation="6" class="auth-card">
            <v-card-text class="pa-8">
              <h2 class="text-h5 font-weight-bold mb-1">Login</h2>
              <p class="text-body-2 text-grey-darken-1 mb-6">Sign in to your account</p>

              <v-alert v-if="error" type="error" class="mb-4" density="compact" rounded="lg">{{ error }}</v-alert>

              <v-form @submit.prevent="handleLogin">
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
                  :rules="[v => !!v || 'Password is required']"
                />
                <v-btn type="submit" color="primary" block size="large" rounded="lg" elevation="2" :loading="loading" class="auth-btn">
                  Login
                </v-btn>
              </v-form>

              <p class="text-center mt-6 text-body-2">
                Don't have an account?
                <NuxtLink to="/register" class="text-primary font-weight-medium">Sign up</NuxtLink>
              </p>
            </v-card-text>
          </v-card>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
const { login } = useAuth()

const email = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

async function handleLogin() {
  loading.value = true
  error.value = ''
  try {
    await login(email.value, password.value)
    navigateTo('/')
  } catch (e: any) {
    error.value = 'Invalid email or password'
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
</style>
