<template>
  <Wizard :ref="'wizard'" :wizard="applicationWizard">
    <template #header>
      <WizardHeader class="mb-6" />
    </template>
    <template #stepperHeader>
      <v-stepper-header>
        <template v-for="(step, index) in Object.values(wizardStore.steps)" :key="step.stage">
          <v-stepper-item
            color="primary"
            :step="wizardStore.step"
            :value="index + 1"
            :title="step.title"
            :rules="wizardStore.step <= index + 1 ? [] : [() => wizardStore.validationState[step.stage as Components.Schemas.PortalStage]]"
          ></v-stepper-item>
          <v-divider v-if="index !== Object.values(wizardStore.steps).length - 1" :key="`divider-${index}`" />
        </template>
      </v-stepper-header>
    </template>
    <template #PrintPreview>
      <ConfirmationDialog
        :config="{
          cancelButtonText: 'Cancel',
          acceptButtonText: 'Yes',
          title: 'Print Confirmation',
          customButtonVariant: 'text',
          isDialogDisabled: wizardStore.allStageValidations,
        }"
        @accept="printPage"
      >
        <template #activator>
          <span @click="wizardStore.allStageValidations ? printPage() : {}">
            <v-icon color="secondary" icon="mdi-printer-outline" class="mr-2"></v-icon>
            <a class="small">Print Preview</a>
          </span>
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
    </template>
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
            <v-btn v-if="showSaveButtons" rounded="lg" variant="outlined" color="primary" class="mr-4" primary @click="handleSaveAsDraft">Save as Draft</v-btn>
            <v-btn v-if="showSaveButtons" rounded="lg" color="primary" @click="handleSaveAndContinue">Save and Continue</v-btn>
            <v-btn v-if="showSubmitApplication" rounded="lg" color="primary" :loading="loadingStore.isLoading('application_post')" @click="handleSubmit">
              Submit Application
            </v-btn>
          </v-col>
        </v-row>
      </v-container>
    </template>
  </Wizard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { getProfile, putProfile } from "@/api/profile";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import Wizard from "@/components/Wizard.vue";
import WizardHeader from "@/components/WizardHeader.vue";
import applicationWizard from "@/config/application-wizard";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useCertificationTypeStore } from "@/store/certificationType";
import { useLoadingStore } from "@/store/loading";
import { useUserStore } from "@/store/user";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";

import { AddressType } from "../inputs/EceAddresses.vue";

export default defineComponent({
  name: "Application",
  components: { Wizard, WizardHeader, ConfirmationDialog },
  setup: async () => {
    const wizardStore = useWizardStore();
    const userStore = useUserStore();
    const alertStore = useAlertStore();
    const applicationStore = useApplicationStore();
    const certificationTypeStore = useCertificationTypeStore();
    const loadingStore = useLoadingStore();

    // Refresh userProfile from the server
    const userProfile = await getProfile();
    if (userProfile !== null) {
      userStore.setUserProfile(userProfile);
    }

    certificationTypeStore.$reset();
    wizardStore.initializeWizard(applicationWizard, applicationStore.draftApplication);

    return { applicationWizard, applicationStore, wizardStore, alertStore, userStore, certificationTypeStore, loadingStore };
  },
  computed: {
    showSaveButtons() {
      return (
        this.wizardStore.currentStepStage !== "Review" &&
        !(this.wizardStore.currentStepStage === "Education" && this.wizardStore.listComponentMode === "add") &&
        !(this.wizardStore.currentStepStage === "WorkReferences" && this.wizardStore.listComponentMode === "add")
      );
    },
    showSubmitApplication() {
      return this.wizardStore.currentStepStage === "Review";
    },
  },
  methods: {
    async handleSubmit() {
      const submitApplicationResponse = await this.applicationStore.submitApplication();
      if (submitApplicationResponse?.applicationId) {
        this.$router.push({ path: "/submitted" });
      } else {
        this.alertStore.setFailureAlert("Your application is incomplete. You need to complete it before you can submit.");
      }
    },
    async handleSaveAndContinue() {
      const currentStepFormId = this.wizardStore.currentStep.form.id;
      const formRef = (this.$refs.wizard as typeof Wizard).$refs[currentStepFormId][0].$refs[currentStepFormId];
      const { valid } = await formRef.validate();
      if (!valid) {
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
      this.applicationStore.draftApplication.stage = this.wizardStore.currentStepStage as Components.Schemas.PortalStage;
    },
    decrementWizard() {
      this.wizardStore.decrementStep();
      this.applicationStore.draftApplication.stage = this.wizardStore.currentStepStage as Components.Schemas.PortalStage;
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
          break;
      }
    },
    async submitApplication() {
      const draftApplicationResponse = await this.applicationStore.saveDraft();
      if (draftApplicationResponse?.applicationId) {
        this.alertStore.setSuccessAlert("Draft application saved successfully");
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
    printPage() {
      window.print();
    },
  },
});
</script>
