<template>
  <v-app>
    <AppNavBar />
    <v-main>
      <NuxtPage />
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
const { init } = useAuth()

onMounted(() => {
  init()
})
</script>
