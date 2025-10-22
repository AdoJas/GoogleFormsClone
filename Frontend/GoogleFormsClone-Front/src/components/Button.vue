<template>
  <button
      :type="type"
      @click="$emit('click')"
      class="w-full py-3 rounded-lg font-semibold text-lg transition"
      style="all: unset; display: inline-block; width: 100%; text-align: center; cursor: pointer; border-radius: 0.5rem; background-color: #2563eb; color: white;"
  >
    <slot />
  </button>
</template>

<script setup lang="ts">
defineProps({ type: { type: String, default: 'button' } })
</script>