<template>
  <v-container class="py-6">
    <h1 class="text-h4 font-weight-bold mb-6">My Bookmarks</h1>

    <div v-if="loading" class="d-flex justify-center py-8">
      <v-progress-circular indeterminate color="primary" />
    </div>

    <div v-else-if="bookmarks.length === 0" class="text-center py-8">
      <v-icon size="64" color="grey-lighten-1" icon="mdi-bookmark-outline" />
      <p class="text-h6 text-grey mt-2">No saved events yet</p>
      <v-btn to="/" color="primary" variant="outlined" class="mt-4">Discover Events</v-btn>
    </div>

    <v-row v-else>
      <v-col
        v-for="event in bookmarks"
        :key="event.id"
        cols="12" sm="6" lg="4"
      >
        <EventCard :event="{ ...event, isBookmarked: true }" @toggle-bookmark="handleRemove" />
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
definePageMeta({ middleware: ['auth'] })

const { getBookmarks, removeBookmark } = useBookmarks()

const bookmarks = ref<any[]>([])
const loading = ref(true)

async function handleRemove(eventId: number) {
  try {
    await removeBookmark(eventId)
    bookmarks.value = bookmarks.value.filter(b => b.id !== eventId)
  } catch (e) {
    console.error('Failed to remove bookmark:', e)
  }
}

onMounted(async () => {
  try {
    bookmarks.value = await getBookmarks()
  } catch (e) {
    console.error('Failed to load bookmarks:', e)
  } finally {
    loading.value = false
  }
})
</script>
