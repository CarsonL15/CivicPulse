<template>
  <v-container class="py-6">
    <h1 class="text-h4 font-weight-bold mb-6">Admin Dashboard</h1>

    <v-tabs v-model="tab" color="primary">
      <v-tab value="users">Users</v-tab>
      <v-tab value="events">Events</v-tab>
    </v-tabs>

    <v-tabs-window v-model="tab" class="mt-4">
      <v-tabs-window-item value="users">
        <v-data-table
          :headers="userHeaders"
          :items="users"
          :loading="usersLoading"
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
            </v-btn>
          </template>
        </v-data-table>
      </v-tabs-window-item>

      <v-tabs-window-item value="events">
        <v-data-table
          :headers="eventHeaders"
          :items="events"
          :loading="eventsLoading"
        >
          <template #item.isApproved="{ item }">
            <v-chip :color="item.isApproved ? 'success' : 'warning'" size="small">
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
            </v-btn>
            <v-btn icon size="small" color="error" variant="text" @click="deleteEvent(item.id)">
              <v-icon>mdi-delete</v-icon>
            </v-btn>
          </template>
        </v-data-table>
      </v-tabs-window-item>
    </v-tabs-window>
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
