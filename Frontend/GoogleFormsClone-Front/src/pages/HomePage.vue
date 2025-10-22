<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useTheme } from 'vuetify'

const router = useRouter()
const theme = useTheme()
const isDark = ref(false)

onMounted(() => {
  const savedTheme = localStorage.getItem('theme')
  isDark.value = savedTheme === 'dark'
  theme.global.name.value = isDark.value ? 'dark' : 'light'
})

const toggleTheme = () => {
  isDark.value = !isDark.value
  theme.global.name.value = isDark.value ? 'dark' : 'light'
  localStorage.setItem('theme', isDark.value ? 'dark' : 'light')
}
</script>

<template>
  <v-main class="d-flex align-center justify-center" style="min-height: 80vh;">
    <div class="text-center pa-12">
      <h1 class="text-h3 font-bold mb-6">Welcome to FormBuilder</h1>
      <RouterLink to="/register">
        <v-btn color="primary" class="mx-2">Get Started</v-btn>
      </RouterLink>
      <RouterLink to="/login">
        <v-btn color="primary" outlined class="mx-2">Login</v-btn>
      </RouterLink>
    </div>
  </v-main>
</template>
