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

import { createOrUpdateDraftApplication } from "@/api/application";
import { getProfile, putProfile } from "@/api/profile";
import Wizard from "@/components/Wizard.vue";
import applicationWizard from "@/config/application-wizard";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
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

    // Refresh userProfile from the server
    const userProfile = await getProfile();
    if (userProfile !== null) {
      userStore.setUserProfile(userProfile);
    }

    wizardStore.initializeWizard(applicationWizard, applicationStore.draftApplication);

    return { applicationWizard, applicationStore, wizardStore, alertStore, userStore };
  },
  data: () => ({
    isFormValid: null as boolean | null,
  }),
  methods: {
    handleSaveAndContinue() {
      if (!this.isFormValid) {
        this.alertStore.setFailureAlert("Please fill out all required fields");
      } else {
        switch (this.wizardStore.currentStage) {
          case "ContactInformation":
            this.saveProfile();
            break;
          case "Declaration":
          case "Review":
          case "CertificationType":
          case "Education":
          case "WorkReferences":
          case "CharacterReferences":
            this.saveDeclaration();
            break;
          default:
            this.wizardStore.incrementStep();
        }
      }
    },
    handleBack() {
      this.wizardStore.decrementStep();
      this.isFormValid = true;
    },
    async handleSaveAsDraft() {
      this.applicationStore.draftApplication.stage = "ContactInformation";
      const applicationId = await createOrUpdateDraftApplication(this.applicationStore.draftApplication);
      if (applicationId) {
        this.alertStore.setSuccessAlert("Your responses have been saved. You may resume this application from your dashboard.");
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
        this.wizardStore.incrementStep();
      } else {
        this.alertStore.setFailureAlert("Profile save failed");
      }
    },
    async saveDeclaration() {
      //TODO ECER-812 add in logic that if the application is at a further step that we do not overwrite the application stage.
      this.applicationStore.draftApplication.stage = "Declaration";
      const applicationId = await createOrUpdateDraftApplication({
        signedDate: this.wizardStore.wizardData[applicationWizard.steps.declaration.form.inputs.signedDate.id],
      });
      if (applicationId) {
        this.alertStore.setSuccessAlert("Declaration saved successfully.");
        this.wizardStore.incrementStep();
      } else {
        this.alertStore.setFailureAlert("Declaration save failed");
      }
    },
  },
});
</script>
