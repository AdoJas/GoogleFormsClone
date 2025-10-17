<template>
  <v-app>
    <v-main class="bg-gray-50">
      <v-container class="pa-6">
        <slot />
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import Navbar from '../components/Navbar.vue'
</script>