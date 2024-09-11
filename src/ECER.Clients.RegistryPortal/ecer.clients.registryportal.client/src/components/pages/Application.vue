<template>
  <Wizard :ref="'wizard'" :wizard="wizardConfigSetup">
    <template #header>
      <WizardHeader
        class="mb-6"
        :handle-save-draft="handleSaveAsDraft"
        :show-save-button="showSaveButtons"
        :is-renewal="applicationStore?.draftApplication.applicationType === 'Renewal'"
        :is-registrant="userStore.isRegistrant"
      />
      <v-container>
        <!-- prettier-ignore -->
        <a v-if="$vuetify.display.mobile && wizardStore.step !== 1" href="#" @click.prevent="handleBack">
          <v-icon large>mdi-chevron-left</v-icon>Back to previous step
        </a>
      </v-container>
    </template>
    <template #stepperHeader>
      <v-container v-show="!$vuetify.display.mobile">
        <v-stepper-header class="elevation-0">
          <template v-for="(step, index) in Object.values(wizardStore.steps)" :key="step.stage">
            <v-stepper-item
              color="primary"
              :step="wizardStore.step"
              :value="index + 1"
              :title="step.title"
              :editable="index + 1 < wizardStore.step && wizardStore.listComponentMode !== 'add'"
              :complete="index + 1 < wizardStore.step"
            >
              <template #title>
                <a v-if="index + 1 < wizardStore.step && wizardStore.listComponentMode !== 'add'" href="#" @click.prevent>{{ step.title }}</a>
                <div v-else>{{ step.title }}</div>
              </template>
            </v-stepper-item>
            <v-divider v-if="index !== Object.values(wizardStore.steps).length - 1" :key="`divider-${index}`" />
          </template>
        </v-stepper-header>
      </v-container>
    </template>
    <template #PrintPreview>
      <v-btn rounded="lg" variant="text" @click="printPage()">
        <v-icon color="secondary" icon="mdi-printer-outline" class="mr-2"></v-icon>
        <a class="small">Print Preview</a>
      </v-btn>
    </template>
    <template #actions>
      <v-container>
        <v-btn v-if="showSaveButtons" rounded="lg" color="primary" @click="handleSaveAndContinue">Save and continue</v-btn>
        <v-btn v-if="showSubmitApplication" rounded="lg" color="primary" :loading="loadingStore.isLoading('application_post')" @click="handleSubmit">
          Submit Application
        </v-btn>
      </v-container>
    </template>
  </Wizard>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { defineComponent } from "vue";

import { getProfile, putProfile } from "@/api/profile";
import Wizard from "@/components/Wizard.vue";
import WizardHeader from "@/components/WizardHeader.vue";
import applicationWizardAssistantAndOneYear from "@/config/application-wizard-assistant-and-one-year";
import applicationWizardFiveYear from "@/config/application-wizard-five-year";
import applicationWizardRenewAssistant from "@/config/application-wizard-renew-assistant";
import applicationWizardRenewFiveYearActive from "@/config/application-wizard-renew-five-year-active";
import applicationWizardRenewFiveYearExpiredLessThan5Years from "@/config/application-wizard-renew-five-year-expired-less-than-five-years";
import applicationWizardRenewFiveYearExpiredMoreThan5Years from "@/config/application-wizard-renew-five-year-expired-more-than-five-years";
import applicationWizardRenewOneYearActive from "@/config/application-wizard-renew-one-year-active";
import applicationWizardRenewOneYearExpired from "@/config/application-wizard-renew-one-year-expired";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useCertificationStore } from "@/store/certification";
import { useLoadingStore } from "@/store/loading";
import { useUserStore } from "@/store/user";
import { useWizardStore } from "@/store/wizard";
import type { ApplicationStage, Wizard as WizardType } from "@/types/wizard";
import { AddressType } from "@/utils/constant";

export default defineComponent({
  name: "Application",
  components: { Wizard, WizardHeader },
  setup: async () => {
    const wizardStore = useWizardStore();
    const userStore = useUserStore();
    const alertStore = useAlertStore();
    const applicationStore = useApplicationStore();
    const loadingStore = useLoadingStore();
    const certificationStore = useCertificationStore();
    let wizardConfigSetup: WizardType | undefined = undefined;

    // Refresh userProfile from the server
    const userProfile = await getProfile();
    if (userProfile !== null) {
      userStore.setUserProfile(userProfile);
    }
    const latestCertificateIsExpired = (dt: string) => {
      return DateTime.fromISO(dt) > DateTime.fromISO(certificationStore?.latestCertification?.expiryDate!);
    };

    const latestCertificateExpiredMoreThan5Years = (dt1: string) => {
      const dt2 = DateTime.fromISO(certificationStore.latestCertification?.expiryDate!);
      const differenceInYears = Math.abs(DateTime.fromISO(dt1).diff(dt2, "years").years);
      return differenceInYears > 5;
    };

    const draftApplicationCreatedOn = applicationStore.draftApplication.createdOn!;

    if (applicationStore.isDraftApplicationRenewal) {
      if (applicationStore.isDraftCertificateTypeEceAssistant) {
        await wizardStore.initializeWizard(applicationWizardRenewAssistant, applicationStore.draftApplication);
        wizardConfigSetup = applicationWizardRenewAssistant;
      } else if (applicationStore.isDraftCertificateTypeOneYear) {
        {
          if (!latestCertificateIsExpired(draftApplicationCreatedOn)) {
            await wizardStore.initializeWizard(applicationWizardRenewOneYearActive, applicationStore.draftApplication);
            wizardConfigSetup = applicationWizardRenewOneYearActive;
          } else if (!latestCertificateExpiredMoreThan5Years(applicationStore.draftApplication.createdOn!)) {
            await wizardStore.initializeWizard(applicationWizardRenewOneYearExpired, applicationStore.draftApplication);
            wizardConfigSetup = applicationWizardRenewOneYearExpired;
          }
        }
      } else if (applicationStore.isDraftCertificateTypeFiveYears) {
        if (!latestCertificateIsExpired(draftApplicationCreatedOn)) {
          await wizardStore.initializeWizard(applicationWizardRenewFiveYearActive, applicationStore.draftApplication);
          wizardConfigSetup = applicationWizardRenewFiveYearActive;
        } else if (!latestCertificateExpiredMoreThan5Years(draftApplicationCreatedOn)) {
          await wizardStore.initializeWizard(applicationWizardRenewFiveYearExpiredLessThan5Years, applicationStore.draftApplication);
          wizardConfigSetup = applicationWizardRenewFiveYearExpiredLessThan5Years;
        } else if (latestCertificateExpiredMoreThan5Years(draftApplicationCreatedOn)) {
          await wizardStore.initializeWizard(applicationWizardRenewFiveYearExpiredMoreThan5Years, applicationStore.draftApplication);
          wizardConfigSetup = applicationWizardRenewFiveYearExpiredMoreThan5Years;
        }
      }
    } else {
      if (applicationStore.isDraftCertificateTypeFiveYears) {
        await wizardStore.initializeWizard(applicationWizardFiveYear, applicationStore.draftApplication);
        wizardConfigSetup = applicationWizardFiveYear;
      } else {
        await wizardStore.initializeWizard(applicationWizardAssistantAndOneYear, applicationStore.draftApplication);
        wizardConfigSetup = applicationWizardAssistantAndOneYear;
      }
    }

    return {
      applicationStore,
      wizardStore,
      alertStore,
      userStore,
      loadingStore,
      applicationWizardFiveYear,
      applicationWizardAssistantAndOneYear,
      certificationStore,
      wizardConfigSetup,
    };
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
        this.$router.push({ name: "submitted", params: { applicationId: submitApplicationResponse.applicationId } });
      }
    },
    async handleSaveAndContinue() {
      const currentStepFormId = this.wizardStore.currentStep.form.id;
      const formRef = (this.$refs.wizard as typeof Wizard).$refs[currentStepFormId][0].$refs[currentStepFormId];
      const { valid } = await formRef.validate();
      if (!valid) {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
      } else {
        switch (this.wizardStore.currentStepStage) {
          case "ContactInformation":
            this.saveProfile();
            this.incrementWizard();
            break;
          case "ExplanationLetter":
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
      this.applicationStore.draftApplication.stage = this.wizardStore.currentStepStage as ApplicationStage;
    },
    decrementWizard() {
      this.wizardStore.decrementStep();
      this.applicationStore.draftApplication.stage = this.wizardStore.currentStepStage as ApplicationStage;
    },
    handleBack() {
      switch (this.wizardStore.currentStepStage) {
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
          await this.saveProfile();
          break;
        default:
          await this.saveDraftAndAlertSuccess();
          break;
      }
    },
    async saveProfile() {
      const { error } = await putProfile({
        firstName: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.legalFirstName.id],
        middleName: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.legalMiddleName.id],
        preferredName: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.preferredName.id],
        lastName: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.legalLastName.id],
        dateOfBirth: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.dateOfBirth.id],
        residentialAddress: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.addresses.id][AddressType.RESIDENTIAL],
        mailingAddress: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.addresses.id][AddressType.MAILING],
        email: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.email.id],
        phone: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.primaryContactNumber.id],
        alternateContactPhone: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.alternateContactNumber.id],
      });

      if (!error) {
        this.alertStore.setSuccessAlert("Your responses have been saved. You may resume this application from your dashboard.");
        this.userStore.setUserInfo({
          firstName: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.legalFirstName.id],
          lastName: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.legalLastName.id],
          email: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.email.id],
          phone: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.primaryContactNumber.id],
          dateOfBirth: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.dateOfBirth.id],
        });
      }
    },
    printPage() {
      window.print();
    },
  },
});
</script>
