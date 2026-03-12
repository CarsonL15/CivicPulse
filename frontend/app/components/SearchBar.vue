<template>
  <div>
    <div class="search-wrapper d-flex align-center" style="gap: 8px">
      <div style="flex: 1; min-width: 0">
        <v-text-field
          v-model="modelValue"
          placeholder="Search for civic events..."
          prepend-inner-icon="mdi-magnify"
          variant="outlined"
          density="comfortable"
          rounded="pill"
          clearable
          hide-details
          bg-color="white"
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
      </div>
      <v-btn
        color="white"
        icon="mdi-magnify"
        size="large"
        rounded="circle"
        variant="flat"
        class="search-btn flex-shrink-0"
        @click="$emit('search')"
      >
        <v-icon color="primary">mdi-magnify</v-icon>
        <v-tooltip activator="parent" location="bottom">Search</v-tooltip>
      </v-btn>
    </div>

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

.search-field :deep(.v-field--focused) {
  box-shadow: 0 0 0 2px rgba(255, 255, 255, 0.4);
}

.search-btn {
  transition: transform 0.15s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.search-btn:hover {
  transform: scale(1.05);
}
</style>
