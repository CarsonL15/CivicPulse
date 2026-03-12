<template>
  <div>
    <div v-if="loading" class="py-4">
      <v-row>
        <v-col v-for="n in 6" :key="n" cols="12" sm="6" lg="4">
          <v-skeleton-loader type="card" elevation="2" rounded="lg" />
        </v-col>
      </v-row>
    </div>

    <div v-else-if="events.length === 0" class="text-center py-12">
      <v-card variant="tonal" color="grey-lighten-3" rounded="xl" class="d-inline-block pa-8 animate-scale-in">
        <v-icon size="64" color="grey-lighten-1" icon="mdi-calendar-blank" />
        <p class="text-h6 text-grey-darken-1 mt-3">{{ emptyMessage }}</p>
        <p class="text-body-2 text-grey mt-1">Check back later for new events</p>
      </v-card>
    </div>

    <v-row v-else>
      <v-col
        v-for="(event, index) in events"
        :key="event.id"
        cols="12"
        sm="6"
        lg="4"
      >
        <EventCard :event="event" :index="index" @toggle-bookmark="$emit('toggleBookmark', $event)" />
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
