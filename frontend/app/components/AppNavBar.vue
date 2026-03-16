<template>
  <v-app-bar class="navbar-gradient" density="comfortable" flat>
    <v-app-bar-title>
      <NuxtLink to="/" class="text-white text-decoration-none d-flex align-center ga-2">
        <v-icon icon="mdi-pulse" size="28" />
        <span class="navbar-brand">CivicPulse</span>
      </NuxtLink>
    </v-app-bar-title>

    <template v-if="!mobile">
      <v-btn to="/" variant="text" class="nav-link" prepend-icon="mdi-compass">Discover</v-btn>
      <v-btn v-if="isAuthenticated" to="/bookmarks" variant="text" class="nav-link" prepend-icon="mdi-bookmark-outline">Bookmarks</v-btn>
      <v-btn v-if="isOrganizer" to="/events/create" variant="text" class="nav-link" prepend-icon="mdi-plus-circle-outline">Create Event</v-btn>
      <v-btn v-if="isAdmin" to="/admin" variant="text" class="nav-link" prepend-icon="mdi-shield-account">Admin</v-btn>
    </template>

    <v-spacer />

    <template v-if="isAuthenticated">
      <v-chip class="mr-2" color="white" variant="outlined" size="small" prepend-icon="mdi-account-circle">
        {{ user?.displayName }}
      </v-chip>
      <v-btn icon variant="text" @click="logout">
        <v-icon>mdi-logout</v-icon>
        <v-tooltip activator="parent" location="bottom">Logout</v-tooltip>
      </v-btn>
    </template>
    <template v-else>
      <v-btn to="/login" variant="text">Login</v-btn>
      <v-btn to="/register" variant="outlined" color="white" class="ml-1">Sign Up</v-btn>
    </template>

    <v-app-bar-nav-icon v-if="mobile" @click="drawer = !drawer" />
  </v-app-bar>

  <v-navigation-drawer v-if="mobile" v-model="drawer" temporary rounded="e">
    <div class="pa-4 navbar-gradient">
      <div class="d-flex align-center ga-2">
        <v-icon icon="mdi-pulse" color="white" size="24" />
        <span class="text-h6 font-weight-bold text-white">CivicPulse</span>
      </div>
    </div>
    <v-list nav class="mt-2">
      <v-list-item to="/" title="Discover" prepend-icon="mdi-compass" />
      <v-list-item v-if="isAuthenticated" to="/bookmarks" title="Bookmarks" prepend-icon="mdi-bookmark" />
      <v-list-item v-if="isOrganizer" to="/events/create" title="Create Event" prepend-icon="mdi-plus-circle" />
      <v-list-item v-if="isAdmin" to="/admin" title="Admin" prepend-icon="mdi-shield-account" />
      <v-divider class="my-2" />
      <template v-if="isAuthenticated">
        <v-list-item :title="user?.displayName" subtitle="Signed in" prepend-icon="mdi-account-circle" />
        <v-list-item title="Logout" prepend-icon="mdi-logout" @click="logout" />
      </template>
      <template v-else>
        <v-list-item to="/login" title="Login" prepend-icon="mdi-login" />
        <v-list-item to="/register" title="Sign Up" prepend-icon="mdi-account-plus" />
      </template>
    </v-list>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { useDisplay } from 'vuetify'

const { mobile } = useDisplay()
const drawer = ref(false)
const { user, isAuthenticated, isOrganizer, isAdmin, logout } = useAuth()
</script>

<style scoped>
.navbar-gradient {
  background: #0f2b3e !important;
}

.navbar-brand {
  font-weight: 800;
  font-size: 1.25rem;
  letter-spacing: -0.02em;
}

.nav-link {
  text-transform: none;
  letter-spacing: 0;
  font-weight: 500;
  color: #ffffff !important;
}
</style>
