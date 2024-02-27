<template>
  <WizardHeader class="mb-6" />
  <v-stepper v-model="wizardStore.step" min-height="100dvh" flat color="primary" :items="getStepTitles()" :alt-labels="true" :mobile="$vuetify.display.mobile">
    <template v-for="step in wizard.steps" :key="step.id" #[step.key]>
      <v-container>
        <h3>{{ step.title }}</h3>
        <h4>{{ step.subtitle }}</h4>
        <DeclarationStepContent v-if="step.id == 'declaration'" class="mt-6" />
        <v-row>
          <v-col cols="12" md="8" lg="6" xl="4">
            <EceForm
              :form="step.form"
              :form-data="wizardStore.wizardData"
              @updated-form-data="wizardStore.setWizardData"
              @updated-validation="$emit('updatedValidation', $event)"
            />
          </v-col>
        </v-row>
      </v-container>
    </template>
    <template #actions>
      <v-container class="mb-8">
        <v-row class="justify-space-between ga-4" no-gutters>
          <v-col cols="auto" class="mr-auto">
            <v-btn :disabled="wizardStore.step === 1" rounded="lg" variant="outlined" color="primary" aut @click="$emit('back')">Back</v-btn>
          </v-col>
          <v-col cols="auto">
            <v-btn rounded="lg" variant="outlined" color="primary" class="mr-4" primary @click="$emit('saveAsDraft')">Save as Draft</v-btn>
            <v-btn type="submit" :form="getFormId" rounded="lg" color="primary" @click="$emit('saveAndContinue')">Save and Continue</v-btn>
          </v-col>
        </v-row>
      </v-container>
    </template>
  </v-stepper>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import DeclarationStepContent from "@/components/DeclarationStepContent.vue";
import EceForm from "@/components/Form.vue";
import WizardHeader from "@/components/WizardHeader.vue";
import applicationWizard from "@/config/application-wizard";
import { useAlertStore } from "@/store/alert";
import { useUserStore } from "@/store/user";
import { useWizardStore } from "@/store/wizard";
import type { Step, Wizard } from "@/types/wizard";

export default defineComponent({
  name: "Wizard",
  components: { WizardHeader, EceForm, DeclarationStepContent },
  props: {
    wizard: {
      type: Object as PropType<Wizard>,
      default: () => applicationWizard,
    },
  },
  emits: {
    saveAsDraft: () => true,
    saveAndContinue: () => true,
    back: () => true,
    updatedValidation: (_validation: boolean | null) => true,
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
  data: () => ({
    isFormValid: null as boolean | null,
  }),
  computed: {
    getFormId() {
      return this.wizardStore.steps[this.wizardStore.step - 1].form.id;
    },
  },
  methods: {
    getStepTitles() {
      return Object.values(this.wizard.steps).map((step: Step) => step.title);
    },
  },
});
</script>
