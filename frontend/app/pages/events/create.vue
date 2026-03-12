<template>
  <v-container class="py-6" style="max-width: 700px">
    <h1 class="text-h4 font-weight-bold mb-6">Create Event</h1>

    <v-alert v-if="error" type="error" class="mb-4" closable @click:close="error = ''">
      {{ error }}
    </v-alert>

    <v-alert v-if="success" type="success" class="mb-4">
      Event created successfully!
      <template v-if="createdEvent?.aiSummary">
        <br /><strong>AI Summary:</strong> {{ createdEvent.aiSummary }}
      </template>
    </v-alert>

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
    // Reset form
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
