<template>
  <v-container fluid class="bg-primary">
    <v-container>
      <v-row>
        <v-col
          :class="[
            mobile ? 'flex-column' : 'justify-space-between',
            'd-flex',
            'align-center',
          ]"
        >
          <slot name="title"></slot>
          <div :class="[{ ['text-right mb-2']: mobile }]">
            <v-btn
              id="btnSaveAndExit"
              variant="outlined"
              :loading="
                loadingStore.isLoading('draftprogram_put') ||
                loadingStore.isLoading('course_put')
              "
              @click="saveAndExit"
            >
              Save and exit
            </v-btn>
          </div>
        </v-col>
      </v-row>
    </v-container>
    <ConfirmationDialog
      :show="showSaveExitConfirmation"
      :title="
        wizardStore.listComponentMode === 'add'
          ? 'Your information will not be saved'
          : 'Missing or invalid information'
      "
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
        <p>
          If you discard your changes, no information on this page will be
          saved.
        </p>
      </template>
    </ConfirmationDialog>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";

import { useProgramStore } from "@/store/program";
import { useLoadingStore } from "@/store/loading";

import ConfirmationDialog from "./ConfirmationDialog.vue";
import { useRouter } from "vue-router";
import { useWizardStore } from "@/store/wizard";

export default defineComponent({
  name: "WizardHeader",
  components: { ConfirmationDialog },
  props: {
    handleSaveDraft: {
      type: Function,
      required: true,
    },
    validateForm: {
      type: Function,
      required: true,
    },
  },
  setup() {
    const programStore = useProgramStore();
    const loadingStore = useLoadingStore();
    const wizardStore = useWizardStore();
    const { mobile } = useDisplay();
    const router = useRouter();

    return {
      programStore,
      loadingStore,
      wizardStore,
      mobile,
      router,
    };
  },
  data: () => ({
    showConfirmation: false,
    showSaveExitConfirmation: false,
  }),
  methods: {
    toggleSaveExitConfirmation() {
      this.showSaveExitConfirmation = !this.showSaveExitConfirmation;
    },
    async saveAndExit() {
      const valid = await this.validateForm();
      const step = this.wizardStore.currentStep.stage;

      // If user is in the middle of inputting courses they should be able to exit without validation midway.
      // Also review step should not require validation to exit they should accept the terms right before submission.
      if (
        !valid &&
        this.wizardStore.currentStep &&
        step !== "EarlyChildhood" &&
        step !== "InfantAndToddler" &&
        step !== "SpecialNeeds" &&
        step !== "Review"
      ) {
        this.showSaveExitConfirmation = true;
      } else {
        await this.handleSaveDraft();
        this.goToDashboard();
      }
    },
    goToDashboard() {
      this.showSaveExitConfirmation = false; //prevents issue where router will stop responding
      this.router.push({ name: "dashboard" });
    },
  },
});
</script>
