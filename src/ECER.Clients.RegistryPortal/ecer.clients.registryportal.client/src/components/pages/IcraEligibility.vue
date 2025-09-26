<template>
  <Wizard :ref="'wizard'" :wizard="applicationWizardIcraEligibility">
    <template #header>
      <WizardHeader class="mb-6" :handle-save-draft="handleSaveAndExit" :validate-form="validateForm" :is-icra-eligibility="true" />
      <v-container>
        <!-- prettier-ignore -->
        <a v-if="$vuetify.display.mobile && wizardStore.step !== 1 && showSaveButtons" href="#" @click.prevent="handleBack">
          <v-icon large>mdi-chevron-left</v-icon>Back to previous step
        </a>
      </v-container>
    </template>
    <template #stepperHeader>
      <v-container v-show="showSteps">
        <v-stepper-header class="elevation-0">
          <template v-for="(step, index) in Object.values(wizardStore.steps)" :key="step.stage">
            <v-stepper-item
              color="primary"
              :step="wizardStore.step"
              :value="index + 1"
              :title="step.title"
              :editable="index + 1 < wizardStore.step && wizardStore.listComponentMode !== 'add'"
              :complete="index + 1 < wizardStore.step"
              :class="`small ${mdAndDown ? 'text-wrap' : 'text-no-wrap'}`"
            >
              <template #title>
                <a v-if="index + 1 < wizardStore.step" href="#" @click.prevent>{{ step.title }}</a>
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
      <v-window class="my-n10">
        <v-stepper-window>
          <v-container>
            <v-row>
              <v-col>
                <v-btn
                  id="btnSaveAndContinue"
                  v-if="showSaveButtons"
                  :loading="loadingStore.isLoading('icra_put') || loadingStore.isLoading('profile_put') || loadingStore.isLoading('profile_get')"
                  rounded="lg"
                  color="primary"
                  @click="handleSaveAndContinue"
                >
                  Save and continue
                </v-btn>
                <v-btn
                  id="btnSubmitEligibilitySubmission"
                  v-if="showSubmitEligibilitySubmission"
                  rounded="lg"
                  color="primary"
                  :loading="loadingStore.isLoading('icra_put') || loadingStore.isLoading('icra_post')"
                  @click="handleSubmit"
                >
                  Submit eligibility submission
                </v-btn>
              </v-col>
            </v-row>
          </v-container>
        </v-stepper-window>
      </v-window>
    </template>
  </Wizard>
</template>

<script lang="ts">
import { mapWritableState } from "pinia";
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";

import { getProfile, putProfile } from "@/api/profile";
import Wizard from "@/components/Wizard.vue";
import WizardHeader from "@/components/WizardHeader.vue";
import applicationWizardIcraEligibility from "@/config/application-wizard-icra-eligibility";
import { useAlertStore } from "@/store/alert";
import { useLoadingStore } from "@/store/loading";
import { useUserStore } from "@/store/user";
import { useWizardStore } from "@/store/wizard";
import { useIcraStore } from "@/store/icra";
import type { IcraEligibilityStage } from "@/types/wizard";
import { AddressType } from "@/utils/constant";

import { useRouter } from "vue-router";

export default defineComponent({
  name: "IcraEligibility",
  components: { Wizard, WizardHeader },
  setup: async () => {
    const wizardStore = useWizardStore();
    const userStore = useUserStore();
    const alertStore = useAlertStore();
    const icraStore = useIcraStore();
    const loadingStore = useLoadingStore();
    const { mdAndDown, mobile } = useDisplay();
    const router = useRouter();

    // Refresh userProfile from the server
    const userProfile = await getProfile();
    if (userProfile !== null) {
      userStore.setUserProfile(userProfile);
    }

    //Initialize wizard
    await wizardStore.initializeWizardForIcraEligibility(applicationWizardIcraEligibility, icraStore.draftIcraEligibility);

    return {
      icraStore,
      wizardStore,
      alertStore,
      userStore,
      loadingStore,
      applicationWizardIcraEligibility,
      mdAndDown,
      mobile,
      router,
    };
  },
  computed: {
    ...mapWritableState(useWizardStore, { mode: "listComponentMode" }),
    showSaveButtons() {
      return (
        this.wizardStore.currentStepStage !== "Review" &&
        !(this.wizardStore.currentStepStage === "EmploymentExperience" && this.wizardStore.listComponentMode === "add") &&
        !(this.wizardStore.currentStepStage === "InternationalCertification" && this.wizardStore.listComponentMode === "add")
      );
    },
    showSubmitEligibilitySubmission() {
      return this.wizardStore.currentStepStage === "Review";
    },
    showSteps() {
      return !this.mobile && this.wizardStore.listComponentMode !== "add";
    },
  },
  mounted() {
    if (this.icraStore.draftIcraEligibility.signedDate === null) {
      console.warn("user entered into /application route without a signedDate or certificationType");
      this.router.push("/");
    }
    this.mode = "list";
  },
  methods: {
    async handleSubmit() {
      const submitIcraEligibilityResponse = await this.icraStore.submitIcraEligibilityApplication();

      if (submitIcraEligibilityResponse?.eligibility) {
        if (submitIcraEligibilityResponse !== null && submitIcraEligibilityResponse !== undefined && submitIcraEligibilityResponse.eligibility) {
          this.icraStore.icraEligibility = submitIcraEligibilityResponse.eligibility;
        }
        this.router.push({ name: "icra-eligibility-submitted", params: { icraEligibilityId: submitIcraEligibilityResponse.eligibility.id } });
      }
    },
    async handleSaveAndContinue() {
      const valid = await this.validateForm();
      if (!valid) {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
      } else {
        switch (this.wizardStore.currentStepStage) {
          case "ContactInformation":
            await this.saveProfile(false);
            this.incrementWizard();
            break;
          case "InternationalCertification":
          case "EmploymentExperience":
            await this.saveDraftAndAlertSuccess(false);
            this.incrementWizard();
            break;
          case "Review":
            this.saveDraftAndAlertSuccess(false);
            this.incrementWizard();
            break;
        }
      }
    },
    async validateForm() {
      const currentStepFormId = this.wizardStore.currentStep.form.id;
      const formRef = (this.$refs.wizard as typeof Wizard).$refs[currentStepFormId][0].$refs[currentStepFormId];
      const { valid } = await formRef.validate();

      return valid;
    },
    incrementWizard() {
      this.wizardStore.incrementStep();
      this.icraStore.draftIcraEligibility.portalStage = this.wizardStore.currentStepStage as IcraEligibilityStage;
    },
    decrementWizard() {
      this.wizardStore.decrementStep();
      this.icraStore.draftIcraEligibility.portalStage = this.wizardStore.currentStepStage as IcraEligibilityStage;
    },
    handleBack() {
      switch (this.wizardStore.currentStepStage) {
        default:
          this.icraStore.saveDraft();
          this.decrementWizard();
          break;
      }
    },
    async saveDraftAndAlertSuccess(exit: boolean) {
      const draftIcraEligibilityResponse = await this.icraStore.saveDraft();
      if (draftIcraEligibilityResponse?.eligibility) {
        let message = "Information saved. If you save and exit, you can resume your eligibility submission later.";
        if (exit) message = "Information saved. You can resume your eligibility submission later.";
        this.alertStore.setSuccessAlert(message);
      }
    },
    async handleSaveAndExit() {
      await this.handleSaveAsDraft(true);
    },
    async handleSaveAsDraft(exit: boolean) {
      switch (this.wizardStore.currentStepStage) {
        case "ContactInformation":
          await this.saveProfile(exit);
          break;
        default:
          await this.saveDraftAndAlertSuccess(exit);
          break;
      }
    },
    async saveProfile(exit: boolean) {
      const { error } = await putProfile({
        firstName: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.legalFirstName?.id || ""],
        middleName: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.legalMiddleName?.id || ""],
        preferredName: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.preferredName?.id || ""],
        lastName: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.legalLastName?.id || ""],
        dateOfBirth: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.dateOfBirth?.id || ""],
        residentialAddress:
          this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.addresses?.id || ""][AddressType.RESIDENTIAL],
        mailingAddress: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.addresses?.id || ""][AddressType.MAILING],
        email: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.email?.id || ""],
        phone: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.primaryContactNumber?.id || ""],
        alternateContactPhone: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.alternateContactNumber?.id || ""],
      });

      if (!error) {
        let message = "Information saved. If you save and exit, you can resume your application later.";
        if (exit) message = "Information saved. You can resume your application later.";
        this.alertStore.setSuccessAlert(message);

        this.userStore.setUserInfo({
          firstName: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.legalFirstName?.id || ""],
          lastName: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.legalLastName?.id || ""],
          email: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.email?.id || ""],
          phone: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.primaryContactNumber?.id || ""],
          dateOfBirth: this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.profile?.form?.inputs?.dateOfBirth?.id || ""],
        });

        //we should get the latest from getProfile and update the wizard. In case the wizard refreshes with stale profile data.
        const userProfile = await getProfile();
        if (userProfile !== null) {
          this.userStore.setUserProfile(userProfile);
        }
      }
    },
    printPage() {
      window.print();
    },
  },
});
</script>
