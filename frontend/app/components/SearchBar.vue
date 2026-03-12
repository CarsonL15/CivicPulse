<template>
  <div>
    <v-text-field
      v-model="modelValue"
      placeholder="Search for civic events... (e.g., 'affordable housing', 'school budget')"
      prepend-inner-icon="mdi-magnify"
      variant="outlined"
      density="comfortable"
      clearable
      hide-details
      bg-color="white"
      @keyup.enter="$emit('search')"
      @click:clear="$emit('clear')"
    >
      <template #append-inner>
        <v-tooltip text="Search by image — upload an event poster" location="bottom">
          <template #activator="{ props }">
            <v-btn
              v-bind="props"
              icon="mdi-image-search"
              variant="text"
              size="small"
              @click="openFilePicker"
            />
          </template>
        </v-tooltip>
      </template>
    </v-text-field>

    <v-chip
      v-if="imageFile"
      class="mt-2"
      closable
      color="primary"
      variant="tonal"
      @click:close="clearImage"
    >
      <v-icon start icon="mdi-image" />
      {{ imageFile.name }}
    </v-chip>

    <input
      ref="fileInput"
      type="file"
      accept="image/jpeg,image/png,image/bmp,image/gif,image/tiff"
      hidden
      @change="onFileSelected"
    >
  </div>
</template>

<script setup lang="ts">
const modelValue = defineModel<string>({ default: '' })
const imageFile = defineModel<File | null>('imageFile', { default: null })

const fileInput = ref<HTMLInputElement | null>(null)

const emit = defineEmits<{
  search: []
  clear: []
  imageSearch: [file: File]
}>()

function openFilePicker() {
  fileInput.value?.click()
}

function onFileSelected(event: Event) {
  const input = event.target as HTMLInputElement
  if (input.files?.length) {
    imageFile.value = input.files[0]
    emit('imageSearch', input.files[0])
  }
}

function clearImage() {
  imageFile.value = null
  if (fileInput.value) fileInput.value.value = ''
}
</script>
