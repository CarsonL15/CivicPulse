<template>
  <v-container class="py-8">
    <div class="mb-6 animate-fade-in-down">
      <h1 class="page-title">Admin Dashboard</h1>
      <p class="text-body-1 text-grey-darken-1 mt-1">Manage users and events</p>
      <hr class="gradient-divider mt-3" style="width: 60px" />
    </div>

    <!-- Stats Cards -->
    <v-row class="mb-6">
      <v-col cols="12" sm="4">
        <v-card rounded="lg" elevation="2" class="animate-fade-in-up stagger-1">
          <v-card-text class="d-flex align-center ga-3 pa-5">
            <v-avatar color="primary" variant="tonal" size="48">
              <v-icon icon="mdi-account-group" />
            </v-avatar>
            <div>
              <p class="text-h5 font-weight-bold">{{ users.length }}</p>
              <p class="text-caption text-grey">Total Users</p>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" sm="4">
        <v-card rounded="lg" elevation="2" class="animate-fade-in-up stagger-2">
          <v-card-text class="d-flex align-center ga-3 pa-5">
            <v-avatar color="secondary" variant="tonal" size="48">
              <v-icon icon="mdi-calendar-multiple" />
            </v-avatar>
            <div>
              <p class="text-h5 font-weight-bold">{{ events.length }}</p>
              <p class="text-caption text-grey">Total Events</p>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" sm="4">
        <v-card rounded="lg" elevation="2" class="animate-fade-in-up stagger-3">
          <v-card-text class="d-flex align-center ga-3 pa-5">
            <v-avatar color="warning" variant="tonal" size="48">
              <v-icon icon="mdi-clock-alert" />
            </v-avatar>
            <div>
              <p class="text-h5 font-weight-bold">{{ pendingCount }}</p>
              <p class="text-caption text-grey">Pending Approval</p>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Tabs -->
    <v-card rounded="lg" elevation="2" class="animate-fade-in-up stagger-4">
      <v-tabs v-model="tab" color="primary" grow>
        <v-tab value="users" prepend-icon="mdi-account-group">Users</v-tab>
        <v-tab value="events" prepend-icon="mdi-calendar-multiple">Events</v-tab>
      </v-tabs>

      <v-divider />

      <v-tabs-window v-model="tab">
        <v-tabs-window-item value="users">
          <v-data-table
            :headers="userHeaders"
            :items="users"
            :loading="usersLoading"
            hover
            class="admin-table"
          >
            <template #item.role="{ item }">
              <v-select
                :model-value="item.role"
                :items="['Citizen', 'Organizer', 'Admin']"
                variant="outlined"
                density="compact"
                hide-details
                style="max-width: 150px"
                @update:model-value="changeRole(item.id, $event)"
              />
            </template>
            <template #item.actions="{ item }">
              <v-btn icon size="small" color="error" variant="text" @click="deleteUser(item.id)">
                <v-icon>mdi-delete</v-icon>
                <v-tooltip activator="parent" location="top">Delete User</v-tooltip>
              </v-btn>
            </template>
          </v-data-table>
        </v-tabs-window-item>

        <v-tabs-window-item value="events">
          <v-data-table
            :headers="eventHeaders"
            :items="events"
            :loading="eventsLoading"
            hover
            class="admin-table"
          >
            <template #item.isApproved="{ item }">
              <v-chip :color="item.isApproved ? 'success' : 'warning'" size="small" variant="flat">
                <v-icon start :icon="item.isApproved ? 'mdi-check-circle' : 'mdi-clock'" size="small" />
                {{ item.isApproved ? 'Approved' : 'Pending' }}
              </v-chip>
            </template>
            <template #item.actions="{ item }">
              <v-btn
                icon size="small" variant="text"
                :color="item.isApproved ? 'warning' : 'success'"
                @click="toggleApprove(item.id)"
              >
                <v-icon>{{ item.isApproved ? 'mdi-eye-off' : 'mdi-check' }}</v-icon>
                <v-tooltip activator="parent" location="top">
                  {{ item.isApproved ? 'Unapprove' : 'Approve' }}
                </v-tooltip>
              </v-btn>
              <v-btn icon size="small" color="error" variant="text" @click="deleteEvent(item.id)">
                <v-icon>mdi-delete</v-icon>
                <v-tooltip activator="parent" location="top">Delete Event</v-tooltip>
              </v-btn>
            </template>
          </v-data-table>
        </v-tabs-window-item>
      </v-tabs-window>
    </v-card>
  </v-container>
</template>

<script setup lang="ts">
definePageMeta({ middleware: ['admin'] })

const api = useApi()

const tab = ref('users')
const users = ref<any[]>([])
const events = ref<any[]>([])
const usersLoading = ref(true)
const eventsLoading = ref(true)

const pendingCount = computed(() => events.value.filter(e => !e.isApproved).length)

const userHeaders = [
  { title: 'Name', key: 'displayName' },
  { title: 'Email', key: 'email' },
  { title: 'Role', key: 'role' },
  { title: 'Actions', key: 'actions', sortable: false },
]

const eventHeaders = [
  { title: 'Title', key: 'title' },
  { title: 'Category', key: 'category' },
  { title: 'Organizer', key: 'organizerName' },
  { title: 'Status', key: 'isApproved' },
  { title: 'Actions', key: 'actions', sortable: false },
]

async function changeRole(userId: string, role: string) {
  await api.put(`/api/admin/users/${userId}/role`, { role })
  const user = users.value.find(u => u.id === userId)
  if (user) user.role = role
}

async function deleteUser(userId: string) {
  await api.del(`/api/admin/users/${userId}`)
  users.value = users.value.filter(u => u.id !== userId)
}

async function toggleApprove(eventId: number) {
  await api.put(`/api/admin/events/${eventId}/approve`)
  const event = events.value.find(e => e.id === eventId)
  if (event) event.isApproved = !event.isApproved
}

async function deleteEvent(eventId: number) {
  await api.del(`/api/admin/events/${eventId}`)
  events.value = events.value.filter(e => e.id !== eventId)
}

onMounted(async () => {
  try {
    users.value = await api.get('/api/admin/users')
  } catch (e) { console.error(e) }
  finally { usersLoading.value = false }

  try {
    events.value = await api.get('/api/admin/events')
  } catch (e) { console.error(e) }
  finally { eventsLoading.value = false }
})
</script>

<style scoped>
.admin-table :deep(tr:hover) {
  background: rgba(21, 101, 192, 0.04) !important;
}
</style>
