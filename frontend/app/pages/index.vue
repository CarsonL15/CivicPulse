<template>
  <div>
    <!-- Hero Section -->
    <v-container fluid class="bg-primary pa-8 mb-6">
      <v-container>
        <h1 class="text-h3 text-white font-weight-bold mb-2">
          Discover civic events that matter to you
        </h1>
        <p class="text-h6 text-white mb-6" style="opacity: 0.85">
          Find city council meetings, town halls, and community events — explained in plain language.
        </p>

        <SearchBar
          v-model="search.query.value"
          v-model:image-file="search.imageFile.value"
          @search="onSearch"
          @clear="onClear"
          @image-search="onImageSearch"
        />

        <div class="mt-4">
          <CategoryFilter v-model="search.category.value" />
        </div>
      </v-container>
    </v-container>

    <!-- Results / Events -->
    <v-container>
      <template v-if="hasSearched">
        <h2 class="text-h5 mb-4">
          <v-icon v-if="search.imageFile.value" icon="mdi-image-search" class="mr-1" />
          {{ search.imageFile.value ? 'Image Search Results' : 'Search Results' }}
          <v-chip v-if="search.results.value.length" size="small" class="ml-2">
            {{ search.results.value.length }}
          </v-chip>
        </h2>

        <div v-if="search.loading.value" class="d-flex justify-center py-8">
          <v-progress-circular indeterminate color="primary" />
        </div>

        <div v-else-if="search.results.value.length === 0" class="text-center py-8">
          <v-icon size="64" color="grey-lighten-1" icon="mdi-magnify" />
          <p class="text-h6 text-grey mt-2">No results found. Try a different search.</p>
        </div>

        <v-row v-else>
          <v-col
            v-for="result in search.results.value"
            :key="result.id"
            cols="12" sm="6" lg="4"
          >
            <v-card
              elevation="2"
              rounded="lg"
              class="search-result-card"
              @click="navigateTo(`/events/${result.id}`)"
            >
              <v-card-item>
                <v-card-title class="text-h6">{{ result.title }}</v-card-title>
              </v-card-item>
              <v-card-text>
                <div class="d-flex align-center ga-2 mb-2">
                  <v-chip :color="categoryColor(result.category)" size="small" variant="tonal">
                    {{ result.category }}
                  </v-chip>
                  <span class="text-caption text-grey-darken-1">
                    <v-icon size="x-small" icon="mdi-calendar" />
                    {{ formatDate(result.eventDate) }}
                  </span>
                  <span class="text-caption text-grey-darken-1">
                    <v-icon size="x-small" icon="mdi-map-marker" />
                    {{ result.location }}
                  </span>
                </div>
                <p v-if="result.aiSummary" class="text-body-2 text-grey-darken-2">
                  {{ result.aiSummary }}
                </p>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </template>

      <template v-else>
        <h2 class="text-h5 mb-4">Upcoming Events</h2>
        <div v-if="eventsLoading" class="d-flex justify-center py-8">
          <v-progress-circular indeterminate color="primary" />
        </div>
        <v-row v-else>
          <v-col
            v-for="event in events"
            :key="event.id"
            cols="12" sm="6" lg="4"
          >
            <EventCard :event="event" @toggle-bookmark="toggleBookmark" />
          </v-col>
        </v-row>
      </template>
    </v-container>
  </div>
</template>

<script setup lang="ts">
const search = useSearch()
const { getEvents } = useEvents()
const { addBookmark, removeBookmark } = useBookmarks()

const hasSearched = ref(false)
const events = ref<any[]>([])
const eventsLoading = ref(true)

async function onSearch() {
  hasSearched.value = true
  await search.search()
}

async function onImageSearch(file: File) {
  hasSearched.value = true
  await search.searchByImage(file)
}

function onClear() {
  hasSearched.value = false
  search.results.value = []
  search.imageFile.value = null
}

async function toggleBookmark(eventId: number) {
  const event = events.value.find(e => e.id === eventId)
  if (!event) return
  try {
    if (event.isBookmarked) {
      await removeBookmark(eventId)
    } else {
      await addBookmark(eventId)
    }
    event.isBookmarked = !event.isBookmarked
  } catch (e) {
    console.error('Bookmark toggle failed:', e)
  }
}

const categoryColors: Record<string, string> = {
  Education: 'blue', Housing: 'orange', PublicSafety: 'red', Environment: 'green'
}
function categoryColor(cat: string) { return categoryColors[cat] || 'grey' }
function formatDate(date: string) {
  return new Date(date).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
}

onMounted(async () => {
  try {
    events.value = await getEvents()
  } catch (e) {
    console.error('Failed to load events:', e)
  } finally {
    eventsLoading.value = false
  }
})
</script>

<style scoped>
.search-result-card {
  cursor: pointer;
  transition: transform 0.15s, box-shadow 0.15s;
}
.search-result-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15) !important;
}
</style>
