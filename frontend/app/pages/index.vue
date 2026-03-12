<template>
  <div>
    <!-- Hero Section -->
    <div class="hero-section">
      <v-container class="pt-14 pb-16 position-relative" style="z-index: 1">
        <h1 class="hero-headline text-white animate-fade-in-down mb-2">
          Discover civic events<br />that matter to you
        </h1>
        <p class="hero-subtitle text-white animate-fade-in-up stagger-2 mb-8" style="max-width: 560px">
          City council meetings, town halls, and community events — explained in plain language with AI.
        </p>

        <div class="animate-fade-in-up stagger-3" style="max-width: 600px">
          <SearchBar
            v-model="search.query.value"
            v-model:image-file="search.imageFile.value"
            @search="onSearch"
            @clear="onClear"
            @image-search="onImageSearch"
          />
        </div>

        <div class="mt-4 animate-fade-in-up stagger-4">
          <CategoryFilter v-model="search.category.value" />
        </div>
      </v-container>
    </div>

    <!-- Results / Events -->
    <v-container>
      <template v-if="hasSearched">
        <h2 class="section-heading mb-6">
          <v-icon v-if="search.imageFile.value" icon="mdi-image-search" class="mr-1" />
          {{ search.imageFile.value ? 'Image Search Results' : 'Search Results' }}
          <v-chip v-if="search.results.value.length" size="small" class="ml-2" color="primary" variant="tonal">
            {{ search.results.value.length }}
          </v-chip>
        </h2>

        <div v-if="search.loading.value" class="py-4">
          <v-row>
            <v-col v-for="n in 3" :key="n" cols="12" sm="6" lg="4">
              <v-skeleton-loader type="card" elevation="2" rounded="lg" />
            </v-col>
          </v-row>
        </div>

        <div v-else-if="search.results.value.length === 0" class="text-center py-12">
          <v-icon size="48" color="grey-lighten-1" icon="mdi-magnify" class="mb-3" />
          <p class="text-h6 text-grey-darken-1">No results found</p>
          <p class="text-body-2 text-grey mt-1">Try a different search or upload an event poster</p>
        </div>

        <v-row v-else>
          <v-col
            v-for="(result, index) in search.results.value"
            :key="result.id"
            cols="12" sm="6" lg="4"
          >
            <v-card
              elevation="2"
              rounded="lg"
              class="hover-lift"
              :class="['animate-fade-in-up', `stagger-${(index % 9) + 1}`]"
              :style="{ borderTop: `3px solid ${categoryColorHex(result.category)}` }"
              @click="navigateTo(`/events/${result.id}`)"
              style="cursor: pointer"
            >
              <v-card-item>
                <v-card-title class="text-h6 font-weight-bold" style="letter-spacing: -0.01em">{{ result.title }}</v-card-title>
              </v-card-item>
              <v-card-text>
                <div class="d-flex align-center flex-wrap ga-2 mb-3">
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
                <v-divider class="mb-3" />
                <p v-if="result.aiSummary" class="text-body-2 text-grey-darken-2">
                  <v-icon size="x-small" icon="mdi-robot" class="mr-1 text-primary" />
                  {{ result.aiSummary }}
                </p>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </template>

      <template v-else>
        <h2 class="section-heading mb-6">Upcoming Events</h2>

        <div v-if="eventsLoading" class="py-4">
          <v-row>
            <v-col v-for="n in 6" :key="n" cols="12" sm="6" lg="4">
              <v-skeleton-loader type="card" elevation="2" rounded="lg" />
            </v-col>
          </v-row>
        </div>

        <v-row v-else>
          <v-col
            v-for="(event, index) in events"
            :key="event.id"
            cols="12" sm="6" lg="4"
          >
            <EventCard :event="event" :index="index" @toggle-bookmark="toggleBookmark" />
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

useScrollAnimation()

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
const categoryColorHexMap: Record<string, string> = {
  Education: '#1a4a6e', Housing: '#FF9800', PublicSafety: '#F44336', Environment: '#4CAF50'
}
function categoryColor(cat: string) { return categoryColors[cat] || 'grey' }
function categoryColorHex(cat: string) { return categoryColorHexMap[cat] || '#9E9E9E' }
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
.hero-section {
  background: linear-gradient(135deg, #0f2b3e 0%, #1a4a6e 100%);
}
</style>
