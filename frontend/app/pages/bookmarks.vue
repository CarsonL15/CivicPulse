<template>
  <v-container class="py-8">
    <div class="mb-8 animate-fade-in-down">
      <div class="d-flex align-center ga-3 mb-1">
        <h1 class="page-title">My Bookmarks</h1>
        <v-chip v-if="!loading && bookmarks.length" color="primary" variant="tonal" size="small">
          {{ bookmarks.length }} saved
        </v-chip>
      </div>
      <p class="text-body-1 text-grey-darken-1">Events you've saved for later</p>
      <hr class="gradient-divider mt-3" style="width: 60px" />
    </div>

    <div v-if="loading" class="py-4">
      <v-row>
        <v-col v-for="n in 3" :key="n" cols="12" sm="6" lg="4">
          <v-skeleton-loader type="card" elevation="2" rounded="lg" />
        </v-col>
      </v-row>
    </div>

    <div v-else-if="bookmarks.length === 0" class="text-center py-16 animate-fade-in-up">
      <v-icon size="64" color="grey-lighten-1" icon="mdi-bookmark-outline" class="mb-4" />
      <p class="text-h6 font-weight-medium mb-1">No saved events yet</p>
      <p class="text-body-2 text-grey mb-6">Explore events and bookmark the ones you like</p>
      <v-btn to="/" color="primary" variant="flat" rounded="lg" prepend-icon="mdi-compass">
        Discover Events
      </v-btn>
    </div>

    <v-row v-else>
      <v-col
        v-for="(event, index) in bookmarks"
        :key="event.id"
        cols="12" sm="6" lg="4"
      >
        <EventCard :event="{ ...event, isBookmarked: true }" :index="index" @toggle-bookmark="handleRemove" />
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
