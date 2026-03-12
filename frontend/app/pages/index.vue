<template>
  <div>
    <!-- Hero Section -->
    <div class="hero-section">
      <v-container class="py-16 position-relative" style="z-index: 1">
        <h1 class="hero-headline text-white animate-fade-in-down mb-3">
          Discover civic events<br />that matter to you
        </h1>
        <p class="hero-subtitle text-white animate-fade-in-up stagger-2 mb-8">
          Find city council meetings, town halls, and community events — explained in plain language with AI.
        </p>

        <div class="animate-fade-in-up stagger-3" style="max-width: 700px">
          <SearchBar
            v-model="search.query.value"
            v-model:image-file="search.imageFile.value"
            @search="onSearch"
            @clear="onClear"
            @image-search="onImageSearch"
          />
        </div>

        <div class="mt-5 animate-fade-in-up stagger-4">
          <CategoryFilter v-model="search.category.value" />
        </div>
      </v-container>
      <div class="hero-wave" />
    </div>

    <!-- Stats Bar -->
    <v-container class="mt-n4 mb-6">
      <v-row justify="center">
        <v-col cols="auto">
          <v-chip variant="tonal" color="primary" class="mx-1 animate-fade-in-up stagger-1" prepend-icon="mdi-calendar-check">
            {{ events.length }} Active Events
          </v-chip>
        </v-col>
        <v-col cols="auto">
          <v-chip variant="tonal" color="secondary" class="mx-1 animate-fade-in-up stagger-2" prepend-icon="mdi-shape">
            4 Categories
          </v-chip>
        </v-col>
        <v-col cols="auto">
          <v-chip variant="tonal" color="accent" class="mx-1 animate-fade-in-up stagger-3" prepend-icon="mdi-robot">
            AI-Powered Summaries
          </v-chip>
        </v-col>
      </v-row>
    </v-container>

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
          <v-card variant="tonal" color="grey-lighten-3" rounded="xl" class="d-inline-block pa-8 animate-scale-in">
            <v-icon size="64" color="grey-lighten-1" icon="mdi-magnify" />
            <p class="text-h6 text-grey-darken-1 mt-3">No results found</p>
            <p class="text-body-2 text-grey mt-1">Try a different search or upload an event poster</p>
          </v-card>
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
  Education: '#1976D2', Housing: '#FF9800', PublicSafety: '#F44336', Environment: '#4CAF50'
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
  background: linear-gradient(135deg, #0D47A1 0%, #1565C0 40%, #1976D2 70%, #1E88A8 100%);
  position: relative;
  overflow: hidden;
}

.hero-section::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -20%;
  width: 500px;
  height: 500px;
  border-radius: 50%;
  background: rgba(38, 166, 154, 0.1);
  pointer-events: none;
}

.hero-section::after {
  content: '';
  position: absolute;
  bottom: -50%;
  left: -10%;
  width: 400px;
  height: 400px;
  border-radius: 50%;
  background: rgba(255, 112, 67, 0.06);
  pointer-events: none;
}

.hero-wave {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 40px;
  background: #FAFAFA;
  clip-path: ellipse(55% 100% at 50% 100%);
}
</style>
