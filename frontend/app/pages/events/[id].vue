<template>
  <v-container class="py-6">
    <div v-if="loading" class="d-flex justify-center py-8">
      <v-progress-circular indeterminate color="primary" />
    </div>

    <template v-else-if="event">
      <v-btn variant="text" to="/" prepend-icon="mdi-arrow-left" class="mb-4">Back</v-btn>

      <v-row>
        <v-col cols="12" md="8">
          <h1 class="text-h4 font-weight-bold mb-2">{{ event.title }}</h1>

          <div class="d-flex flex-wrap align-center ga-3 mb-4">
            <v-chip :color="categoryColor" variant="tonal">{{ event.category }}</v-chip>
            <span class="text-body-2 text-grey-darken-1">
              <v-icon size="small" icon="mdi-calendar" /> {{ formattedDate }}
            </span>
            <span class="text-body-2 text-grey-darken-1">
              <v-icon size="small" icon="mdi-clock" /> {{ event.time }}
            </span>
            <span class="text-body-2 text-grey-darken-1">
              <v-icon size="small" icon="mdi-map-marker" /> {{ event.location }}
            </span>
          </div>

          <AiSummary
            v-if="event.aiSummary"
            :summary="event.aiSummary"
            :why-it-matters="event.whyItMatters"
          />

          <v-card variant="outlined" rounded="lg" class="mb-4">
            <v-card-item>
              <v-card-title class="text-subtitle-1 font-weight-bold">Full Description</v-card-title>
            </v-card-item>
            <v-card-text class="text-body-1" style="white-space: pre-wrap">{{ event.description }}</v-card-text>
          </v-card>

          <p class="text-caption text-grey">Organized by {{ event.organizerName }}</p>
        </v-col>

        <v-col cols="12" md="4">
          <v-card rounded="lg" elevation="2">
            <v-card-text class="text-center">
              <v-btn
                v-if="isAuthenticated"
                :color="event.isBookmarked ? 'primary' : 'grey'"
                :variant="event.isBookmarked ? 'flat' : 'outlined'"
                block
                size="large"
                @click="toggleBookmark"
              >
                <v-icon start :icon="event.isBookmarked ? 'mdi-bookmark' : 'mdi-bookmark-outline'" />
                {{ event.isBookmarked ? 'Bookmarked' : 'Save Event' }}
              </v-btn>
              <v-btn v-else to="/login" color="primary" variant="outlined" block size="large">
                Login to Save Event
              </v-btn>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </template>

    <div v-else class="text-center py-8">
      <p class="text-h6 text-grey">Event not found</p>
    </div>
  </v-container>
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
