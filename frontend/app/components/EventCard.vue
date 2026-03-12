<template>
  <v-card class="event-card" elevation="2" rounded="lg" @click="navigateTo(`/events/${event.id}`)">
    <v-card-item>
      <v-card-title class="text-h6">{{ event.title }}</v-card-title>
      <template #append>
        <v-btn
          v-if="isAuthenticated"
          :icon="event.isBookmarked ? 'mdi-bookmark' : 'mdi-bookmark-outline'"
          :color="event.isBookmarked ? 'primary' : ''"
          variant="text"
          size="small"
          @click.stop="$emit('toggleBookmark', event.id)"
        />
      </template>
    </v-card-item>

    <v-card-text>
      <div class="d-flex align-center ga-2 mb-2">
        <v-chip :color="categoryColor" size="small" variant="tonal">
          {{ event.category }}
        </v-chip>
        <span class="text-caption text-grey-darken-1">
          <v-icon size="x-small" icon="mdi-calendar" /> {{ formattedDate }}
        </span>
        <span class="text-caption text-grey-darken-1">
          <v-icon size="x-small" icon="mdi-map-marker" /> {{ event.location }}
        </span>
      </div>

      <p v-if="event.aiSummary" class="text-body-2 text-grey-darken-2">
        {{ event.aiSummary }}
      </p>
      <p v-else class="text-body-2 text-grey-darken-2">
        {{ truncatedDescription }}
      </p>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
const props = defineProps<{
  event: {
    id: number
    title: string
    description: string
    eventDate: string
    time: string
    location: string
    category: string
    aiSummary: string | null
    isBookmarked: boolean
  }
}>()

defineEmits<{ toggleBookmark: [id: number] }>()

const { isAuthenticated } = useAuth()

const formattedDate = computed(() => {
  return new Date(props.event.eventDate).toLocaleDateString('en-US', {
    month: 'short', day: 'numeric', year: 'numeric'
  })
})

const truncatedDescription = computed(() => {
  const desc = props.event.description
  return desc.length > 150 ? desc.slice(0, 150) + '...' : desc
})

const categoryColors: Record<string, string> = {
  Education: 'blue',
  Housing: 'orange',
  PublicSafety: 'red',
  Environment: 'green',
}

const categoryColor = computed(() => categoryColors[props.event.category] || 'grey')
</script>

<style scoped>
.event-card {
  cursor: pointer;
  transition: transform 0.15s, box-shadow 0.15s;
}
.event-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15) !important;
}
</style>
