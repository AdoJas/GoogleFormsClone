<template>
  <v-container class="form-builder-container">
    <!-- Form Title & Description -->
    <v-text-field
        v-model="formTitle"
        label="Form Title"
        outlined
        dense
        :rules="[v => !!v || 'Title is required']"
        class="mb-4"
    />
    <v-textarea
        v-model="formDescription"
        label="Form Description"
        outlined
        dense
        class="mb-6"
    />

    <!-- Settings Menu -->
    <v-row>
      <v-col>
        <v-menu v-model="settingsMenu" offset-y :close-on-content-click="false">
          <template #activator="{ props, on }">
            <v-btn color="primary" v-bind="props" v-on="on">Form Settings</v-btn>
          </template>

          <v-card class="pa-4" style="width: 400px;" @click.stop>
            <v-switch v-model="formSettings.allowEditing" label="Allow Editing" color="primary"/>
            <v-switch v-model="formSettings.oneResponsePerUser" label="One Response Per User" color="primary"/>
            <v-switch v-model="formSettings.showProgress" label="Show Progress" color="primary"/>
            <v-text-field v-model="formSettings.confirmationMessage" label="Confirmation Message"/>
            <v-switch v-model="formSettings.collectEmails" label="Collect Emails" color="primary"/>
            <v-switch v-model="formSettings.allowResponseEditing" label="Allow Response Editing" color="primary"/>
            <v-text-field
                v-model.number="formSettings.responseEditingDuration"
                label="Response Editing Duration (minutes)"
                type="number"
            />
            <v-btn color="success" block class="mt-4" @click="settingsMenu=false">Close</v-btn>
          </v-card>
        </v-menu>
      </v-col>
    </v-row>

    <!-- Question Builder -->
    <v-divider class="my-6"/>
    <div>
      <h3 class="mb-3">Questions</h3>
      <div
          v-for="(q, key) in vueform.schema"
          :key="key"
          class="mb-6"
          :style="{ marginLeft: q.logic?.dependsOn ? '40px' : '0px' }"
      >
        <v-card class="pa-4">
          <v-row>
            <v-col cols="12" md="8">
              <v-text-field
                  v-model="q.questionText"
                  label="Question Text"
                  outlined dense
                  :rules="[v => !!v || 'Question text is required']"
              />
            </v-col>
            <v-col cols="12" md="4">
              <v-select
                  v-model="q.type"
                  :items="questionTypes"
                  label="Type" outlined dense
              />
              <v-switch v-model="q.required" label="Required" color="primary"/>
            </v-col>
          </v-row>

          <v-textarea v-model="q.description" label="Description" outlined dense/>

          <!-- Options for multiple choice -->
          <div v-if="['radio','checkbox','dropdown','select'].includes(q.type)" class="mt-4">
            <v-row v-for="(opt, idx) in q.options" :key="opt.id" class="mb-2">
              <v-col cols="10">
                <v-text-field v-model="opt.text" label="Option" dense outlined/>
              </v-col>
              <v-col cols="2">
                <v-btn icon color="error" @click="removeOption(q.id, idx)">
                  <v-icon>mdi-delete</v-icon>
                </v-btn>
              </v-col>
            </v-row>
            <v-btn text small @click="addOption(q.id)">+ Add Option</v-btn>
          </div>

          <!-- Linear Scale -->
          <div v-if="q.type === 'linearScale'" class="mt-4">
            <v-card outlined class="pa-3">
              <!-- Min/Max values -->
              <div class="d-flex mb-2">
                <v-text-field v-model.number="q.linearScale.minValue" label="Min Value" type="number" class="mr-2" dense/>
                <v-text-field v-model.number="q.linearScale.maxValue" label="Max Value" type="number" dense/>
              </div>
              <!-- Min/Max labels -->
              <div class="d-flex mb-2">
                <v-text-field v-model="q.linearScale.minLabel" label="Min Label" class="mr-2" dense/>
                <v-text-field v-model="q.linearScale.maxLabel" label="Max Label" dense/>
              </div>

              <!-- Live preview -->
              <div class="d-flex justify-space-between mb-2">
                <span>{{ q.linearScale.minLabel || q.linearScale.minValue }}</span>
                <span>{{ q.linearScale.maxLabel || q.linearScale.maxValue }}</span>
              </div>
              <v-slider
                  v-model="q.linearScaleValue"
                  :min="q.linearScale.minValue"
                  :max="q.linearScale.maxValue"
                  step="1"
                  thumb-label
              />
            </v-card>
          </div>

          <!-- Logic / Dependency -->
          <div class="mt-3">
            <v-btn text small v-if="!q.logic" @click="addLogic(q.id)">
              + Add Logic
            </v-btn>
            <v-card outlined class="pa-2" v-else>
              <h4 class="mb-2">Logic / Depends On</h4>
              <v-select
                  v-model="q.logic.dependsOn"
                  :items="parentQuestions(q.id)"
                  label="Depends On"
                  dense
              />
              <v-select
                  v-model="q.logic.condition"
                  :items="['equals','notEquals','contains']"
                  label="Condition"
                  dense
              />
              <v-text-field v-model="q.logic.value" label="Value" dense/>
              <v-select
                  v-model="q.logic.action"
                  :items="['show','hide','enable','disable']"
                  label="Action"
                  dense
              />
              <v-btn text small color="error" @click="removeLogic(q.id)">Remove Logic</v-btn>
            </v-card>
          </div>

          <v-btn color="error" class="mt-4" @click="removeQuestion(q.id)">Remove Question</v-btn>
        </v-card>
      </div>
      <v-btn color="primary" class="mt-4" @click="addQuestion()">+ Add Question</v-btn>
    </div>

    <!-- Save Form -->
    <v-divider class="my-6"/>
    <v-btn color="success" block large @click="saveForm" :disabled="!validateForm()">Save Form</v-btn>
  </v-container>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { formApi } from '../api';

const formTitle = ref('');
const formDescription = ref('');
const formSettings = reactive({
  allowEditing: false,
  oneResponsePerUser: false,
  showProgress: false,
  confirmationMessage: '',
  collectEmails: false,
  allowResponseEditing: false,
  responseEditingDuration: 0
});

const vueform = reactive<{ schema: Record<string, any> }>({ schema: {} });
const settingsMenu = ref(false);

const questionTypes = ['text', 'textarea', 'select', 'checkbox', 'radio', 'linearScale', 'date'];

// Add question
const addQuestion = () => {
  const id = crypto.randomUUID();
  vueform.schema[id] = reactive({
    id,
    questionText: 'New Question',
    description: '',
    type: 'text',
    required: false,
    options: [],
    orderIndex: Object.keys(vueform.schema).length,
    logic: null,
    linearScale: { minValue:1, maxValue:5, minLabel:'', maxLabel:'' },
    linearScaleValue: 1
  });
};

// Remove question
const removeQuestion = (id: string) => { delete vueform.schema[id]; };

// Add/remove option
const addOption = (questionId: string) => {
  vueform.schema[questionId].options.push({ id: crypto.randomUUID(), text: '' });
};
const removeOption = (questionId: string, idx: number) => { vueform.schema[questionId].options.splice(idx,1); };

// Logic reactive
const addLogic = (questionId: string) => {
  vueform.schema[questionId].logic = reactive({
    dependsOn: '',
    condition: 'equals',
    value: '',
    action: 'show'
  });
};
const removeLogic = (questionId: string) => {
  vueform.schema[questionId].logic = null;
};

// Only allow previous questions as parents
const parentQuestions = (questionId: string) => {
  return Object.values(vueform.schema)
      .filter(q => q.id !== questionId)
      .map(q => ({ label: q.questionText, value: q.id }));
};

// Form validation
const validateForm = (): boolean => {
  if (!formTitle.value.trim()) return false;

  for (const q of Object.values(vueform.schema)) {
    if (q.required && !q.questionText.trim()) return false;
    if (['radio','checkbox','dropdown','select'].includes(q.type)) {
      if (q.required && (!q.options || q.options.length === 0)) return false;
      for (const opt of q.options) if (!opt.text.trim()) return false;
    }
    if (q.type === 'linearScale' && q.linearScale.minValue >= q.linearScale.maxValue) return false;
  }
  return true;
};

// Save form
const saveForm = async () => {
  if (!validateForm()) { alert('Form invalid!'); return; }
  const questions = Object.values(vueform.schema).map(q => ({ ...q, orderIndex: q.orderIndex ?? 0 }));
  const payload = { title: formTitle.value, description: formDescription.value, settings: formSettings, questions };
  try {
    await formApi.create(payload);
    alert('Form saved!');
  } catch (err) {
    console.error(err);
    alert('Failed to save form.');
  }
};
</script>

<style scoped>
.form-builder-container { padding-top: 24px; }
</style>
