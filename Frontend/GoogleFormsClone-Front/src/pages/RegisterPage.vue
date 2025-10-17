<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'

const router = useRouter()
const { register, loading, error } = useAuth()

const firstName = ref('')
const lastName = ref('')
const email = ref('')
const password = ref('')
const passwordConfirm = ref('')
const showPassword = ref(false)
const showPasswordConfirm = ref(false)

// form validation rules
const nameRules = [(v: string) => !!v || 'Name is required']
const emailRules = [
  (v: string) => !!v || 'Email is required',
  (v: string) => /^\S+@\S+\.\S+$/.test(v) || 'Email must be valid',
]
const passwordRules = [
  (v: string) => !!v || 'Password is required',
  (v: string) => v.length >= 8 || 'Password must be at least 8 characters',
]
const passwordConfirmRules = [
  (v: string) => !!v || 'Please confirm your password',
  (v: string) => v === password.value || 'Passwords must match',
]

const handleSubmit = async () => {
  try {
    await register(email.value, password.value, passwordConfirm.value, `${firstName.value} ${lastName.value}`)
    router.push('/dashboard')
  } catch (err) {
    console.error(err)
  }
}
</script>

<template>
  <v-main>
    <v-container class="fill-height d-flex justify-center align-center">
      <v-card elevation="10" class="pa-8" max-width="500">
        <v-card-title class="justify-center text-h5 font-weight-bold">
          Create an Account
        </v-card-title>

        <v-card-text>
          <v-form @submit.prevent="handleSubmit" lazy-validation>
            <v-row>
              <v-col cols="12">
                <v-text-field
                    v-model="firstName"
                    label="First Name"
                    required
                    :rules="nameRules"
                />
              </v-col>
            </v-row>

            <v-text-field
                v-model="email"
                label="Email"
                type="email"
                required
                :rules="emailRules"
                class="mb-4"
            />

            <v-text-field
                v-model="password"
                :type="showPassword ? 'text' : 'password'"
                label="Password"
                required
                :rules="passwordRules"
                :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
                @click:append-inner="showPassword = !showPassword"
                class="mb-4"
            />

            <v-text-field
                v-model="passwordConfirm"
                :type="showPasswordConfirm ? 'text' : 'password'"
                label="Confirm Password"
                required
                :rules="passwordConfirmRules"
                :append-inner-icon="showPasswordConfirm ? 'mdi-eye-off' : 'mdi-eye'"
                @click:append-inner="showPasswordConfirm = !showPasswordConfirm"
                class="mb-4"
            />

            <v-alert v-if="error" type="error" dense text class="mb-4">
              {{ error }}
            </v-alert>

            <v-btn :loading="loading" color="primary" type="submit" block>
              Create Account
            </v-btn>
          </v-form>
        </v-card-text>

        <v-card-actions class="justify-center">
          <span>Already have an account?</span>
          <v-btn text color="primary" @click="$router.push('/login')">
            Sign in
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-container>
  </v-main>
</template>
