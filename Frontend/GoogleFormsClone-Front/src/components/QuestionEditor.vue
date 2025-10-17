<template>
  <v-card class="pa-4 mb-4 elevation-2">
    <!-- Remove Question Button -->
    <div class="d-flex justify-end mb-2">
      <v-btn icon color="red" @click="$emit('remove')">
        <v-icon>mdi-close</v-icon>
      </v-btn>
    </div>

    <!-- Question Basic -->
    <v-row>
      <v-col cols="12" md="8">
        <v-text-field v-model="localQuestion.label" label="Question Text" outlined dense />
      </v-col>
      <v-col cols="12" md="4">
        <v-select v-model="localQuestion.type" :items="questionTypes" label="Question Type" outlined dense />
        <v-checkbox v-model="localQuestion.rules" :value="'required'" label="Required" dense />
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-textarea v-model="localQuestion.description" label="Question Description" rows="2" outlined dense />
      </v-col>
    </v-row>

    <!-- Options for select/checkbox/radio -->
    <div v-if="['select','checkbox','radio'].includes(localQuestion.type)" class="mt-4">
      <v-row v-for="(item, idx) in localQuestion.items" :key="idx" class="align-center mb-2">
        <v-col cols="10">
          <v-text-field v-model="localQuestion.items[idx]" outlined dense />
        </v-col>
        <v-col cols="2">
          <v-btn icon color="red" @click="removeOption(idx)">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </v-col>
      </v-row>
      <v-btn color="primary" text @click="addOption">Add Option</v-btn>
    </div>

    <!-- Linear Scale Preview -->
    <div v-if="localQuestion.type === 'linearScale'" class="mt-4">
      <v-card outlined class="pa-3">
        <div class="d-flex justify-space-between mb-2">
          <span>{{ localQuestion.linearScale.minLabel || localQuestion.linearScale.minValue }}</span>
          <span>{{ localQuestion.linearScale.maxLabel || localQuestion.linearScale.maxValue }}</span>
        </div>
        <v-slider
            v-model="localQuestion.linearScaleValue"
            :min="localQuestion.linearScale.minValue"
            :max="localQuestion.linearScale.maxValue"
            step="1"
            thumb-label
        ></v-slider>
        <small class="grey--text">This is a live preview of the linear scale.</small>
      </v-card>
    </div>

    <!-- Appearance Settings -->
    <v-expansion-panels flat>
      <v-expansion-panel>
        <v-expansion-panel-title>Appearance</v-expansion-panel-title>
        <v-expansion-panel-text>
          <v-text-field v-model="localQuestion.appearance.placeholder" label="Placeholder" outlined dense />
          <v-text-field v-model="localQuestion.appearance.helpText" label="Help Text" outlined dense />
          <v-text-field v-model="localQuestion.appearance.imageUrl" label="Image URL" outlined dense />
        </v-expansion-panel-text>
      </v-expansion-panel>

      <!-- Validation -->
      <v-expansion-panel>
        <v-expansion-panel-title>Validation</v-expansion-panel-title>
        <v-expansion-panel-text>
          <v-text-field v-model.number="localQuestion.validation.minLength" label="Min Length" type="number" outlined dense />
          <v-text-field v-model.number="localQuestion.validation.maxLength" label="Max Length" type="number" outlined dense />
          <v-text-field v-model.number="localQuestion.validation.minValue" label="Min Value" type="number" outlined dense />
          <v-text-field v-model.number="localQuestion.validation.maxValue" label="Max Value" type="number" outlined dense />
        </v-expansion-panel-text>
      </v-expansion-panel>

      <!-- Linear Scale Settings -->
      <v-expansion-panel v-if="localQuestion.type === 'linearScale'">
        <v-expansion-panel-title>Linear Scale Settings</v-expansion-panel-title>
        <v-expansion-panel-text>
          <v-text-field v-model.number="localQuestion.linearScale.minValue" label="Min Value" type="number" outlined dense />
          <v-text-field v-model.number="localQuestion.linearScale.maxValue" label="Max Value" type="number" outlined dense />
          <v-text-field v-model="localQuestion.linearScale.minLabel" label="Min Label" outlined dense />
          <v-text-field v-model="localQuestion.linearScale.maxLabel" label="Max Label" outlined dense />
        </v-expansion-panel-text>
      </v-expansion-panel>

      <!-- Logic / Dependent Question -->
      <v-expansion-panel>
        <v-expansion-panel-title>Logic / Dependent Question</v-expansion-panel-title>
        <v-expansion-panel-text>
          <v-select v-model="localQuestion.logic.dependsOn" :items="parentQuestionOptions" label="Depends On Question" outlined dense />
          <v-text-field v-model="localQuestion.logic.value" label="Trigger Value" outlined dense />
        </v-expansion-panel-text>
      </v-expansion-panel>
    </v-expansion-panels>
  </v-card>
</template>

<script setup lang="ts">
import { reactive, toRefs, watch } from 'vue';

interface LinearScale {
  minValue: number;
  maxValue: number;
  minLabel: string;
  maxLabel: string;
}

interface Validation {
  minLength?: number;
  maxLength?: number;
  minValue?: number;
  maxValue?: number;
}

interface Logic {
  dependsOn?: string;
  value?: string | number;
}

interface Appearance {
  placeholder?: string;
  helpText?: string;
  imageUrl?: string;
}

export interface VueQuestion {
  id?: string;
  label: string;
  description: string;
  type: string;
  rules: string[];
  allowMultipleSelection: boolean;
  items: string[];
  linearScale: LinearScale;
  linearScaleValue?: number;
  validation: Validation;
  logic: Logic;
  appearance: Appearance;
}

const props = defineProps<{
  question: VueQuestion;
  parentQuestionOptions: string[];
}>();

const emit = defineEmits(['remove']);

const localQuestion = reactive({
  ...props.question,
  linearScale: props.question.linearScale || { minValue: 1, maxValue: 5, minLabel: '', maxLabel: '' },
  linearScaleValue: props.question.linearScaleValue ?? props.question.linearScale?.minValue ?? 1,
  validation: props.question.validation || {},
  logic: props.question.logic || {},
  appearance: props.question.appearance || {}
});

watch(localQuestion, (newVal) => {
  Object.assign(props.question, newVal);
}, { deep: true });

const questionTypes = ['text', 'textarea', 'select', 'checkbox', 'radio', 'date', 'linearScale'];

const addOption = () => { localQuestion.items.push(`Option ${localQuestion.items.length + 1}`); };
const removeOption = (idx: number) => { localQuestion.items.splice(idx, 1); };
</script>

<style scoped>
.linear-scale-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
</style>
