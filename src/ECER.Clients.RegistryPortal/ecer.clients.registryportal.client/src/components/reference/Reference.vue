<template>
  <Wizard
    :ref="'wizard'"
    :wizard="wizardStore.wizardData?.inviteType === PortalInviteType.WORK_EXPERIENCE ? workExperienceReferenceWizardConfig : characterReferenceWizardConfig"
    :show-steps="false"
  >
    <template #header>
      <v-container fluid class="bg-white">
        <h2>{{ inviteTypeTitle }}</h2>
        <div role="doc-subtitle">{{ `For applicant: ${wizardStore.wizardData.applicantFirstName} ${wizardStore.wizardData.applicantLastName}` }}</div>
        <v-btn v-if="wizardStore.step !== 1" variant="text" rounded="lg" color="primary" @click="handleBack">{{ "< back" }}</v-btn>
      </v-container>
    </template>
    <template #actions>
      <v-container class="mb-8">
        <v-row no-gutters>
          <v-col>
            <v-btn
              v-if="wizardStore.step === userDeclinedStep"
              :loading="loadingStore.isLoading('reference_optout')"
              rounded="lg"
              variant="flat"
              color="primary"
              @click="handleDecline"
            >
              Submit
            </v-btn>
            <v-btn v-else-if="wizardStore.step === userReviewStep" rounded="lg" variant="flat" color="primary" @click="handleSubmit">Submit</v-btn>
            <v-btn v-else rounded="lg" variant="flat" color="primary" @click="handleContinue">Continue</v-btn>
          </v-col>
        </v-row>
      </v-container>
    </template>
  </Wizard>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute } from "vue-router";

import { getReference, optOutReference } from "@/api/reference";
import characterReferenceWizardConfig from "@/config/character-reference-wizard";
import workExperienceReferenceWizardConfig from "@/config/work-experience-reference-wizard";
import { useAlertStore } from "@/store/alert";
import { useLoadingStore } from "@/store/loading";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { PortalInviteType } from "@/utils/constant";

import Wizard from "../Wizard.vue";

export default defineComponent({
  name: "Reference",
  components: { Wizard },
  async setup() {
    const route = useRoute();
    const { data } = await getReference(route.params.token as string);
    const wizardStore = useWizardStore();
    const loadingStore = useLoadingStore();
    const alertStore = useAlertStore();
    if (data?.portalInvitation?.inviteType === PortalInviteType.WORK_EXPERIENCE) {
      wizardStore.initializeWizardForReference(workExperienceReferenceWizardConfig, data.portalInvitation);
    } else if (data?.portalInvitation?.inviteType === PortalInviteType.CHARACTER) {
      wizardStore.initializeWizardForReference(characterReferenceWizardConfig, data.portalInvitation);
    }
    return { alertStore, workExperienceReferenceWizardConfig, characterReferenceWizardConfig, wizardStore, PortalInviteType, loadingStore };
  },
  computed: {
    userDeclinedStep(): number {
      return this.wizardStore.steps.findIndex((step) => step.stage === "Decline") + 1;
    },
    userReviewStep(): number {
      return this.wizardStore.steps.findIndex((step) => step.stage === "Review") + 1;
    },
    inviteTypeTitle(): string {
      return this.wizardStore.wizardData?.inviteType === PortalInviteType.WORK_EXPERIENCE ? "Work experience reference" : "Character reference";
    },
  },
  methods: {
    async handleContinue() {
      const currentStepFormId = this.wizardStore.currentStep.form.id;
      const formRef = (this.$refs.wizard as typeof Wizard).$refs[currentStepFormId][0].$refs[currentStepFormId];
      const { valid } = await formRef.validate();

      if (!valid) {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format to continue");
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
      this.alertStore.setWarningAlert("User Accepted");
    },
    async handleDecline() {
      const currentStepFormId = this.wizardStore.currentStep.form.id;
      const formRef = (this.$refs.wizard as typeof Wizard).$refs[currentStepFormId][0].$refs[currentStepFormId];
      const { valid } = await formRef.validate();

      if (!valid) {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format to continue");
        return;
      }

      const reason = this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.decline.form.inputs.referenceDecline.id];
      const result = await optOutReference(this.$route.params.token as string, reason as Components.Schemas.UnabletoProvideReferenceReasons);
      if (!result.error) {
        this.$router.push({ path: "/reference-submitted" });
      }
    },
  },
});
</script>
