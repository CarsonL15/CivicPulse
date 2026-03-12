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
      <v-container class="py-8" style="max-width: 960px">
        <!-- Back link -->
        <v-btn variant="text" color="grey-darken-1" size="small" to="/" class="mb-4 text-none" prepend-icon="mdi-arrow-left">
          Back to Events
        </v-btn>

        <!-- Title -->
        <h1 class="text-h4 font-weight-bold mb-3">{{ event.title }}</h1>

        <!-- Metadata -->
        <div class="d-flex flex-wrap align-center ga-4 mb-6 text-body-2 text-grey-darken-1">
          <v-chip :color="categoryColor" variant="tonal" size="small">{{ event.category }}</v-chip>
          <span class="d-flex align-center ga-1"><v-icon size="small" icon="mdi-calendar" /> {{ formattedDate }}</span>
          <span class="d-flex align-center ga-1"><v-icon size="small" icon="mdi-clock-outline" /> {{ event.time }}</span>
          <span class="d-flex align-center ga-1"><v-icon size="small" icon="mdi-map-marker" /> {{ event.location }}</span>
        </div>

        <v-row>
          <v-col cols="12" md="8">
            <!-- AI Summary -->
            <AiSummary
              v-if="event.aiSummary"
              :summary="event.aiSummary"
              :why-it-matters="event.whyItMatters"
            />

            <!-- Description -->
            <div class="mb-6">
              <p class="text-overline text-grey mb-2">Description</p>
              <p class="text-body-1" style="white-space: pre-wrap">{{ event.description }}</p>
            </div>

            <v-divider class="mb-4" />

            <!-- Organizer -->
            <div class="d-flex align-center ga-3">
              <v-avatar color="primary" size="36">
                <v-icon icon="mdi-account" size="small" color="white" />
              </v-avatar>
              <div>
                <p class="text-body-2 font-weight-medium">{{ event.organizerName }}</p>
                <p class="text-caption text-grey">Organizer</p>
              </div>
            </div>
          </v-col>

          <!-- Sidebar -->
          <v-col cols="12" md="4">
            <v-card variant="outlined" rounded="lg" class="sidebar-card">
              <v-card-text class="pa-5 text-center">
                <p class="text-h3 font-weight-bold text-primary">{{ eventDay }}</p>
                <p class="text-overline text-grey-darken-1">{{ eventMonth }} {{ eventYear }}</p>
                <p class="text-body-2 text-grey-darken-1 mt-1 mb-4">
                  <v-icon size="small" icon="mdi-clock-outline" /> {{ event.time }}
                </p>

                <v-btn
                  v-if="isAuthenticated"
                  :color="event.isBookmarked ? 'primary' : 'grey-darken-2'"
                  :variant="event.isBookmarked ? 'flat' : 'outlined'"
                  block
                  rounded="lg"
                  class="text-none"
                  @click="toggleBookmark"
                >
                  <v-icon start :icon="event.isBookmarked ? 'mdi-bookmark' : 'mdi-bookmark-outline'" />
                  {{ event.isBookmarked ? 'Bookmarked' : 'Save Event' }}
                </v-btn>
                <v-btn v-else to="/login" color="primary" variant="outlined" block rounded="lg" class="text-none">
                  <v-icon start icon="mdi-login" />
                  Login to Save
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

const categoryColors: Record<string, string> = {
  Education: 'blue', Housing: 'orange', PublicSafety: 'red', Environment: 'green'
}
const categoryColor = computed(() => categoryColors[event.value?.category] || 'grey')

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
.sidebar-card {
  position: sticky;
  top: 80px;
}
</style>
