<template>
  <v-form ref="form" @submit.prevent="$emit('submit')">
    <v-text-field
      v-model="title"
      label="Event Title"
      :rules="[v => !!v || 'Title is required']"
      variant="outlined"
      class="mb-3"
    />

    <v-textarea
      v-model="description"
      label="Description"
      :rules="[v => !!v || 'Description is required']"
      variant="outlined"
      rows="4"
      class="mb-3"
    />

    <v-row>
      <v-col cols="12" sm="6">
        <v-text-field
          v-model="eventDate"
          label="Date"
          type="date"
          :rules="[v => !!v || 'Date is required']"
          variant="outlined"
        />
      </v-col>
      <v-col cols="12" sm="6">
        <v-text-field
          v-model="time"
          label="Time (e.g., 6:00 PM - 8:00 PM)"
          :rules="[v => !!v || 'Time is required']"
          variant="outlined"
        />
      </v-col>
    </v-row>

    <v-text-field
      v-model="location"
      label="Location"
      :rules="[v => !!v || 'Location is required']"
      variant="outlined"
      class="mb-3"
    />

    <v-select
      v-model="category"
      label="Category"
      :items="categories"
      item-title="label"
      item-value="value"
      :rules="[v => v !== null || 'Category is required']"
      variant="outlined"
      class="mb-3"
    />

    <v-btn type="submit" color="primary" size="large" :loading="loading" block>
      {{ submitLabel }}
    </v-btn>
  </v-form>
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
