<template>
  <DefaultLayout>
    <v-main class="vf-bg-gray-100">
      <v-container class="py-8">
        <div class="vf-flex vf-justify-between vf-items-center mb-8">
          <h1 class="text-h4 font-weight-bold">My Forms</h1>
          <RouterLink to="/form-builder">
            <v-btn
                color="primary"
                variant="elevated"
                rounded
                class="px-6 py-2"
            >
              + Create New Form
            </v-btn>
          </RouterLink>
        </div>

        <v-row class="gap-6">
          <v-col
              v-for="form in forms"
              :key="form.id"
              cols="12"
              sm="6"
              md="4"
          >
            <FormCard
                :form="form"
                @delete="deleteForm"
            />
          </v-col>
        </v-row>

        <div
            v-if="forms.length === 0"
            class="vf-text-center vf-text-gray-500 mt-16 text-lg"
        >
          You have no forms yet. Click "Create New Form" to get started!
        </div>
      </v-container>
    </v-main>
  </DefaultLayout>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useAuth } from '../composables/useAuth'
import axios from 'axios'
import FormCard from '../components/FormCard.vue'
import DefaultLayout from '../layouts/DefaultLayout.vue'
import api from '../api/axios'

interface Form {
  id: string
  title: string
  createdAt: string
  responseCount: number
  creatorId: string
}

const forms = ref<Form[]>([])
const { user } = useAuth()


const fetchForms = async () => {
  if (!user.value) return
  try {
    const res = await api.get<Form[]>(`/forms/user/${user.value.id}`)
    forms.value = res.data
  } catch (err: any) {
    console.error('Failed to fetch forms:', err.response?.status, err.response?.data)
  }
}

const deleteForm = async (id: string) => {
  if (!confirm('Are you sure you want to delete this form?')) return
  try {
    await axios.delete(`http://localhost:5057/api/forms/${id}`)
    forms.value = forms.value.filter(f => f.id !== id)
  } catch (err) {
    console.error(err)
  }
}

onMounted(fetchForms)
watch(user, fetchForms)
</script>
