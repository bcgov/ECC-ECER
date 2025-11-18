<template>
  <v-container fluid class="bg-primary">
    <v-container>
      <v-row>
        <v-col :class="[mobile ? 'flex-column-reverse' : 'justify-space-between', 'd-flex']">
          <div>
            <div v-if="isIcraEligibility" class="text-white text-h5 font-weight-bold">Apply with international certification</div>
            <ApplicationCertificationTypeHeader
              v-else
              :is-renewal="isRenewal"
              :is-labor-mobility="isLaborMobility"
              :certification-types="applicationStore.draftApplication.certificationTypes ?? []"
            />
            <a
              v-if="!isRenewal && !isRegistrant && !isLaborMobility && !isIcraEligibility && !applicationStore.isDraftApplicationIcra"
              href="#"
              class="text-white"
              @click.prevent="toggleChangeCertificationConfirmation"
            >
              Change certification type
            </a>
          </div>
          <div :class="[{ ['text-right mb-2']: mobile }]">
            <v-btn
              id="btnSaveAndExit"
              variant="outlined"
              :loading="loadingStore.isLoading(isIcraEligibility ? 'icra_put' : 'draftapplication_put')"
              @click="saveAndExit"
            >
              Save and exit
            </v-btn>
          </div>
        </v-col>
      </v-row>
    </v-container>
    <ConfirmationDialog
      :show="showConfirmation"
      title="Are you sure you want to change the type?"
      accept-button-text="Change type"
      @accept="changeCertification"
      @cancel="toggleChangeCertificationConfirmation"
    >
      <template #confirmation-text>
        <div class="pb-3">When you change the type of certification you're applying for</div>
        <ul class="ml-10">
          <li>It will save the data you've entered</li>
          <li>It may change the type and amount of information you need to provide</li>
        </ul>
      </template>
    </ConfirmationDialog>
    <ConfirmationDialog
      :show="showSaveExitConfirmation"
      :title="wizardStore.listComponentMode === 'add' ? 'Your information will not be saved' : 'Missing or invalid information'"
      accept-button-text="Discard changes"
      cancel-button-text="Continue editing"
      @accept="goToDashboard"
      @cancel="toggleSaveExitConfirmation"
    >
      <template #confirmation-text>
        <p class="pb-3">
          {{
            wizardStore.listComponentMode === "add"
              ? "You must complete this entry to save your changes."
              : "To save your changes, you must enter all required fields in a valid format."
          }}
        </p>
        <p>If you discard your changes, no information on this page will be saved.</p>
      </template>
    </ConfirmationDialog>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";

import { useApplicationStore } from "@/store/application";
import { useLoadingStore } from "@/store/loading";

import ApplicationCertificationTypeHeader from "./ApplicationCertificationTypeHeader.vue";
import ConfirmationDialog from "./ConfirmationDialog.vue";
import { useRouter } from "vue-router";
import { useWizardStore } from "@/store/wizard";

export default defineComponent({
  name: "WizardHeader",
  components: { ApplicationCertificationTypeHeader, ConfirmationDialog },
  props: {
    handleSaveDraft: {
      type: Function,
      required: true,
    },
    isRenewal: {
      type: Boolean,
      default: false,
    },
    isLaborMobility: {
      type: Boolean,
      default: false,
    },
    isRegistrant: {
      type: Boolean,
      default: false,
    },
    isIcraEligibility: {
      type: Boolean,
      default: false,
    },
    validateForm: {
      type: Function,
      required: true,
    },
  },
  setup() {
    const applicationStore = useApplicationStore();
    const loadingStore = useLoadingStore();
    const wizardStore = useWizardStore();
    const { mobile } = useDisplay();
    const router = useRouter();

    return {
      applicationStore,
      loadingStore,
      wizardStore,
      mobile,
      router,
    };
  },
  data: () => ({
    items: [
      {
        title: "Home",
        disabled: false,
        href: "/",
      },

      {
        title: "Apply New Certification",
        disabled: true,
        href: "application",
      },
    ],
    showConfirmation: false,
    showSaveExitConfirmation: false,
  }),
  methods: {
    toggleChangeCertificationConfirmation() {
      this.showConfirmation = !this.showConfirmation;
    },
    toggleSaveExitConfirmation() {
      this.showSaveExitConfirmation = !this.showSaveExitConfirmation;
    },
    async saveAndExit() {
      const valid = await this.validateForm();
      const step = this.wizardStore.currentStep.stage;

      //if user is on the list screen for these ones, they should be able to save and exit while saving what they currently have
      if (!valid && step !== "Education" && step !== "WorkReferences" && step !== "ProfessionalDevelopment") {
        this.showSaveExitConfirmation = true;
      } else if (this.wizardStore.listComponentMode === "add") {
        this.showSaveExitConfirmation = true;
      } else {
        await this.handleSaveDraft();
        this.goToDashboard();
      }
    },
    async changeCertification() {
      this.showConfirmation = false;
      this.router.push({ name: "application-certification" });
    },
    goToDashboard() {
      this.showSaveExitConfirmation = false; //prevents issue where router will stop responding
      this.router.push({ name: "dashboard" });
    },
  },
});
</script>
