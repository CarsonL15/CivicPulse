<template>
  <div>
    <v-sheet elevation="4" rounded="xl" class="search-wrapper pa-1 d-flex align-center">
      <v-text-field
        v-model="modelValue"
        placeholder="Search for civic events... (e.g., 'affordable housing', 'school budget')"
        prepend-inner-icon="mdi-magnify"
        variant="solo"
        density="comfortable"
        clearable
        hide-details
        flat
        class="search-field"
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
                color="grey-darken-1"
                @click="openFilePicker"
              />
            </template>
          </v-tooltip>
        </template>
      </v-text-field>

      <v-btn
        color="primary"
        icon="mdi-magnify"
        size="large"
        rounded="xl"
        class="ml-1 mr-1 search-btn"
        @click="$emit('search')"
      >
        <v-icon>mdi-magnify</v-icon>
        <v-tooltip activator="parent" location="bottom">Search</v-tooltip>
      </v-btn>
    </v-sheet>

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
.search-wrapper {
  background: white;
  transition: box-shadow 0.3s ease;
}

.search-wrapper:focus-within {
  box-shadow: 0 4px 20px rgba(21, 101, 192, 0.2) !important;
}

.search-field :deep(.v-field__input) {
  font-size: 1.05rem;
}

.search-btn {
  transition: transform 0.15s ease;
}

.search-btn:hover {
  transform: scale(1.05);
}
</style>
