<template>
  <v-container class="fill-height d-flex justify-center align-center">
    <v-card elevation="10" class="pa-8" max-width="400">
      <v-card-title class="justify-center text-h5 font-weight-bold">
        Login to FormBuilder
      </v-card-title>

      <v-card-text>
        <v-form @submit.prevent="handleSubmit" lazy-validation>
          <v-text-field
              v-model="email"
              label="Email"
              type="email"
              required
              :rules="emailRules"
              prepend-inner-icon="mdi-email"
              class="mb-4"
          />
          <v-text-field
              v-model="password"
              :type="showPassword ? 'text' : 'password'"
              label="Password"
              required
              prepend-inner-icon="mdi-lock"
              :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
              @click:append-inner="showPassword = !showPassword"
              class="mb-4"
          />

          <v-alert
              v-if="error"
              type="error"
              dense
              text
              class="mb-4"
          >
            {{ error }}
          </v-alert>

          <v-btn
              :loading="loading"
              color="primary"
              type="submit"
              block
          >
            Login
          </v-btn>
        </v-form>
      </v-card-text>

      <v-card-actions class="justify-center">
        <span>Don't have an account?</span>
        <v-btn text color="primary" @click="$router.push('/register')">
          Register
        </v-btn>
      </v-card-actions>
  </v-card>
  </v-container>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'
import DefaultLayout from '../layouts/DefaultLayout.vue'

const router = useRouter()
const { login, loading, error } = useAuth()

const email = ref('')
const password = ref('')
const showPassword = ref(false)

const emailRules = [
  (v: string) => !!v || 'Email is required',
  (v: string) => /^\S+@\S+\.\S+$/.test(v) || 'Email must be valid',
]
const passwordRules = [
  v => !!v || 'Password is required',
]

const handleSubmit = async () => {
  if (!email.value || !password.value) return
  try {
    await login(email.value, password.value)
    await router.push('/dashboard')
  } catch (err) {
    console.error(err)
  }
}
</script>
