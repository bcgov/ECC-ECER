<template>
  <Wizard :ref="'wizard'" :wizard="programStore.applicationConfiguration">
    <template #header>
      <WizardHeader
        class="mb-6"
        :handle-save-draft="handleSaveAndExit"
        :validate-form="validateForm"
      >
        <template #title>
          <h1>{{ generateWizardHeaderTitle }}</h1>
        </template>
      </WizardHeader>
    </template>
    <template #stepperHeader>
      <v-container v-show="showSteps">
        <v-stepper-header class="elevation-0">
          <template
            v-for="(step, index) in Object.values(wizardStore.steps)"
            :key="step.stage"
          >
            <v-stepper-item
              color="primary"
              :step="wizardStore.step"
              :value="index + 1"
              :title="step.title"
              :editable="
                index + 1 < wizardStore.step &&
                wizardStore.listComponentMode !== 'add'
              "
              :complete="index + 1 < wizardStore.step"
              :class="`small ${mdAndDown ? 'text-wrap' : 'text-no-wrap'}`"
            >
              <template #title>
                <a v-if="index + 1 < wizardStore.step" href="#" @click.prevent>
                  {{ step.title }}
                </a>
                <div v-else>{{ step.title }}</div>
              </template>
            </v-stepper-item>
            <v-divider
              v-if="index !== Object.values(wizardStore.steps).length - 1"
              :key="`divider-${index}`"
            />
          </template>
        </v-stepper-header>
      </v-container>
    </template>
    <template #PrintPreview>
      <v-btn rounded="lg" variant="text" @click="printPage()">
        <v-icon
          color="secondary"
          icon="mdi-printer-outline"
          class="mr-2"
        ></v-icon>
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
                  :loading="
                    loadingStore.isLoading('draftprogram_put') ||
                    loadingStore.isLoading('psp_user_profile_put') ||
                    loadingStore.isLoading('psp_user_profile_get')
                  "
                  rounded="lg"
                  color="primary"
                  @click="handleSaveAndContinue"
                >
                  Save and continue
                </v-btn>
                <v-btn
                  id="btnSubmitApplication"
                  v-if="showSubmitApplication"
                  rounded="lg"
                  color="primary"
                  :loading="
                    loadingStore.isLoading('draftprogram_put') ||
                    loadingStore.isLoading('program_post')
                  "
                  @click="handleSubmit"
                >
                  Submit
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
import { DateTime } from "luxon";
import { useRouter } from "vue-router";

import Wizard from "@/components/Wizard.vue";
import WizardHeader from "@/components/WizardHeader.vue";
import programWizard from "@/config/program-wizard/program-wizard.ts";
import { useAlertStore } from "@/store/alert";
import { useProgramStore } from "@/store/program";
import { useLoadingStore } from "@/store/loading";
import { useWizardStore } from "@/store/wizard";
import type { ProgramStage } from "@/types/wizard";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "ProgramWizard",
  components: { Wizard, WizardHeader },
  props: {
    programId: {
      type: String,
      required: true,
    },
    program: {
      type: Object as () => Components.Schemas.Program,
      required: true,
    },
  },
  setup: async (props) => {
    const wizardStore = useWizardStore();
    const alertStore = useAlertStore();
    const programStore = useProgramStore();
    const loadingStore = useLoadingStore();
    const router = useRouter();
    const { mdAndDown, mobile } = useDisplay();

    programStore.setDraftProgramFromProfile(props.program);

    await wizardStore.initializeWizard(
      programStore.applicationConfiguration,
      programStore.draftProgram,
    );

    return {
      programStore,
      wizardStore,
      alertStore,
      loadingStore,
      programWizard,
      mdAndDown,
      mobile,
      router,
    };
  },
  computed: {
    ...mapWritableState(useWizardStore, { mode: "listComponentMode" }),
    generateWizardHeaderTitle(): string {
      const startYear = this.programStore.draftProgram.startDate
        ? DateTime.fromISO(this.programStore.draftProgram.startDate).toFormat(
            "yyyy",
          )
        : "";

      const endYear = this.programStore.draftProgram.endDate
        ? DateTime.fromISO(this.programStore.draftProgram.endDate).toFormat(
            "yyyy",
          )
        : "";
      return `${this.programStore.draftProgram.programName} ${startYear} - ${endYear}`;
    },
    showSaveButtons() {
      return (
        this.wizardStore.currentStepStage !== "Review" &&
        !(
          this.wizardStore.currentStepStage === "ProgramOverview" &&
          this.wizardStore.listComponentMode === "add"
        ) &&
        !(
          this.wizardStore.currentStepStage === "EarlyChildhood" &&
          this.wizardStore.listComponentMode === "add"
        ) &&
        !(
          this.wizardStore.currentStepStage === "InfantAndToddler" &&
          this.wizardStore.listComponentMode === "add"
        ) &&
        !(
          this.wizardStore.currentStepStage === "SpecialNeeds" &&
          this.wizardStore.listComponentMode === "add"
        )
      );
    },
    showSubmitApplication() {
      return this.wizardStore.currentStepStage === "Review";
    },
    showSteps() {
      return !this.mobile && this.wizardStore.listComponentMode !== "add";
    },
  },
  mounted() {
    this.mode = "list";
  },
  methods: {
    async handleSaveAndContinue() {
      const valid = await this.validateForm();
      if (valid) {
        switch (this.wizardStore.currentStepStage) {
          case "ProgramOverview":
            await this.saveDraftAndAlertSuccess(false);
            this.incrementWizard();
            break;
          case "EarlyChildhood":
          case "InfantAndToddler":
          case "SpecialNeeds":
            await this.saveDraftAndAlertSuccess(false);
            // refresh needed to sync wizard data and draft program changes
            await this.wizardStore.initializeWizard(
              this.programStore.applicationConfiguration,
              this.programStore.draftProgram,
            );
            this.incrementWizard();
            break;
        }
      } else {
        this.alertStore.setFailureAlert(
          "You must enter all required fields in the valid format.",
        );
      }
    },
    async handleSubmit() {
      const valid = await this.validateForm();
      if (valid) {
        const submitProgramResponse =
          await this.programStore.submitDraftProgramApplication();
        if (submitProgramResponse) {
          this.router.push({
            name: "programSubmitted",
          });
        } else {
          this.alertStore.setFailureAlert(
            "There was an error submitting your application. Please try again later.",
          );
        }
      } else {
        this.alertStore.setFailureAlert(
          "You must enter all required fields in the valid format.",
        );
      }
    },
    async validateForm() {
      const currentStepFormId = this.wizardStore.currentStep.form.id;
      const formRef = (this.$refs.wizard as typeof Wizard).$refs[
        currentStepFormId
      ][0].$refs[currentStepFormId];
      const { valid } = await formRef.validate();

      return valid;
    },
    incrementWizard() {
      this.wizardStore.incrementStep();
      this.programStore.draftProgram.portalStage = this.wizardStore
        .currentStepStage as ProgramStage;
    },
    decrementWizard() {
      this.wizardStore.decrementStep();
      this.programStore.draftProgram.portalStage = this.wizardStore
        .currentStepStage as ProgramStage;
    },
    async handleBack() {
      await this.programStore.saveDraft();
      this.decrementWizard();
    },
    async handleSaveAndExit() {
      await this.saveDraftAndAlertSuccess(true);
    },
    async saveDraftAndAlertSuccess(exit: boolean) {
      const draftApplicationResponse = await this.programStore.saveDraft();
      if (draftApplicationResponse?.program) {
        let message =
          "Information saved. If you save and exit, you can resume your application later.";
        if (exit)
          message = "Information saved. You can resume your application later.";
        this.alertStore.setSuccessAlert(message);
      }
    },
    printPage() {
      globalThis.print();
    },
  },
});
</script>
