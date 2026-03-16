<template>
  <v-card rounded="xl" variant="outlined" class="animate-fade-in-up">
    <v-card-text class="pa-6">
      <v-form ref="form" @submit.prevent="$emit('submit')">
        <p class="text-overline text-primary mb-3">Event Details</p>

        <v-text-field
          v-model="title"
          label="Event Title"
          :rules="[v => !!v || 'Title is required']"
          variant="outlined"
          prepend-inner-icon="mdi-format-title"
          class="mb-3"
        />

        <v-textarea
          v-model="description"
          label="Description"
          :rules="[v => !!v || 'Description is required']"
          variant="outlined"
          prepend-inner-icon="mdi-text"
          rows="4"
          class="mb-3"
        />

        <p class="text-overline text-primary mb-3">When & Where</p>

        <v-row>
          <v-col cols="12" sm="6">
            <v-text-field
              v-model="eventDate"
              label="Date"
              type="date"
              :rules="[v => !!v || 'Date is required']"
              variant="outlined"
              prepend-inner-icon="mdi-calendar"
            />
          </v-col>
          <v-col cols="12" sm="6">
            <v-text-field
              v-model="time"
              label="Time (e.g., 6:00 PM - 8:00 PM)"
              :rules="[v => !!v || 'Time is required']"
              variant="outlined"
              prepend-inner-icon="mdi-clock-outline"
            />
          </v-col>
        </v-row>

        <v-text-field
          v-model="location"
          label="Location"
          :rules="[v => !!v || 'Location is required']"
          variant="outlined"
          prepend-inner-icon="mdi-map-marker"
          class="mb-3"
        />

        <p class="text-overline text-primary mb-3">Classification</p>

        <v-select
          v-model="category"
          label="Category"
          :items="categories"
          item-title="label"
          item-value="value"
          :rules="[v => v !== null || 'Category is required']"
          variant="outlined"
          prepend-inner-icon="mdi-tag"
          class="mb-4"
        />

        <v-btn
          type="submit"
          color="primary"
          size="large"
          :loading="loading"
          block
          rounded="lg"
          elevation="0"
          prepend-icon="mdi-send"
          class="submit-btn"
        >
          {{ submitLabel }}
        </v-btn>
      </v-form>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
const title = defineModel<string>('title', { default: '' })
const description = defineModel<string>('description', { default: '' })
const eventDate = defineModel<string>('eventDate', { default: '' })
const time = defineModel<string>('time', { default: '' })
const location = defineModel<string>('location', { default: '' })
const category = defineModel<number | null>('category', { default: null })

withDefaults(defineProps<{
  loading?: boolean
  submitLabel?: string
}>(), {
  loading: false,
  submitLabel: 'Create Event'
})

defineEmits<{ submit: [] }>()

const categories = [
  { value: 0, label: 'Education' },
  { value: 1, label: 'Housing' },
  { value: 2, label: 'Public Safety' },
  { value: 3, label: 'Environment' },
]
</script>

<style scoped>
.submit-btn {
  text-transform: none;
  font-weight: 600;
  font-size: 1rem;
  letter-spacing: 0;
}
</style>
