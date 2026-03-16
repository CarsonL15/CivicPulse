<template>
  <v-container class="py-8" style="max-width: 700px">
    <div class="mb-8 animate-fade-in-down">
      <h1 class="page-title">Create Event</h1>
      <p class="text-body-1 text-grey-darken-1 mt-1">
        Share a civic event with your community. AI will generate a plain-language summary automatically.
      </p>
      <hr class="gradient-divider mt-3" style="width: 60px" />
    </div>

    <v-alert v-if="error" type="error" class="mb-6" closable rounded="lg" @click:close="error = ''">
      {{ error }}
    </v-alert>

    <!-- Success Card -->
    <v-card v-if="success" rounded="xl" elevation="0" class="mb-6 animate-scale-in" color="success" variant="tonal">
      <v-card-text class="pa-6">
        <div class="d-flex align-center ga-3 mb-3">
          <v-icon icon="mdi-check-circle" size="48" color="success" />
          <div>
            <p class="text-h6 font-weight-bold">Event Created!</p>
            <p class="text-body-2 text-grey-darken-1">{{ createdEvent?.title }}</p>
          </div>
        </div>

        <v-card v-if="createdEvent?.aiSummary" variant="outlined" rounded="lg" class="mt-3">
          <v-card-text>
            <p class="text-caption text-grey font-weight-bold mb-1">
              <v-icon icon="mdi-robot" size="x-small" /> AI Summary
            </p>
            <p class="text-body-2">{{ createdEvent.aiSummary }}</p>
            <template v-if="createdEvent?.whyItMatters">
              <p class="text-caption text-grey font-weight-bold mt-3 mb-1">
                <v-icon icon="mdi-lightbulb-on" size="x-small" /> Why It Matters
              </p>
              <p class="text-body-2">{{ createdEvent.whyItMatters }}</p>
            </template>
          </v-card-text>
        </v-card>

        <v-btn :to="`/events/${createdEvent?.id}`" color="primary" variant="elevated" rounded="lg" class="mt-4" prepend-icon="mdi-eye">
          View Event
        </v-btn>
      </v-card-text>
    </v-card>

    <EventForm
      v-model:title="title"
      v-model:description="description"
      v-model:event-date="eventDate"
      v-model:time="time"
      v-model:location="location"
      v-model:category="category"
      :loading="submitting"
      submit-label="Create Event"
      @submit="handleSubmit"
    />
  </v-container>
</template>

<script setup lang="ts">
definePageMeta({ middleware: ['auth'] })

const { createEvent } = useEvents()

const title = ref('')
const description = ref('')
const eventDate = ref('')
const time = ref('')
const location = ref('')
const category = ref<number | null>(null)
const submitting = ref(false)
const error = ref('')
const success = ref(false)
const createdEvent = ref<any>(null)

async function handleSubmit() {
  if (!title.value || !description.value || !eventDate.value || !time.value || !location.value || category.value === null) {
    error.value = 'Please fill in all fields'
    return
  }

  submitting.value = true
  error.value = ''
  success.value = false

  try {
    createdEvent.value = await createEvent({
      title: title.value,
      description: description.value,
      eventDate: eventDate.value,
      time: time.value,
      location: location.value,
      category: category.value
    })
    success.value = true
    title.value = ''
    description.value = ''
    eventDate.value = ''
    time.value = ''
    location.value = ''
    category.value = null
  } catch (e: any) {
    error.value = e.message || 'Failed to create event'
  } finally {
    submitting.value = false
  }
}
</script>
