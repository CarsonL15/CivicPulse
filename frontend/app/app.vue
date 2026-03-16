<template>
  <v-app style="font-family: 'Inter', sans-serif">
    <AppNavBar />
    <v-main style="background: #fff">
      <NuxtPage />
    </v-main>
    <AppFooter />
  </v-app>
</template>

<script setup lang="ts">
const { init } = useAuth()

onMounted(() => {
  init()
})
</script>
