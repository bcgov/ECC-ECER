<template>
  <Wizard :ref="'wizard'" :wizard="referenceWizardConfig" :config="{ showSteps: false }">
    <template #header>
      <v-container fluid class="bg-white">
        <h2>{{ referenceType + " reference" }}</h2>
        <div role="doc-subtitle">{{ `For applicant: ${applicantFirstName} ${applicantLastName}` }}</div>
        <v-btn v-if="wizardStore.step !== 1" variant="text" rounded="lg" color="primary" @click="handleBack">{{ "< back" }}</v-btn>
      </v-container>
    </template>
    <template #actions>
      <v-container class="mb-8">
        <v-row no-gutters>
          <v-col>
            <v-btn v-if="wizardStore.step === userReviewStep" rounded="lg" variant="flat" color="primary" @click="handleSubmit">Submit</v-btn>
            <v-btn v-else rounded="lg" variant="flat" color="primary" @click="handleContinue">Continue</v-btn>
          </v-col>
        </v-row>
      </v-container>
    </template>
  </Wizard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import referenceWizardConfig from "@/config/reference-wizard";
import { useAlertStore } from "@/store/alert";
import { useWizardStore } from "@/store/wizard";

import Wizard from "../Wizard.vue";

export default defineComponent({
  name: "Reference",
  components: { Wizard },
  setup() {
    const wizardStore = useWizardStore();
    const alertStore = useAlertStore();
    return { alertStore, referenceWizardConfig, wizardStore };
  },
  data() {
    return {
      referenceType: "",
      applicantFirstName: "",
      applicantLastName: "",
    };
  },
  computed: {
    userDeclinedStep(): number {
      return this.wizardStore.steps.findIndex((step) => step.stage === "Decline") + 1;
    },
    userReviewStep(): number {
      return this.wizardStore.steps.findIndex((step) => step.stage === "Review") + 1;
    },
  },
  mounted() {
    //TODO: implement check here whether we are doing a character or work reference and load the correct config
    this.wizardStore.initializeWizardForReference(referenceWizardConfig);
    this.referenceType = this.wizardStore.wizardData?.referenceType || "empty";
    this.applicantFirstName = this.wizardStore.wizardData?.applicantFirstName || "empty";
    this.applicantLastName = this.wizardStore.wizardData?.applicantLastName || "empty";
  },
  methods: {
    async handleContinue() {
      const currentStepFormId = this.wizardStore.currentStep.form.id;
      const formRef = (this.$refs.wizard as typeof Wizard).$refs[currentStepFormId][0].$refs[currentStepFormId];
      const { valid } = await formRef.validate();

      if (!valid) {
        this.alertStore.setFailureAlert("Please fill out all required fields");
      } else {
        if (
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.declaration.form.inputs.declarationForm.id] === false &&
          this.wizardStore.step === 1
        ) {
          this.wizardStore.setStep(this.userDeclinedStep);
        } else {
          this.wizardStore.incrementStep();
        }
      }
    },
    handleBack() {
      if (this.wizardStore.step === this.userDeclinedStep) {
        this.wizardStore.setStep(1); //start back at beginning if user goes back from declining to be a reference
      } else {
        this.wizardStore.decrementStep();
      }
    },
    handleSubmit() {
      this.alertStore.setWarningAlert("Submit does not work yet");
    },
  },
});
</script>
