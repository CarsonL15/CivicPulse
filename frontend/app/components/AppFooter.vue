<template>
  <v-footer color="grey-darken-4" class="pa-0">
    <div class="gradient-divider w-100" />
    <v-container class="py-8">
      <v-row>
        <v-col cols="12" md="4" class="mb-4 mb-md-0">
          <div class="d-flex align-center ga-2 mb-3">
            <v-icon icon="mdi-pulse" color="white" />
            <span class="text-h6 font-weight-bold text-white">CivicPulse</span>
          </div>
          <p class="text-body-2 text-grey-lighten-1">
            Connecting young people with the civic events that shape their communities.
          </p>
        </v-col>

        <v-col cols="6" md="4">
          <p class="text-overline text-grey-lighten-1 mb-3">Quick Links</p>
          <div class="d-flex flex-column ga-2">
            <NuxtLink to="/" class="text-grey-lighten-2 text-body-2 text-decoration-none footer-link">
              Discover Events
            </NuxtLink>
            <NuxtLink to="/login" class="text-grey-lighten-2 text-body-2 text-decoration-none footer-link">
              Login
            </NuxtLink>
            <NuxtLink to="/register" class="text-grey-lighten-2 text-body-2 text-decoration-none footer-link">
              Sign Up
            </NuxtLink>
          </div>
        </v-col>

        <v-col cols="6" md="4">
          <p class="text-overline text-grey-lighten-1 mb-3">Project</p>
          <p class="text-body-2 text-grey-lighten-2 mb-1">EWU CSCD379</p>
          <p class="text-body-2 text-grey-lighten-2">Winter 2026</p>
        </v-col>
      </v-row>

      <v-divider class="my-6" color="grey-darken-2" />

      <p class="text-caption text-grey text-center">
        &copy; {{ new Date().getFullYear() }} CivicPulse. Built with Nuxt, Vuetify, and Azure AI.
      </p>
    </v-container>
  </v-footer>
</template>

<style scoped>
.footer-link:hover {
  color: white !important;
  transition: color 0.2s;
}
</style>
