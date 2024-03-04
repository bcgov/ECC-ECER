<template>
  <Wizard
    :wizard="applicationWizard"
    @save-and-continue="handleSaveAndContinue"
    @save-as-draft="handleSaveAsDraft"
    @back="handleBack"
    @updated-validation="isFormValid = $event"
  />
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
              this.saveDraft();
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
            this.saveDraft();
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
          this.saveDraft();
          this.decrementWizard();
          this.isFormValid = true;
          break;
      }
    },
    async handleSaveAsDraft() {
      switch (this.wizardStore.currentStepStage) {
        case "ContactInformation":
          this.saveProfile();
          break;
        default:
          this.saveDraft();
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
        this.alertStore.setSuccessAlert("Profile saved successfully");
        this.userStore.setUserInfo({
          firstName: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.legalFirstName.id],
          lastName: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.legalLastName.id],
          email: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.email.id],
          phone: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.primaryContactNumber.id],
          dateOfBirth: this.wizardStore.wizardData[applicationWizard.steps.profile.form.inputs.dateOfBirth.id],
        });
      } else {
        this.alertStore.setFailureAlert("Profile save failed");
      }
    },
    async saveDraft() {
      this.applicationStore.prepareDraftApplicationFromWizard();
      this.applicationStore.upsertDraftApplication();
    },
  },
});
</script>
