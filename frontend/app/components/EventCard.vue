<template>
  <v-card
    class="event-card hover-lift"
    elevation="2"
    rounded="lg"
    :style="{ borderTop: `3px solid ${categoryColor}` }"
    :class="[animationClass]"
    @click="navigateTo(`/events/${event.id}`)"
  >
    <v-card-item>
      <v-card-title class="text-h6 card-title">{{ event.title }}</v-card-title>
      <template #append>
        <v-btn
          v-if="isAuthenticated"
          :icon="event.isBookmarked ? 'mdi-bookmark' : 'mdi-bookmark-outline'"
          :color="event.isBookmarked ? 'primary' : ''"
          variant="text"
          size="small"
          class="bookmark-btn"
          @click.stop="$emit('toggleBookmark', event.id)"
        />
      </template>
    </v-card-item>

    <v-card-text>
      <div class="d-flex align-center flex-wrap ga-2 mb-3">
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

      <v-divider class="mb-3" />

      <p v-if="event.aiSummary" class="text-body-2 text-grey-darken-2">
        <v-icon size="x-small" icon="mdi-robot" class="mr-1 text-primary" />
        {{ event.aiSummary }}
      </p>
      <p v-else class="text-body-2 text-grey-darken-2">
        {{ truncatedDescription }}
      </p>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
const props = withDefaults(defineProps<{
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
  index?: number
}>(), {
  index: 0
})

defineEmits<{ toggleBookmark: [id: number] }>()

const { isAuthenticated } = useAuth()

const animationClass = computed(() => {
  const stagger = (props.index % 9) + 1
  return `animate-fade-in-up stagger-${stagger}`
})

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
}

.card-title {
  font-weight: 700;
  letter-spacing: -0.01em;
}

.bookmark-btn {
  transition: transform 0.2s ease;
}

.bookmark-btn:active {
  transform: scale(1.2);
}
</style>
