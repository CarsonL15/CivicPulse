<template>
  <div>
    <v-text-field
      v-model="modelValue"
      placeholder="Search for civic events..."
      prepend-inner-icon="mdi-magnify"
      variant="solo"
      density="comfortable"
      rounded="pill"
      clearable
      hide-details
      flat
      class="search-field"
      @keyup.enter="$emit('search')"
      @click:clear="$emit('clear')"
    >
      <template #append-inner>
        <v-btn
          icon="mdi-image-search"
          variant="text"
          size="small"
          color="grey-darken-1"
          @click="openFilePicker"
        />
        <v-btn
          icon="mdi-magnify"
          variant="flat"
          size="small"
          color="primary"
          class="ml-1"
          @click="$emit('search')"
        />
      </template>
    </v-text-field>

    <v-scale-transition>
      <v-chip
        v-if="imageFile"
        class="mt-3"
        closable
        color="primary"
        variant="tonal"
        @click:close="clearImage"
      >
        <v-icon start icon="mdi-image" />
        {{ imageFile.name }}
      </v-chip>
    </v-scale-transition>

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

<style scoped>
.search-field :deep(.v-field) {
  font-size: 1.05rem;
}
</style>
