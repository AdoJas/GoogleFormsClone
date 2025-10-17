<template>
  <aside class="vf-w-64 vf-bg-white vf-shadow-md vf-p-4 vf-flex vf-flex-col">
    <h2 class="vf-text-lg vf-font-semibold vf-mb-6">Forms Menu</h2>

    <RouterLink
        to="/dashboard"
        class="vf-block vf-px-4 vf-py-2 vf-rounded hover:vf-bg-gray-100 vf-mb-2"
        active-class="vf-bg-gray-200"
    >
      Dashboard
    </RouterLink>

    <RouterLink
        to="/form-builder"
        class="vf-block vf-px-4 vf-py-2 vf-rounded hover:vf-bg-gray-100 vf-mb-2"
        active-class="vf-bg-gray-200"
    >
      Create Form
    </RouterLink>

    <RouterLink
        to="/form-responses"
        class="vf-block vf-px-4 vf-py-2 vf-rounded hover:vf-bg-gray-100 vf-mb-2"
        active-class="vf-bg-gray-200"
    >
      Responses
    </RouterLink>

    <RouterLink
        to="/preferences"
        class="vf-block vf-px-4 vf-py-2 vf-rounded hover:vf-bg-gray-100 vf-mt-auto"
        active-class="vf-bg-gray-200"
    >
      Preferences
    </RouterLink>
  </aside>
</template>

<script setup lang="ts">
</script>
