<template>
  <v-card variant="tonal" color="primary" rounded="lg" class="mb-4">
    <v-card-item>
      <template #prepend>
        <v-icon icon="mdi-robot" color="primary" />
      </template>
      <v-card-title class="text-subtitle-1 font-weight-bold">AI Summary</v-card-title>
    </v-card-item>
    <v-card-text>
      <p class="text-body-1 mb-3">{{ summary }}</p>
      <div v-if="whyItMatters" class="mt-2">
        <p class="text-subtitle-2 font-weight-bold text-primary mb-1">Why It Matters</p>
        <p class="text-body-2">{{ whyItMatters }}</p>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
defineProps<{
  summary: string
  whyItMatters?: string | null
}>()
</script>
