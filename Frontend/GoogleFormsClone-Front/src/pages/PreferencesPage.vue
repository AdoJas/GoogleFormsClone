<template>
  <DefaultLayout>
    <div class="vf-max-w-xl vf-mx-auto vf-bg-white vf-rounded-lg vf-shadow-md vf-p-8">
      <h1 class="vf-text-2xl vf-font-bold vf-mb-6 text-center">User Preferences</h1>

      <Vueform v-bind="vueform" @submit="onSubmit" />

      <div v-if="successMessage" class="vf-text-green-600 vf-mt-4 text-center">
        {{ successMessage }}
      </div>
      <div v-if="errorMessage" class="vf-text-red-600 vf-mt-4 text-center">
        {{ errorMessage }}
      </div>
    </div>
  </DefaultLayout>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axios from 'axios'
import DefaultLayout from '../layouts/DefaultLayout.vue'
import { defineConfig } from '@vueform/vueform'
import en from '@vueform/vueform/locales/en'
import Vueform from '@vueform/vueform'

const successMessage = ref('')
const errorMessage = ref('')

const vueform = ref(
    defineConfig({
      size: 'md',
      displayErrors: true,
      schema: {
        theme: 'vueform',
        locales: { en },
        locale: 'en',
        classHelpers: true,
        username: {
          type: 'text',
          label: 'Username',
          placeholder: 'Enter your username',
          rules: ['required', 'max:50'],
        },
        email: {
          type: 'text',
          inputType: 'email',
          label: 'Email',
          placeholder: 'Enter your email',
          rules: ['required', 'email'],
        },
        avatar: {
          type: 'file',
          label: 'Upload Profile Photo',
          accept: 'image/*',
          rules: ['max:2097152'], // max 2MB
        },
        theme: {
          type: 'select',
          label: 'Theme',
          placeholder: 'Choose a theme',
          items: [
            { value: 'light', text: 'Light' },
            { value: 'dark', text: 'Dark' },
          ],
        },
        notifications: {
          type: 'checkbox',
          label: 'Receive email notifications',
        },
        save: {
          type: 'button',
          buttonLabel: 'Save Preferences',
          submits: true,
          full: true,
          size: 'lg',
        },
      },
    })
)

const loadPreferences = async () => {
  try {
    const res = await axios.get('http://localhost:5057/api/user/preferences', {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`,
      },
    })
    const values = res.data
    Object.keys(values).forEach((key) => {
      if (vueform.value.schema[key]) {
        vueform.value.schema[key].value = values[key]
      }
    })
  } catch (err) {
    console.error(err)
  }
}

const onSubmit = async (values: any) => {
  const formData = new FormData()
  Object.entries(values).forEach(([key, value]) => {
    if (value instanceof File) {
      formData.append(key, value)
    } else {
      formData.append(key, value)
    }
  })

  await axios.put('http://localhost:5057/api/user/preferences', formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
      Authorization: `Bearer ${localStorage.getItem('accessToken')}`,
    },
  })
}

onMounted(loadPreferences)
</script>
