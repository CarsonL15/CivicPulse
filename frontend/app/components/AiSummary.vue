<template>
  <div class="ai-summary mb-6">
    <p class="text-overline text-grey d-flex align-center ga-1 mb-2">
      <v-icon icon="mdi-robot" size="small" /> AI Summary
    </p>
    <p class="text-body-1 mb-3">{{ summary }}</p>
    <div v-if="whyItMatters" class="why-it-matters pa-3 rounded-lg">
      <p class="text-caption font-weight-bold text-grey-darken-2 mb-1 d-flex align-center ga-1">
        <v-icon icon="mdi-lightbulb-on" size="x-small" />
        Why It Matters
      </p>
      <p class="text-body-2">{{ whyItMatters }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  summary: string
  whyItMatters?: string | null
}>()
</script>

<style scoped>
.ai-summary {
  border-left: 3px solid #1a4a6e;
  padding-left: 16px;
}

.why-it-matters {
  background: #f5f5f5;
}
</style>
