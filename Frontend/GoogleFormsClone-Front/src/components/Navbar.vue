<template>
  <v-app-bar app color="primary" dark>
    <v-toolbar-title class="cursor-pointer" @click="$router.push('/')">
      FormBuilder
    </v-toolbar-title>

    <v-spacer />

    <!-- Guest -->
    <template v-if="!isLoggedIn">
      <v-btn text @click="$router.push('/login')">Login</v-btn>
      <v-btn text @click="$router.push('/register')">Register</v-btn>
    </template>

    <!-- Logged-in -->
    <template v-else>
      <v-btn text @click="$router.push('/dashboard')">Dashboard</v-btn>

      <v-menu v-model="menu" :close-on-content-click="false" placement="bottom-end">
        <template #activator="{ props }">
          <v-avatar v-bind="props" size="48" class="mx-3 cursor-pointer">
            <v-img :src="avatarBlobUrl" alt="User Avatar" />
          </v-avatar>
        </template>

        <v-card min-width="300">
          <!-- User Info -->
          <v-list v-if="currentUser">
            <v-list-item>
              <v-avatar class="me-3" size="40">
                <v-img :src="avatarBlobUrl" />
              </v-avatar>
              <v-list-item-content>
                <v-list-item-title>{{ currentUser.name }}</v-list-item-title>
                <v-list-item-subtitle>{{ currentUser.email }}</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
          </v-list>

          <v-divider />

          <!-- Avatar Upload -->
          <v-list>
            <v-list-item>
              <v-btn small @click="triggerAvatarUpload">Change Avatar</v-btn>
              <input
                  ref="avatarInput"
                  type="file"
                  class="d-none"
                  accept="image/*"
                  @change="handleAvatarSelected"
              />
            </v-list-item>
          </v-list>

          <v-divider />

          <!-- Theme Toggle -->
          <v-list>
            <v-list-item>
              <v-switch v-model="isDarkTheme" color="purple" label="Dark theme" />
            </v-list-item>
          </v-list>

          <v-divider />

          <!-- Actions -->
          <v-card-actions>
            <v-spacer />
            <v-btn text @click="menu = false">Close</v-btn>
            <v-btn color="error" text @click="handleLogout">Logout</v-btn>
          </v-card-actions>
        </v-card>
      </v-menu>
    </template>
  </v-app-bar>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onBeforeUnmount } from 'vue'
import { useRouter } from 'vue-router'
import { useTheme } from 'vuetify'
import defaultAvatar from '../assets/default-avatar.png'
import { useUser } from '../composables/useUser'
import { useAuth } from '../composables/useAuth'
import { fileApi } from '../api/fileApi'

// Router & auth
const router = useRouter()
const { logout } = useAuth()
const { currentUser, updateMe, setUser } = useUser()

// Theme
const theme = useTheme()
theme.global.name.value = localStorage.getItem('theme') || 'light'

// Refs
const menu = ref(false)
const avatarInput = ref<HTMLInputElement | null>(null)
const avatarBlobUrl = ref<string>(defaultAvatar)
let previousBlobUrl: string | null = null

// Computed
const isLoggedIn = computed(() => !!currentUser?.value?.id)
const isDarkTheme = computed({
  get: () => currentUser.value?.preferences?.theme === 'dark' || theme.global.name.value === 'dark',
  set: async (val: boolean) => {
    const newTheme = val ? 'dark' : 'light'
    theme.global.name.value = newTheme
    localStorage.setItem('theme', newTheme)
    if (currentUser.value?.preferences) {
      try {
        await updateMe({ preferences: { theme: newTheme } })
        currentUser.value = {
          ...currentUser.value,
          preferences: { ...currentUser.value.preferences, theme: newTheme }
        }
      } catch (err) {
        console.error('Failed to update theme preference:', err)
      }
    }
  }
})

// Lifecycle
onMounted(() => {
  const savedUser = localStorage.getItem('user')
  if (savedUser) {
    try {
      setUser(JSON.parse(savedUser))
    } catch (err) {
      console.error('Failed to parse user from localStorage', err)
    }
  }
})

  onBeforeUnmount(() => {
    if (previousBlobUrl) URL.revokeObjectURL(previousBlobUrl)
  })

// Watch avatar changes
watch(
    () => currentUser.value?.avatarUrl,
    async (fileId) => {
      if (!fileId) {
        avatarBlobUrl.value = defaultAvatar
        return
      }

      try {
        const res = await fileApi.downloadFile(fileId)
        const blob = res.data as Blob 
        if (previousBlobUrl) URL.revokeObjectURL(previousBlobUrl)
        avatarBlobUrl.value = URL.createObjectURL(blob)
        previousBlobUrl = avatarBlobUrl.value
      } catch (err) {
        console.error('Failed to load avatar image:', err)
        avatarBlobUrl.value = defaultAvatar
      }
    },
    { immediate: true }
)

// Watch theme from backend
  watch(
      () => currentUser.value?.preferences?.theme,
      themeFromBackend => {
        if (themeFromBackend) theme.global.name.value = themeFromBackend
      },
      { immediate: true }
  )

// Avatar handlers
  const triggerAvatarUpload = () => avatarInput.value?.click()

const handleAvatarSelected = async (event: Event) => {
  const target = event.target as HTMLInputElement
  if (!target.files?.length || !currentUser.value) return

  const file = target.files[0]
  const oldFileId = currentUser.value.avatarUrl // store previous avatar ID

  try {

    const fileId = await fileApi.uploadAvatar(file, currentUser.value.id)

    await updateMe({ avatarUrl: fileId })
    currentUser.value = { ...currentUser.value, avatarUrl: fileId }
    localStorage.setItem('user', JSON.stringify(currentUser.value))

    if (oldFileId) {
      try {
        await fileApi.deleteFile(oldFileId)
      } catch (err) {
        console.warn('Failed to delete old avatar:', err)
      }
    }

    if (previousBlobUrl) URL.revokeObjectURL(previousBlobUrl)
    avatarBlobUrl.value = URL.createObjectURL(file)
    previousBlobUrl = avatarBlobUrl.value

  } catch (err) {
    console.error('Avatar upload failed:', err)
  } finally {
    target.value = ''
  }
}

// Logout
  const handleLogout = async () => {
    try {
      await logout()
      setUser(null)
      localStorage.removeItem('user')
      menu.value = false
      router.push('/')
    } catch (err) { console.error('Logout failed:', err) }
  }

// Persist user
  watch(currentUser, user => {
    if (user) localStorage.setItem('user', JSON.stringify(user))
    else localStorage.removeItem('user')
  }, { deep: true })
</script>

<style scoped>
.cursor-pointer { cursor: pointer; }
.d-none { display: none; }
</style>
