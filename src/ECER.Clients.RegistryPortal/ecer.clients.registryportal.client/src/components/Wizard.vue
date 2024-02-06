<template>
  <PageContainer>
    <v-stepper
      v-model="wizardStore.step"
      min-height="100dvh"
      :alt-labels="true"
      bg-color="background"
      flat
      color="primary"
      :items="getStepTitles()"
      :mobile="$vuetify.display.mobile"
    >
      <template v-for="step in wizard.steps" :key="step.id" #[step.key]>
        <v-card class="rounded-lg" color="white" :title="step.title" flat>
          <v-container>
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
        </v-card>
      </template>
      <template #actions>
        <v-container>
          <v-row class="justify-space-between ga-4" no-gutters>
            <v-col cols="auto" class="mr-auto">
              <v-btn rounded="lg" variant="outlined" color="primary" aut @click="$emit('back')">Back</v-btn>
            </v-col>
            <v-col cols="auto">
              <v-btn rounded="lg" variant="outlined" color="primary" class="mr-4" primary @click="$emit('saveAsDraft')">Save as Draft</v-btn>
              <v-btn rounded="lg" color="primary" @click="$emit('saveAndContinue')">Save and Continue</v-btn>
            </v-col>
          </v-row>
        </v-container>
      </template>
    </v-stepper>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import EceForm from "@/components/Form.vue";
import PageContainer from "@/components/PageContainer.vue";
import applicationWizard from "@/config/application-wizard";
import { useAlertStore } from "@/store/alert";
import { useUserStore } from "@/store/user";
import { useWizardStore } from "@/store/wizard";
import type { Step, Wizard } from "@/types/wizard";

export default defineComponent({
  name: "Wizard",
  components: { PageContainer, EceForm },
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
  methods: {
    getStepTitles() {
      return Object.values(this.wizard.steps).map((step: Step) => step.title);
    },
  },
});
</script>
