<template>
  <v-container class="py-8" style="max-width: 450px">
    <v-card rounded="lg" elevation="2">
      <v-card-item>
        <v-card-title class="text-h5 text-center">Create Account</v-card-title>
      </v-card-item>
      <v-card-text>
        <v-alert v-if="error" type="error" class="mb-4" density="compact">{{ error }}</v-alert>

        <v-form @submit.prevent="handleRegister">
          <v-text-field
            v-model="displayName"
            label="Display Name"
            variant="outlined"
            class="mb-3"
            :rules="[v => !!v || 'Name is required']"
          />
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
            :rules="[v => v.length >= 6 || 'Password must be at least 6 characters']"
          />

          <v-select
            v-model="role"
            label="I am a..."
            :items="roles"
            item-title="label"
            item-value="value"
            variant="outlined"
            class="mb-3"
          />

          <v-btn type="submit" color="primary" block size="large" :loading="loading">
            Create Account
          </v-btn>
        </v-form>

        <p class="text-center mt-4 text-body-2">
          Already have an account?
          <NuxtLink to="/login" class="text-primary">Login</NuxtLink>
        </p>
      </v-card-text>
    </v-card>
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

const roles = [
  { value: 'Citizen', label: 'Citizen — I want to discover events' },
  { value: 'Organizer', label: 'Organizer — I want to post events' },
]

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
