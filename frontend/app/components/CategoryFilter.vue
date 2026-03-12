<template>
  <div class="d-flex align-center flex-wrap ga-2">
    <span class="text-caption font-weight-medium mr-1 filter-label">Filter:</span>
    <v-chip
      v-for="(cat, i) in categories"
      :key="cat.value"
      :color="cat.color"
      :variant="modelValue === cat.value ? 'flat' : 'tonal'"
      :elevation="0"
      :prepend-icon="modelValue === cat.value ? 'mdi-check' : cat.icon"
      :class="['animate-fade-in-up', `stagger-${i + 1}`, 'category-chip']"
      @click="toggle(cat.value)"
    >
      {{ cat.label }}
    </v-chip>
  </div>
</template>

<script setup lang="ts">
const modelValue = defineModel<string | null>({ default: null })

const categories = [
  { value: 'Education', label: 'Education', icon: 'mdi-school', color: 'blue' },
  { value: 'Housing', label: 'Housing', icon: 'mdi-home', color: 'orange' },
  { value: 'PublicSafety', label: 'Public Safety', icon: 'mdi-shield', color: 'red' },
  { value: 'Environment', label: 'Environment', icon: 'mdi-leaf', color: 'green' },
]

function toggle(value: string) {
  modelValue.value = modelValue.value === value ? null : value
}
</script>

<style scoped>
.category-chip {
  cursor: pointer;
  transition: all 0.2s ease;
}

.category-chip:hover {
  transform: translateY(-1px);
}

.filter-label {
  color: rgba(255, 255, 255, 0.8);
}
</style>
