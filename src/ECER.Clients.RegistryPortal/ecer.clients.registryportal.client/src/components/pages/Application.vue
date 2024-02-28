<template>
  <Wizard
    :wizard="applicationWizard"
    :wizard-data="wizardStore.wizardData"
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

    const userProfile = await getProfile();
    if (userProfile !== null) {
      wizardStore.initializeWizard(applicationWizard, {
        [applicationWizard.steps.declaration.form.inputs.applicantLegalName.id]: userStore.fullName,
        [applicationWizard.steps.declaration.form.inputs.signedDate.id]: new Date().toISOString().slice(0, 10),

        [applicationWizard.steps.profile.form.inputs.legalFirstName.id]: userProfile.firstName,
        [applicationWizard.steps.profile.form.inputs.legalLastName.id]: userProfile.lastName,
        [applicationWizard.steps.profile.form.inputs.dateOfBirth.id]: userProfile.dateOfBirth,
        [applicationWizard.steps.profile.form.inputs.addresses.id]: {
          [AddressType.RESIDENTIAL]: userProfile.residentialAddress || userStore.oidcAddress,
          [AddressType.MAILING]: userProfile.mailingAddress || userStore.oidcAddress,
        },
        [applicationWizard.steps.profile.form.inputs.email.id]: userProfile.email,
        [applicationWizard.steps.profile.form.inputs.legalMiddleName.id]: userProfile.middleName,
        [applicationWizard.steps.profile.form.inputs.preferredName.id]: userProfile.preferredName,
        [applicationWizard.steps.profile.form.inputs.alternateContactNumber.id]: userProfile.alternateContactPhone,
        [applicationWizard.steps.profile.form.inputs.primaryContactNumber.id]: userProfile.phone,
        [applicationWizard.steps.education.form.inputs.educationList.id]: [],
      });
    } else {
      wizardStore.initializeWizard(applicationWizard, {
        [applicationWizard.steps.declaration.form.inputs.applicantLegalName.id]: userStore.fullName,
        [applicationWizard.steps.declaration.form.inputs.signedDate.id]: new Date().toISOString().slice(0, 10),

        [applicationWizard.steps.profile.form.inputs.legalFirstName.id]: userStore.oidcUserInfo.firstName,
        [applicationWizard.steps.profile.form.inputs.legalLastName.id]: userStore.oidcUserInfo.lastName,
        [applicationWizard.steps.profile.form.inputs.dateOfBirth.id]: userStore.oidcUserInfo.dateOfBirth,
        [applicationWizard.steps.profile.form.inputs.addresses.id]: {
          [AddressType.RESIDENTIAL]: userStore.oidcAddress,
          [AddressType.MAILING]: userStore.oidcAddress,
        },
        [applicationWizard.steps.profile.form.inputs.email.id]: userStore.oidcUserInfo.email,
        [applicationWizard.steps.profile.form.inputs.primaryContactNumber.id]: userStore.oidcUserInfo.phone,
        [applicationWizard.steps.education.form.inputs.educationList.id]: [],
      });
    }

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
        switch (this.wizardStore.currentStepId) {
          case applicationWizard.steps.profile.id:
            this.saveProfile();
            break;
          case applicationWizard.steps.declaration.id:
            this.saveDeclaration();
            break;
          case applicationWizard.steps.education.id:
            this.saveEducation();
            break;
          default:
            this.wizardStore.incrementStep();
        }
      }
    },
    handleBack() {
      this.wizardStore.decrementStep();
    },
    async handleSaveAsDraft() {
      const applicationId = await createOrUpdateDraftApplication(this.applicationStore.currentApplication);
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
      this.applicationStore.currentApplication.stage = "Declaration";
      this.applicationStore.currentApplication.signedDate = this.wizardStore.wizardData[applicationWizard.steps.declaration.form.inputs.signedDate.id];
      const applicationId = await createOrUpdateDraftApplication(this.applicationStore.currentApplication);
      if (applicationId) {
        this.alertStore.setSuccessAlert("Declaration saved successfully.");
        this.wizardStore.incrementStep();
      } else {
        this.alertStore.setFailureAlert("Declaration save failed");
      }
    },
    async saveEducation() {
      this.applicationStore.currentApplication.stage = "Education";
      this.applicationStore.currentApplication.transcripts = this.wizardStore.wizardData[applicationWizard.steps.education.form.inputs.educationList.id];
      const applicationId = await createOrUpdateDraftApplication(this.applicationStore.currentApplication);
      if (applicationId) {
        this.alertStore.setSuccessAlert("Education saved successfully.");
        this.wizardStore.incrementStep();
      } else {
        this.alertStore.setFailureAlert("Education save failed");
      }
    },
  },
});
</script>
