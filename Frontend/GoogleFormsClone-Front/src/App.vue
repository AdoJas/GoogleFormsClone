<template>
  <v-app>
    <Navbar app />

    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import Navbar from './components/Navbar.vue'
import { useUser } from './composables/useUser'
import { useAuth } from './composables/useAuth'

const { fetchMe } = useUser()
const { isLoggedIn } = useAuth()

onMounted(() => {
  if (isLoggedIn.value) {
    fetchMe()
  }
})
</script>
