<template>
  <v-app-bar color="primary" density="comfortable">
    <v-app-bar-title>
      <NuxtLink to="/" class="text-white text-decoration-none font-weight-bold">
        CivicPulse
      </NuxtLink>
    </v-app-bar-title>

    <template v-if="!mobile">
      <v-btn to="/" variant="text">Discover</v-btn>
      <v-btn v-if="isAuthenticated" to="/bookmarks" variant="text">Bookmarks</v-btn>
      <v-btn v-if="isOrganizer" to="/events/create" variant="text">Create Event</v-btn>
      <v-btn v-if="isAdmin" to="/admin" variant="text">Admin</v-btn>
    </template>

    <v-spacer />

    <template v-if="isAuthenticated">
      <v-chip class="mr-2" color="white" variant="outlined" size="small">
        {{ user?.displayName }}
      </v-chip>
      <v-btn icon @click="logout">
        <v-icon>mdi-logout</v-icon>
      </v-btn>
    </template>
    <template v-else>
      <v-btn to="/login" variant="text">Login</v-btn>
      <v-btn to="/register" variant="outlined" color="white">Sign Up</v-btn>
    </template>

    <v-app-bar-nav-icon v-if="mobile" @click="drawer = !drawer" />
  </v-app-bar>

  <v-navigation-drawer v-if="mobile" v-model="drawer" temporary>
    <v-list>
      <v-list-item to="/" title="Discover" prepend-icon="mdi-magnify" />
      <v-list-item v-if="isAuthenticated" to="/bookmarks" title="Bookmarks" prepend-icon="mdi-bookmark" />
      <v-list-item v-if="isOrganizer" to="/events/create" title="Create Event" prepend-icon="mdi-plus" />
      <v-list-item v-if="isAdmin" to="/admin" title="Admin" prepend-icon="mdi-shield-account" />
    </v-list>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { useDisplay } from 'vuetify'

const { mobile } = useDisplay()
const drawer = ref(false)
const { user, isAuthenticated, isOrganizer, isAdmin, logout } = useAuth()
</script>
