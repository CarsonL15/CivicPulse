<template>
  <div>
    <!-- Loading -->
    <div v-if="loading">
      <v-sheet color="grey-lighten-3" height="160" />
      <v-container class="py-6">
        <v-row>
          <v-col cols="12" md="8">
            <v-skeleton-loader type="heading" class="mb-4" />
            <v-skeleton-loader type="chip, chip, chip" class="mb-4" />
            <v-skeleton-loader type="card" class="mb-4" />
            <v-skeleton-loader type="paragraph" />
          </v-col>
          <v-col cols="12" md="4">
            <v-skeleton-loader type="card" />
          </v-col>
        </v-row>
      </v-container>
    </div>

    <!-- Event Content -->
    <template v-else-if="event">
      <!-- Hero Header -->
      <div class="event-header" :style="{ background: headerGradient }">
        <v-container class="position-relative" style="z-index: 1">
          <v-breadcrumbs class="px-0 mb-2" :items="breadcrumbs" color="white">
            <template #divider>
              <v-icon icon="mdi-chevron-right" size="small" color="white" />
            </template>
          </v-breadcrumbs>
          <h1 class="text-h4 text-white font-weight-bold animate-fade-in-down">{{ event.title }}</h1>
        </v-container>
      </div>

      <v-container class="py-6">
        <v-row>
          <v-col cols="12" md="8">
            <!-- Metadata Pills -->
            <v-sheet rounded="lg" color="grey-lighten-4" class="pa-4 d-flex flex-wrap ga-3 mb-6 animate-fade-in-up">
              <v-chip variant="tonal" :color="categoryColor" prepend-icon="mdi-tag">{{ event.category }}</v-chip>
              <v-chip variant="text" prepend-icon="mdi-calendar">{{ formattedDate }}</v-chip>
              <v-chip variant="text" prepend-icon="mdi-clock-outline">{{ event.time }}</v-chip>
              <v-chip variant="text" prepend-icon="mdi-map-marker">{{ event.location }}</v-chip>
            </v-sheet>

            <!-- AI Summary -->
            <AiSummary
              v-if="event.aiSummary"
              :summary="event.aiSummary"
              :why-it-matters="event.whyItMatters"
            />

            <!-- Full Description -->
            <v-card variant="outlined" rounded="lg" class="mb-6 animate-fade-in-up stagger-3">
              <v-card-item>
                <v-card-title class="text-subtitle-1 font-weight-bold d-flex align-center ga-2">
                  <v-icon icon="mdi-text" size="small" color="primary" />
                  Full Description
                </v-card-title>
              </v-card-item>
              <v-card-text class="text-body-1" style="white-space: pre-wrap">{{ event.description }}</v-card-text>
            </v-card>

            <!-- Organizer -->
            <v-card variant="tonal" color="grey-lighten-3" rounded="lg" class="animate-fade-in-up stagger-4">
              <v-card-text class="d-flex align-center ga-3">
                <v-avatar color="primary" size="40">
                  <v-icon icon="mdi-account" color="white" />
                </v-avatar>
                <div>
                  <p class="text-body-2 font-weight-bold">{{ event.organizerName }}</p>
                  <p class="text-caption text-grey">Event Organizer</p>
                </div>
              </v-card-text>
            </v-card>
          </v-col>

          <!-- Sidebar -->
          <v-col cols="12" md="4">
            <v-card rounded="lg" elevation="3" class="sidebar-card animate-fade-in-right">
              <v-card-text class="pa-5">
                <!-- Date Display -->
                <div class="text-center mb-4">
                  <p class="text-h3 font-weight-bold text-primary">{{ eventDay }}</p>
                  <p class="text-overline text-grey-darken-1">{{ eventMonth }} {{ eventYear }}</p>
                  <p class="text-body-2 text-grey-darken-1 mt-1">
                    <v-icon size="small" icon="mdi-clock-outline" /> {{ event.time }}
                  </p>
                </div>

                <v-divider class="mb-4" />

                <v-btn
                  v-if="isAuthenticated"
                  :color="event.isBookmarked ? 'primary' : 'grey'"
                  :variant="event.isBookmarked ? 'flat' : 'outlined'"
                  block
                  size="large"
                  rounded="lg"
                  class="bookmark-action-btn"
                  @click="toggleBookmark"
                >
                  <v-icon start :icon="event.isBookmarked ? 'mdi-bookmark' : 'mdi-bookmark-outline'" />
                  {{ event.isBookmarked ? 'Bookmarked' : 'Save Event' }}
                </v-btn>
                <v-btn v-else to="/login" color="primary" variant="outlined" block size="large" rounded="lg">
                  <v-icon start icon="mdi-login" />
                  Login to Save Event
                </v-btn>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </template>

    <!-- Not Found -->
    <v-container v-else class="text-center py-16">
      <v-icon size="80" color="grey-lighten-1" icon="mdi-calendar-remove" />
      <p class="text-h6 text-grey mt-4">Event not found</p>
      <v-btn to="/" color="primary" variant="elevated" rounded="lg" class="mt-4" prepend-icon="mdi-arrow-left">
        Back to Events
      </v-btn>
    </v-container>
  </div>
</template>

<script setup lang="ts">
const route = useRoute()
const { getEvent } = useEvents()
const { addBookmark, removeBookmark } = useBookmarks()
const { isAuthenticated } = useAuth()

const event = ref<any>(null)
const loading = ref(true)

const formattedDate = computed(() => {
  if (!event.value) return ''
  return new Date(event.value.eventDate).toLocaleDateString('en-US', {
    weekday: 'long', month: 'long', day: 'numeric', year: 'numeric'
  })
})

const eventDay = computed(() => {
  if (!event.value) return ''
  return new Date(event.value.eventDate).getDate()
})

const eventMonth = computed(() => {
  if (!event.value) return ''
  return new Date(event.value.eventDate).toLocaleDateString('en-US', { month: 'short' })
})

const eventYear = computed(() => {
  if (!event.value) return ''
  return new Date(event.value.eventDate).getFullYear()
})

const breadcrumbs = computed(() => [
  { title: 'Home', to: '/' },
  { title: 'Events', disabled: true },
  { title: event.value?.title || '', disabled: true },
])

const categoryColors: Record<string, string> = {
  Education: 'blue', Housing: 'orange', PublicSafety: 'red', Environment: 'green'
}
const categoryColor = computed(() => categoryColors[event.value?.category] || 'grey')

const categoryGradients: Record<string, string> = {
  Education: 'linear-gradient(135deg, #0D47A1, #1976D2)',
  Housing: 'linear-gradient(135deg, #E65100, #FF9800)',
  PublicSafety: 'linear-gradient(135deg, #B71C1C, #F44336)',
  Environment: 'linear-gradient(135deg, #1B5E20, #4CAF50)',
}
const headerGradient = computed(() => categoryGradients[event.value?.category] || 'linear-gradient(135deg, #0D47A1, #1976D2)')

async function toggleBookmark() {
  if (!event.value) return
  try {
    if (event.value.isBookmarked) {
      await removeBookmark(event.value.id)
    } else {
      await addBookmark(event.value.id)
    }
    event.value.isBookmarked = !event.value.isBookmarked
  } catch (e) {
    console.error('Bookmark toggle failed:', e)
  }
}

onMounted(async () => {
  try {
    const id = Number(route.params.id)
    event.value = await getEvent(id)
  } catch (e) {
    console.error('Failed to load event:', e)
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.event-header {
  padding: 2rem 0;
  position: relative;
  overflow: hidden;
}

.event-header::before {
  content: '';
  position: absolute;
  top: -40%;
  right: -15%;
  width: 300px;
  height: 300px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.08);
}

.sidebar-card {
  position: sticky;
  top: 80px;
}

.bookmark-action-btn {
  text-transform: none;
  font-weight: 600;
  letter-spacing: 0;
}
</style>
