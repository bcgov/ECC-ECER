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

import { getProfile, putProfile } from "@/api/profile";
import Wizard from "@/components/Wizard.vue";
import applicationWizard from "@/config/application-wizard";
import { useAlertStore } from "@/store/alert";
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

    const userProfile = await getProfile();
    if (userProfile !== null) {
      wizardStore.initializeWizard(applicationWizard, {
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
        [applicationWizard.steps.education.form.inputs.educationList.id]: [
          {
            id: "1",
            school: "UVic",
            program: "Software Engineering (BSeng)",
            campusLocation: "Victoria",
            studentName: "Peter Parker",
            studentNumber: "1234",
            language: "English",
            startYear: "01-01-2009",
            endYear: "01-01-2013",
          },
          {
            id: "2",
            school: "University of Toronto",
            program: "Computer Science",
            campusLocation: "Toronto",
            studentName: "Bill Smith",
            studentNumber: "4321",
            language: "French",
            startYear: "01-01-2009",
            endYear: "01-01-2013",
          },
        ],
      });
    } else {
      wizardStore.initializeWizard(applicationWizard, {
        [applicationWizard.steps.profile.form.inputs.legalFirstName.id]: userStore.oidcUserInfo.firstName,
        [applicationWizard.steps.profile.form.inputs.legalLastName.id]: userStore.oidcUserInfo.lastName,
        [applicationWizard.steps.profile.form.inputs.dateOfBirth.id]: userStore.oidcUserInfo.dateOfBirth,
        [applicationWizard.steps.profile.form.inputs.addresses.id]: {
          [AddressType.RESIDENTIAL]: userStore.oidcAddress,
          [AddressType.MAILING]: userStore.oidcAddress,
        },
        [applicationWizard.steps.profile.form.inputs.email.id]: userStore.oidcUserInfo.email,
        [applicationWizard.steps.profile.form.inputs.primaryContactNumber.id]: userStore.oidcUserInfo.phone,
        [applicationWizard.steps.education.form.inputs.educationList.id]: [
          {
            id: "1",
            school: "UVic",
            program: "Software Engineering (BSeng)",
            campusLocation: "Victoria",
            studentName: "Peter Parker",
            studentNumber: "1234",
            language: "English",
            startYear: "01-01-2009",
            endYear: "01-01-2013",
          },
        ],
      });
    }

    return { applicationWizard, wizardStore, alertStore, userStore };
  },
  data: () => ({
    isFormValid: null as boolean | null,
  }),
  methods: {
    handleSaveAndContinue() {
      this.saveProfile();
    },
    handleBack() {
      this.wizardStore.decrementStep();
    },
    handleSaveAsDraft() {
      this.alertStore.setSuccessAlert("Save as Draft");
    },
    async saveProfile() {
      if (!this.isFormValid) {
        this.alertStore.setFailureAlert("Please fill out all required fields");
      } else {
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
      }
    },
  },
});
</script>
