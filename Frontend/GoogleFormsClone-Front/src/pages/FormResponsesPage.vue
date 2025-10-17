<template>
  <DefaultLayout>
    <div class="vf-p-8 vf-bg-gray-50 vf-min-h-screen">
      <h1 class="vf-text-2xl vf-font-semibold vf-mb-6">Form Responses</h1>

      <!-- Table of responses -->
      <div class="vf-overflow-auto vf-shadow-md vf-rounded-lg vf-bg-white">
        <table class="vf-w-full vf-text-left vf-border-collapse">
          <thead>
          <tr class="vf-bg-gray-100">
            <th class="vf-px-4 vf-py-2">#</th>
            <th class="vf-px-4 vf-py-2">Respondent</th>
            <th class="vf-px-4 vf-py-2">Email</th>
            <th class="vf-px-4 vf-py-2">Submitted At</th>
            <th class="vf-px-4 vf-py-2">Actions</th>
          </tr>
          </thead>
          <tbody>
          <tr v-for="(resp, idx) in responses" :key="resp.id">
            <td class="vf-px-4 vf-py-2">{{ idx + 1 }}</td>
            <td class="vf-px-4 vf-py-2">{{ resp.name }}</td>
            <td class="vf-px-4 vf-py-2">{{ resp.email }}</td>
            <td class="vf-px-4 vf-py-2">{{ formatDate(resp.submittedAt) }}</td>
            <td class="vf-px-4 vf-py-2">
              <button @click="viewResponse(resp)" class="vf-bg-blue-600 vf-text-white vf-px-3 vf-py-1 vf-rounded vf-cursor-pointer hover:vf-bg-blue-700">
                View
              </button>
            </td>
          </tr>
          </tbody>
        </table>
      </div>

      <!-- VF Modal for viewing response -->
      <Vueform
          v-if="selectedResponse"
          :schema="modalSchema"
          :display-errors="false"
          size="md"
          :model="modalData"
          @close="selectedResponse = null"
      />
    </div>
  </DefaultLayout>
</template>

<script>
import { ref, reactive, computed } from 'vue';

export default {
  setup() {
    const responses = ref([
      // Example, replace with API data
      { id: '1', name: 'John Doe', email: 'john@example.com', submittedAt: new Date(), answers: [{ question: 'Favorite color?', answer: 'Blue' }] },
      { id: '2', name: 'Jane Smith', email: 'jane@example.com', submittedAt: new Date(), answers: [{ question: 'Favorite color?', answer: 'Red' }] }
    ]);

    const selectedResponse = ref(null);

    const modalData = reactive({});

    const modalSchema = computed(() => {
      if (!selectedResponse.value) return {};
      const schema = { page_title: { type: 'static', tag: 'h2', content: 'Response Details', attrs: { class: 'vf-text-xl vf-font-bold vf-mb-4' } } };
      selectedResponse.value.answers.forEach((ans, idx) => {
        schema[`q_${idx}`] = {
          type: 'static',
          tag: 'p',
          content: `${ans.question}: ${ans.answer}`,
          attrs: { class: 'vf-mb-2' }
        };
      });
      schema.close = { type: 'button', buttonLabel: 'Close', full: true, size: 'md', emits: ['close'] };
      return schema;
    });

    function viewResponse(resp) {
      selectedResponse.value = resp;
      // Populate modalData if needed
      Object.assign(modalData, resp);
    }

    function formatDate(date) {
      const d = new Date(date);
      return d.toLocaleString();
    }

    return { responses, selectedResponse, viewResponse, formatDate, modalSchema, modalData };
  }
};
</script>
