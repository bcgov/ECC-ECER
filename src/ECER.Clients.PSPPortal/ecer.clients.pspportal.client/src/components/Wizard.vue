<template>
  <slot name="header"></slot>
  <v-stepper v-model="wizardStore.step" min-height="100dvh" :alt-labels="true" :elevation="0">
    <slot name="stepperHeader">
      <v-stepper-header v-if="showSteps">
        <template v-for="(step, index) in Object.values(wizard.steps)" :key="step.stage">
          <v-stepper-item color="primary" :step="wizardStore.step" :value="index + 1" :title="step.title" :editable="true"></v-stepper-item>
          <v-divider v-if="index !== Object.values(wizard.steps).length - 1" :key="`divider-${index}`" />
        </template>
      </v-stepper-header>
    </slot>
    <v-stepper-window v-model="wizardStore.step">
      <v-stepper-window-item
        v-for="(step, index) in Object.values(wizard.steps)"
        :key="step.stage"
        :value="index + 1"
        :transition="false"
        :reverse-transition="false"
      >
        <v-container>
          <v-row class="justify-space-between mb-4" v-if="step.title || wizardStore.currentStepStage === 'Review'">
            <v-col cols="auto">
              <h1>{{ step.title }}</h1>
            </v-col>
            <v-col v-if="wizardStore.currentStepStage === 'Review'" cols="auto">
              <slot name="PrintPreview"></slot>
            </v-col>
          </v-row>
          <h4 v-if="step.subtitle">{{ step.subtitle }}</h4>
          <v-row>
            <v-col cols="12">
              <EceForm :ref="step.form.id" :form="step.form" :form-data="wizardStore.wizardData" @updated-form-data="wizardStore.setWizardData" />
            </v-col>
          </v-row>
        </v-container>
      </v-stepper-window-item>
    </v-stepper-window>
    <template #actions>
      <slot name="actions"></slot>
    </template>
  </v-stepper>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import EceForm from "@/components/Form.vue";
import { useAlertStore } from "@/store/alert";
import { useUserStore } from "@/store/user";
import { useWizardStore } from "@/store/wizard";
import type { Step, Wizard } from "@/types/wizard";

export default defineComponent({
  name: "Wizard",
  components: { EceForm },
  props: {
    wizard: {
      type: Object as PropType<Wizard>,
      required: true
    },
    showSteps: {
      type: Boolean,
      default: true,
    },
  },
  setup: async () => {
    const wizardStore = useWizardStore();
    const userStore = useUserStore();
    const alertStore = useAlertStore();

    return {
      wizardStore,
      userStore,
      alertStore,
    };
  },

  methods: {
    getStepTitles(): string[] {
      return Object.values(this.wizard.steps).map((step: Step) => step.title);
    },
  },
});
</script>
