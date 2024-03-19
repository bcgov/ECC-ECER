<template>
  <Wizard :wizard="applicationWizard" @updated-validation="isFormValid = $event">
    <template #actions>
      <v-container class="mb-8">
        <v-row class="justify-space-between ga-4" no-gutters>
          <v-col cols="auto" class="mr-auto">
            <v-btn
              :disabled="wizardStore.step === 1 && certificationTypeStore.mode === 'selection'"
              rounded="lg"
              variant="outlined"
              color="primary"
              aut
              @click="handleBack"
            >
              Back
            </v-btn>
          </v-col>
          <v-col cols="auto">
            <v-btn rounded="lg" variant="outlined" color="primary" class="mr-4" primary @click="handleSaveAsDraft">Save as Draft</v-btn>
            <v-btn type="submit" :form="getFormId" rounded="lg" color="primary" :disabled="isDisabled" @click="handleSaveAndContinue">Save and Continue</v-btn>
          </v-col>
        </v-row>
      </v-container>
    </template>
  </Wizard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { getProfile, putProfile } from "@/api/profile";
import Wizard from "@/components/Wizard.vue";
import applicationWizard from "@/config/application-wizard";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useCertificationTypeStore } from "@/store/certificationType";
import { useUserStore } from "@/store/user";
import { useWizardStore } from "@/store/wizard";

import { AddressType } from "../inputs/EceAddresses.vue";

export default defineComponent({
  name: "Application",
  components: { Wizard },
  setup: async () => {
    const wizardStore = useWizardStore();
    const userStore = useUserStore();
    const alertStore = useAlertStore();
    const applicationStore = useApplicationStore();
    const certificationTypeStore = useCertificationTypeStore();

    // Refresh userProfile from the server
    const userProfile = await getProfile();
    if (userProfile !== null) {
      userStore.setUserProfile(userProfile);
    }

    certificationTypeStore.$reset();
    wizardStore.initializeWizard(applicationWizard, applicationStore.draftApplication);

    return { applicationWizard, applicationStore, wizardStore, alertStore, userStore, certificationTypeStore };
  },
  data: () => ({
    isFormValid: null as boolean | null,
  }),
  computed: {
    getFormId(): string {
      return this.wizardStore.currentStep.form.id;
    },
    isDisabled(): boolean {
      return this.wizardStore.currentStepStage === "Declaration" && !this.isFormValid;
    },
  },
  methods: {
    handleSaveAndContinue() {
      if (!this.isFormValid) {
        this.alertStore.setFailureAlert("Please fill out all required fields");
      } else {
        switch (this.wizardStore.currentStepStage) {
          case "CertificationType":
            if (this.certificationTypeStore.mode == "selection") {
              this.certificationTypeStore.mode = "terms";
            } else {
              this.saveDraftAndAlertSuccess();
              this.incrementWizard();
            }
            break;
          case "ContactInformation":
            this.saveProfile();
            this.incrementWizard();
            break;
          case "Declaration":
          case "Education":
          case "WorkReferences":
          case "CharacterReferences":
          case "Review":
            this.saveDraftAndAlertSuccess();
            this.incrementWizard();
            break;
        }
      }
    },
    incrementWizard() {
      this.wizardStore.incrementStep();
      this.applicationStore.draftApplication.stage = this.wizardStore.currentStepStage;
    },
    decrementWizard() {
      this.wizardStore.decrementStep();
      this.applicationStore.draftApplication.stage = this.wizardStore.currentStepStage;
    },
    handleBack() {
      switch (this.wizardStore.currentStepStage) {
        case "CertificationType":
          if (this.certificationTypeStore.mode == "terms") {
            this.certificationTypeStore.mode = "selection";
          }
          break;
        default:
          this.applicationStore.saveDraft();
          this.decrementWizard();
          this.isFormValid = true;
          break;
      }
    },
    async saveDraftAndAlertSuccess() {
      const draftApplicationResponse = await this.applicationStore.saveDraft();
      if (draftApplicationResponse?.applicationId) {
        this.alertStore.setSuccessAlert("Your responses have been saved. You may resume this application from your dashboard.");
      }
    },
    async handleSaveAsDraft() {
      switch (this.wizardStore.currentStepStage) {
        case "ContactInformation":
          this.saveProfile();
          break;
        default:
          this.saveDraftAndAlertSuccess();
          break;
      }
    },
    async saveProfile() {
      const success = await putProfile({
        firstName: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.legalFirstName.id],
        middleName: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.legalMiddleName.id],
        preferredName: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.preferredName.id],
        lastName: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.legalLastName.id],
        dateOfBirth: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.dateOfBirth.id],
        residentialAddress: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.addresses.id][AddressType.RESIDENTIAL],
        mailingAddress: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.addresses.id][AddressType.MAILING],
        email: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.email.id],
        phone: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.primaryContactNumber.id],
        alternateContactPhone: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.alternateContactNumber.id],
      });

      if (success) {
        this.alertStore.setSuccessAlert("Your responses have been saved. You may resume this application from your dashboard.");
        this.userStore.setUserInfo({
          firstName: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.legalFirstName.id],
          lastName: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.legalLastName.id],
          email: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.email.id],
          phone: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.primaryContactNumber.id],
          dateOfBirth: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.dateOfBirth.id],
        });
      }
    },
  },
});
</script>
