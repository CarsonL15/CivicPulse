<template>
  <div>
    <div v-if="loading" class="d-flex justify-center py-8">
      <v-progress-circular indeterminate color="primary" />
    </div>

    <div v-else-if="events.length === 0" class="text-center py-8">
      <v-icon size="64" color="grey-lighten-1" icon="mdi-calendar-blank" />
      <p class="text-h6 text-grey mt-2">{{ emptyMessage }}</p>
    </div>

    <v-row v-else>
      <v-col
        v-for="event in events"
        :key="event.id"
        cols="12"
        sm="6"
        lg="4"
      >
        <EventCard :event="event" @toggle-bookmark="$emit('toggleBookmark', $event)" />
      </v-col>
    </v-row>
  </div>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  events: any[]
  loading?: boolean
  emptyMessage?: string
}>(), {
  loading: false,
  emptyMessage: 'No events found'
})

defineEmits<{ toggleBookmark: [id: number] }>()
</script>
