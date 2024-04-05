<template>
  <WizardHeader class="mb-6" />
  <v-stepper v-model="wizardStore.step" min-height="100dvh" :alt-labels="true" :mobile="$vuetify.display.mobile">
    <v-stepper-header>
      <template v-for="(step, index) in Object.values(wizard.steps)" :key="step.stage">
        <v-stepper-item
          color="primary"
          :step="wizardStore.step"
          :value="index + 1"
          :title="step.title"
          :rules="wizardStore.step <= index + 1 ? [] : [() => wizardStore.validationState[step.stage]]"
        ></v-stepper-item>
        <v-divider v-if="index !== Object.values(wizard.steps).length - 1" :key="`divider-${index}`" />
      </template>
    </v-stepper-header>
    <v-stepper-window v-model="wizardStore.step">
      <v-stepper-window-item v-for="(step, index) in Object.values(wizard.steps)" :key="step.stage" :value="index + 1">
        <v-container>
          <v-row class="justify-space-between">
            <v-col cols="auto">
              <h3>{{ step.title }}</h3>
            </v-col>
            <v-col v-if="wizardStore.currentStepStage === 'Review'" cols="auto">
              <ConfirmationDialog
                v-if="!isWizardDataValid"
                :config="{ cancelButtonText: 'Cancel', acceptButtonText: 'Yes', title: 'Print Confirmation', customButtonVariant: 'text' }"
                @accept="printPage"
              >
                <template #activator>
                  <v-icon color="secondary" icon="mdi-printer-outline" class="mr-2"></v-icon>
                  <a class="small">Print Preview</a>
                </template>
                <template #confirmation-text>
                  <p>
                    Your Application contains missing data and/or invalid data.
                    <br />
                    The printed preview will show which sections are incomplete
                  </p>
                  <br />
                  <p><b>Are you sure you want to proceed?</b></p>
                </template>
              </ConfirmationDialog>
              <v-btn v-if="isWizardDataValid" variant="text" onclick="window.print()">
                <v-row align="center" justify="end">
                  <v-icon color="secondary" icon="mdi-printer-outline" class="mr-2"></v-icon>
                  <a class="small">Print Preview</a>
                </v-row>
              </v-btn>
            </v-col>
          </v-row>

          <h4>{{ step.subtitle }}</h4>
          <DeclarationStepContent v-if="step.stage == 'Declaration'" class="mt-6" />
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

import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import DeclarationStepContent from "@/components/DeclarationStepContent.vue";
import EceForm from "@/components/Form.vue";
import WizardHeader from "@/components/WizardHeader.vue";
import applicationWizard from "@/config/application-wizard";
import { useAlertStore } from "@/store/alert";
import { useCertificationTypeStore } from "@/store/certificationType";
import { useUserStore } from "@/store/user";
import { useWizardStore } from "@/store/wizard";
import type { Step, Wizard } from "@/types/wizard";

export default defineComponent({
  name: "Wizard",
  components: { WizardHeader, EceForm, DeclarationStepContent, ConfirmationDialog },
  props: {
    wizard: {
      type: Object as PropType<Wizard>,
      default: () => applicationWizard,
    },
  },
  setup: async () => {
    const wizardStore = useWizardStore();
    const userStore = useUserStore();
    const alertStore = useAlertStore();
    const certificationTypeStore = useCertificationTypeStore();

    return {
      certificationTypeStore,
      wizardStore,
      userStore,
      alertStore,
    };
  },
  computed: {
    isWizardDataValid() {
      return !(Object.values(this.wizardStore.validationState).indexOf(false) > -1);
    },
  },

  methods: {
    getStepTitles(): string[] {
      return Object.values(this.wizard.steps).map((step: Step) => step.title);
    },
    printPage() {
      window.print();
    },
  },
});
</script>
