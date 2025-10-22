<template>
  <v-card class="pa-4 mb-4 elevation-2">
    <!-- Label and type -->
    <v-row>
      <v-col cols="12" md="8">
        <v-text-field :label="question.label" :readonly="!question.allowEditing" outlined dense v-if="question.type !== 'linearScale' && question.type !== 'checkbox' && question.type !== 'date'" v-model="localAnswer"/>
      </v-col>
      <v-col cols="12" md="4">
        <v-chip small>{{ question.type }}</v-chip>
        <v-chip small v-if="question.required">Required</v-chip>
      </v-col>
    </v-row>

    <!-- Description -->
    <v-row v-if="question.description">
      <v-col>
        <v-textarea :value="question.description" outlined dense readonly rows="2"/>
      </v-col>
    </v-row>

    <!-- Render inputs -->
    <div class="mt-4">
      <!-- Textarea -->
      <v-textarea v-if="question.type==='textarea'" v-model="localAnswer" :readonly="!question.allowEditing" outlined dense rows="3"/>

      <!-- Select / dropdown -->
      <v-select v-else-if="['select','dropdown','radio'].includes(question.type)" v-model="localAnswer" :items="question.options.map(o=>o.text)" :readonly="!question.allowEditing" outlined dense/>

      <!-- Checkbox / multi-select -->
      <v-checkbox-group v-else-if="question.type==='checkbox'" v-model="localAnswer" :disabled="!question.allowEditing">
        <v-checkbox v-for="o in question.options" :key="o.id" :label="o.text" :value="o.text"/>
      </v-checkbox-group>

      <!-- Linear scale -->
      <v-card v-else-if="question.type==='linearScale'" outlined class="pa-3">
        <div class="d-flex justify-space-between mb-2">
          <span>{{ question.linearScale?.minLabel || question.linearScale?.minValue }}</span>
          <span>{{ question.linearScale?.maxLabel || question.linearScale?.maxValue }}</span>
        </div>
        <v-slider v-model="localAnswer" :min="question.linearScale?.minValue ?? 1" :max="question.linearScale?.maxValue ?? 5" step="1" thumb-label :disabled="!question.allowEditing"/>
      </v-card>

      <!-- Date -->
      <v-menu v-else-if="question.type==='date'" v-model="menu" :close-on-content-click="false" transition="scale-transition" offset-y max-width="290px" min-width="auto">
        <template #activator="{ props }">
          <v-text-field v-model="localAnswer" label="Select date" readonly v-bind="props" outlined dense/>
        </template>
        <v-date-picker v-model="localAnswer" @input="menu=false"/>
      </v-menu>
    </div>

    <!-- Help text -->
    <div v-if="question.appearance?.helpText" class="grey--text text-caption mt-2">
      {{ question.appearance.helpText }}
    </div>
  </v-card>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import type { VueQuestion } from '../types';

const props = defineProps<{ question: VueQuestion, modelValue: any }>();
const emit = defineEmits(['update:modelValue']);

const localAnswer = ref(props.modelValue);
const menu = ref(false);

watch(localAnswer, val => emit('update:modelValue', val));
watch(() => props.modelValue, val => localAnswer.value = val);
</script>