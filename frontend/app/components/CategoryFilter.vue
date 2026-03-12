<template>
  <div class="d-flex flex-wrap ga-2">
    <v-chip
      v-for="cat in categories"
      :key="cat.value"
      :color="modelValue === cat.value ? 'primary' : 'default'"
      :variant="modelValue === cat.value ? 'flat' : 'outlined'"
      @click="toggle(cat.value)"
    >
      <v-icon start :icon="cat.icon" size="small" />
      {{ cat.label }}
    </v-chip>
  </div>
</template>

<script setup lang="ts">
const modelValue = defineModel<string | null>({ default: null })

const categories = [
  { value: 'Education', label: 'Education', icon: 'mdi-school' },
  { value: 'Housing', label: 'Housing', icon: 'mdi-home' },
  { value: 'PublicSafety', label: 'Public Safety', icon: 'mdi-shield' },
  { value: 'Environment', label: 'Environment', icon: 'mdi-leaf' },
]

function toggle(value: string) {
  modelValue.value = modelValue.value === value ? null : value
}
</script>
