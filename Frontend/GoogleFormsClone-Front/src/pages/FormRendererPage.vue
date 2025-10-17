<template>
  <v-container class="pa-6">
    <v-card class="pa-4" outlined>
      <h2>{{ form.title }}</h2>
      <p v-if="form.description">{{ form.description }}</p>

      <!-- Questions -->
      <div v-for="q in visibleQuestions" :key="q.id">
        <QuestionRenderer
            :question="q"
            v-model="answers[q.id]"
        />
      </div>

      <!-- Validation errors -->
      <v-alert v-if="validationErrors.length" type="error" outlined class="mt-4">
        <div v-for="err in validationErrors" :key="err">{{ err }}</div>
      </v-alert>

      <!-- Submit button -->
      <v-btn color="success" class="mt-4" @click="submitForm">Submit</v-btn>
    </v-card>
  </v-container>
</template>

<script setup lang="ts">
import { reactive, ref, computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import QuestionRenderer from '../components/QuestionRenderer.vue';
import { formApi, responseApi } from '../api';
import type { VueQuestion } from '../types';

const route = useRoute();
const formId = route.params.formId as string;

const form = reactive({
  id: '',
  title: '',
  description: '',
  settings: {} as Record<string, any>,
  questions: [] as VueQuestion[]
});

const answers = reactive<Record<string, any>>({});
const validationErrors = ref<string[]>([]);

onMounted(async () => {
  try {
    const res = await formApi.getById(formId);
    const data = res.data;

    form.id = data.id || '';
    form.title = data.title || '';
    form.description = data.description || '';
    form.settings = data.settings || {};

    form.questions = Array.isArray(data.questions)
        ? data.questions.map((q, idx) => ({
          id: q.id || crypto.randomUUID(),
          type: q.type?.toLowerCase().replace(/\s+/g, '-') || 'text', 
          label: q.questionText?.trim() || `Question ${idx + 1}`,
          description: q.description || '',
          required: q.required || false,
          options: q.options || [],
          linearScale: q.linearScale || { minValue: 1, maxValue: 5, minLabel: '', maxLabel: '' },
          rules: q.required ? ['required'] : [],
          logic: q.logic || null,
          appearance: q.appearance || null,
          allowEditing: form.settings.allowEditing !== false
        }))
        : [];

    // Initialize answers for all question types
    form.questions.forEach(q => {
      switch (q.type) {
        case 'checkbox':
          answers[q.id] = [];
          break;
        case 'linearScale':
          answers[q.id] = q.linearScale?.minValue ?? 1;
          break;
        case 'date':
          answers[q.id] = null;
          break;
        default:
          answers[q.id] = '';
      }
    });
  } catch (err) {
    console.error('Error loading form:', err);
  }
});

// Compute visible questions (logic dependencies)
const visibleQuestions = computed(() =>
    form.questions.filter(q => {
      if (!q.logic?.dependsOn) return true;

      const parentValue = answers[q.logic.dependsOn];
      if (parentValue === undefined || parentValue === null) return false;

      switch (q.logic.condition) {
        case 'equals': return parentValue == q.logic.value;
        case 'notEquals': return parentValue != q.logic.value;
        case 'contains': return Array.isArray(parentValue) && parentValue.includes(q.logic.value);
        default: return true;
      }
    })
);

const submitForm = async () => {
  validationErrors.value = [];

  // Frontend validation
  for (const q of visibleQuestions.value) {
    const val = answers[q.id];
    const isEmpty = val === '' || val === null || (Array.isArray(val) && val.length === 0);
    if (q.required && isEmpty) {
      validationErrors.value.push(`Question "${q.label}" is required.`);
    }
  }
  if (validationErrors.value.length) return;

  // Prepare payload
  const responsePayload = {
    formId: form.id,
    submittedBy: localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).id : null,
    submittedAt: new Date(),
    answers: form.questions.map(q => {
      const val = answers[q.id];
      return {
        questionId: q.id,
        answerText: ['text','textarea'].includes(q.type) ? val : null,
        selectedOptions: q.type === 'checkbox' ? val : null,
        linearScaleValue: q.type === 'linear-scale' ? val : null, // note dash
        questionSnapshot: {
          questionText: q.label,
          questionType: q.type,
          options: q.options?.map(o => ({ id: o.id, text: o.text })) || []
        }
      };
    })
  };

  try {
    await responseApi.submit(responsePayload);
    alert('✅ Form submitted successfully!');
  } catch (err) {
    console.error('Submission error:', err);
    alert('❌ Failed to submit form.');
  }
};
</script>
