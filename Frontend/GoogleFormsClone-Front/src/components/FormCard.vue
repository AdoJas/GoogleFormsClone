<template>
  <v-card elevation="4" class="pa-6 rounded-xl hover:shadow-lg transition-shadow">
    <div class="vf-flex vf-flex-col gap-2">
      <h2 class="text-h6 font-semibold">{{ form.title }}</h2>
      <p class="text-gray-500 text-sm">Created at: {{ formatDate(form.createdAt) }}</p>
      <p class="text-gray-500 text-sm">Responses: {{ form.responseCount }}</p>
    </div>

    <v-card-actions class="vf-flex vf-justify-end gap-2 mt-4">
      <v-btn
          color="error"
          variant="text"
          @click="$emit('delete', form.id)"
          rounded
      >
        Delete
      </v-btn>
      <v-btn
          color="primary"
          variant="elevated"
          rounded
          @click="$router.push(`/form-builder/${form.id}`)"
      >
        Edit
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script setup lang="ts">
import { defineProps } from 'vue'

interface Form {
  id: string
  title: string
  createdAt: string
  responseCount: number
}

const props = defineProps<{
  form: Form
}>()

const formatDate = (dateStr: string) => new Date(dateStr).toLocaleDateString()
</script>

<style scoped>
.v-card:hover {
  transform: translateY(-2px);
  transition: transform 0.2s ease;
}
</style>
