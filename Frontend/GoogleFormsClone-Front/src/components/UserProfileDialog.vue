<template>
  <v-menu v-model="menu" :close-on-content-click="false" location="bottom end">
    <template #activator="{ props }">
      <v-avatar v-bind="props" size="48" class="cursor-pointer">
        <v-img :src="avatarUrl" alt="User Avatar" class="avatar-img" />
      </v-avatar>
    </template>

    <v-card min-width="300">
      <!-- User info -->
      <v-list>
        <v-list-item
            :prepend-avatar="avatarUrl"
            :title="currentUser.value?.name || 'User'"
            :subtitle="currentUser.value?.email || ''"
        >
          <template #append>
            <v-btn
                :class="fav ? 'text-red' : ''"
                icon="mdi-heart"
                variant="text"
                @click="fav = !fav"
            />
          </template>
        </v-list-item>
      </v-list>

      <v-divider />

      <!-- Theme toggle -->
      <v-list>
        <v-list-item>
          <v-switch
              v-model="isDarkTheme"
              color="purple"
              :label="`Dark theme: ${isDarkTheme ? 'On' : 'Off'}`"
              hide-details
              @update:modelValue="toggleTheme"
          />
        </v-list-item>
      </v-list>

      <v-divider />

      <!-- Logout / Close -->
      <v-card-actions>
        <v-spacer />
        <v-btn text @click="menu = false">Close</v-btn>
        <v-btn color="error" variant="text" @click="handleLogout">Logout</v-btn>
      </v-card-actions>
    </v-card>
  </v-menu>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useUser } from '@/composables/useUser'
import { useAuth } from '@/composables/useAuth'
import { useTheme } from 'vuetify'
import defaultAvatar from '@/assets/default-avatar.png'

const { currentUser, updateMe } = useUser()
const { logout } = useAuth()
const theme = useTheme()

const menu = ref(false)
const fav = ref(true)

const isDarkTheme = ref(currentUser.value?.preferences.theme === 'dark')

// Watch backend user preferences and sync Vuetify theme
watch(
    () => currentUser.value?.preferences.theme,
    (newTheme) => {
      if (newTheme) {
        isDarkTheme.value = newTheme === 'dark'
        theme.global.name.value = newTheme
      }
    },
    { immediate: true }
)

const avatarUrl = computed(() =>
    currentUser.value?.avatarUrl ? `/api/files/${currentUser.value.avatarUrl}` : defaultAvatar
)

// Toggle theme and persist to backend
const toggleTheme = async (value: boolean) => {
  if (!currentUser.value) return

  const newTheme = value ? 'dark' : 'light'
  theme.global.name.value = newTheme

  await updateMe({
    preferences: {
      ...currentUser.value.preferences,
      theme: newTheme,
    },
  })
}

const handleLogout = async () => {
  await logout()
  menu.value = false
}
</script>

<style scoped>
.avatar-img img {
  pointer-events: none;
}
</style>
