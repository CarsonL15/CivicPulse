<template>
  <v-card variant="outlined" rounded="lg" class="ai-summary-card mb-4 animate-fade-in-left">
    <v-card-item>
      <template #prepend>
        <v-icon icon="mdi-robot" color="primary" class="ai-icon" />
      </template>
      <v-card-title class="text-subtitle-1 font-weight-bold d-flex align-center ga-2">
        AI Summary
        <v-chip size="x-small" color="primary" variant="flat">AI-Generated</v-chip>
      </v-card-title>
    </v-card-item>
    <v-card-text>
      <p class="text-body-1 mb-3">{{ summary }}</p>
      <div v-if="whyItMatters" class="why-it-matters mt-3 pa-3 rounded-lg">
        <p class="text-subtitle-2 font-weight-bold mb-1 d-flex align-center ga-1">
          <v-icon icon="mdi-lightbulb-on" size="small" color="accent" />
          Why It Matters
        </p>
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

<style scoped>
.ai-summary-card {
  border-left: 4px solid transparent;
  border-image: linear-gradient(to bottom, #1565C0, #26A69A) 1;
}

.ai-icon {
  animation: fadeInLeft 0.5s ease-out;
}

.why-it-matters {
  background: rgba(255, 112, 67, 0.06);
  border-left: 3px solid #FF7043;
}

@keyframes fadeInLeft {
  from { opacity: 0; transform: translateX(-12px); }
  to { opacity: 1; transform: translateX(0); }
}
</style>
