<template>
  <v-container class="py-8" style="max-width: 450px">
    <v-card rounded="lg" elevation="2">
      <v-card-item>
        <v-card-title class="text-h5 text-center">Login</v-card-title>
      </v-card-item>
      <v-card-text>
        <v-alert v-if="error" type="error" class="mb-4" density="compact">{{ error }}</v-alert>

        <v-form @submit.prevent="handleLogin">
          <v-text-field
            v-model="email"
            label="Email"
            type="email"
            variant="outlined"
            class="mb-3"
            :rules="[v => !!v || 'Email is required']"
          />
          <v-text-field
            v-model="password"
            label="Password"
            type="password"
            variant="outlined"
            class="mb-3"
            :rules="[v => !!v || 'Password is required']"
          />
          <v-btn type="submit" color="primary" block size="large" :loading="loading">
            Login
          </v-btn>
        </v-form>

        <p class="text-center mt-4 text-body-2">
          Don't have an account?
          <NuxtLink to="/register" class="text-primary">Sign up</NuxtLink>
        </p>
      </v-card-text>
    </v-card>
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
