<template>
  <Wizard
    :ref="'wizard'"
    :wizard="wizardStore.wizardData?.inviteType === PortalInviteType.WORK_EXPERIENCE ? workExperienceReferenceWizardConfig : characterReferenceWizardConfig"
    :show-steps="false"
  >
    <template #header>
      <v-container class="bg-white">
        <h1>{{ inviteTypeTitle }}</h1>
        <div role="doc-subtitle">{{ `For applicant: ${wizardStore.wizardData.applicantFirstName} ${wizardStore.wizardData.applicantLastName}` }}</div>

        <v-btn v-if="wizardStore.step !== 1" slim variant="text" rounded="lg" color="primary" @click="handleBack">
          <v-icon size="x-large" icon="mdi-chevron-left" />
          Back
        </v-btn>
      </v-container>
    </template>
    <template #PrintPreview>
      <v-btn rounded="lg" variant="text" @click="printPage">
        <v-icon color="secondary" icon="mdi-printer-outline" class="mr-2"></v-icon>
        <a class="small">Print Preview</a>
      </v-btn>
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
            <v-btn
              v-else-if="wizardStore.step === userReviewStep"
              :loading="
                wizardStore.wizardData.inviteType === PortalInviteType.CHARACTER
                  ? loadingStore.isLoading('character_reference_post')
                  : loadingStore.isLoading('workExperience_reference_post')
              "
              rounded="lg"
              variant="flat"
              color="primary"
              @click="handleSubmit"
            >
              Submit Reference
            </v-btn>
            <v-btn v-else rounded="lg" variant="flat" color="primary" @click="handleContinue">Continue</v-btn>
          </v-col>
        </v-row>
      </v-container>
    </template>
  </Wizard>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute, useRouter } from "vue-router";

import { getReference, optOutReference, postCharacterReference, postWorkExperienceReference } from "@/api/reference";
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
    const router = useRouter();
    const { data, error } = await getReference(route.params.token as string);

    if (error) {
      router.push("/invalid-reference");
    }

    const wizardStore = useWizardStore();
    const loadingStore = useLoadingStore();
    const alertStore = useAlertStore();
    if (data?.portalInvitation?.inviteType === PortalInviteType.WORK_EXPERIENCE) {
      wizardStore.initializeWizardForWorkExReference(workExperienceReferenceWizardConfig, data.portalInvitation);
    } else if (data?.portalInvitation?.inviteType === PortalInviteType.CHARACTER) {
      wizardStore.initializeWizardForCharacterReference(characterReferenceWizardConfig, data.portalInvitation);
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
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.declaration.form.inputs.willProvideReference.id] === false &&
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
    async handleSubmit() {
      const currentStepFormId = this.wizardStore.currentStep.form.id;
      const formRef = (this.$refs.wizard as typeof Wizard).$refs[currentStepFormId][0].$refs[currentStepFormId];
      const { valid } = await formRef.validate();

      if (!valid) {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format to continue");
        return;
      }

      if (this.wizardStore.wizardData.inviteType === PortalInviteType.CHARACTER) {
        const response = await postCharacterReference({
          token: this.$route.params.token as string,
          willProvideReference: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.declaration.form.inputs.willProvideReference.id],
          referenceContactInformation:
            this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.contactInformation.form.inputs.referenceContactInformation.id],
          referenceEvaluation: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id],
          confirmProvidedInformationIsRight:
            this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.review.form.inputs.confirmProvidedInformationIsRight.id],
          recaptchaToken: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.review.form.inputs.recaptchaToken.id],
        });

        if (!response?.error) {
          this.$router.push({ path: "/reference-submitted" });
        }
      } else if (this.wizardStore.wizardData.inviteType === PortalInviteType.WORK_EXPERIENCE) {
        const response = await postWorkExperienceReference({
          token: this.$route.params.token as string,
          willProvideReference: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.declaration.form.inputs.willProvideReference.id],
          referenceContactInformation:
            this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.contactInformation.form.inputs.referenceContactInformation.id],
          workExperienceReferenceDetails:
            this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id],
          workExperienceReferenceCompetenciesAssessment:
            this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.assessment.form.inputs.workExperienceAssessment.id],
          confirmProvidedInformationIsRight:
            this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.review.form.inputs.confirmProvidedInformationIsRight.id],
          recaptchaToken: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.review.form.inputs.recaptchaToken.id],
        });
        if (!response?.error) {
          this.$router.push({ path: "/reference-submitted" });
        }
      }
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
      const recaptchaToken = this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.decline.form.inputs.recaptchaToken.id];
      const result = await optOutReference(this.$route.params.token as string, reason as Components.Schemas.UnabletoProvideReferenceReasons, recaptchaToken);
      if (!result.error) {
        this.$router.push({ path: "/reference-submitted" });
      }
    },
    async printPage() {
      const currentStepFormId = this.wizardStore.currentStep.form.id;
      const formRef = (this.$refs.wizard as typeof Wizard).$refs[currentStepFormId][0].$refs[currentStepFormId];
      const { valid } = await formRef.validate();

      if (!valid) {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format before printing");
      } else {
        window.print();
      }
    },
  },
});
</script>
