<template>
  <Loading v-if="loadingStore.isLoading('reconsiderations_get')" />
  <Wizard
    v-else
    :ref="'wizard'"
    :wizard="wizardConfigSetup"
    :show-steps="false"
  >
    <template #header>
      <v-container fluid class="bg-primary">
        <v-container>
          <v-row>
            <v-col>
              <h1 class="white">
                Dispute application for certification decision
              </h1>
            </v-col>
          </v-row>
        </v-container>
      </v-container>
      <v-container>
        <Breadcrumb />
        <v-btn
          v-if="wizardStore.step !== 1"
          slim
          variant="text"
          rounded="lg"
          color="primary"
          @click="handleBack"
        >
          <v-icon size="x-large" icon="mdi-chevron-left" />
          Back
        </v-btn>
      </v-container>
    </template>
    <template #PrintPreview>
      <v-btn rounded="lg" variant="text" @click="printPage">
        <v-icon
          color="secondary"
          icon="mdi-printer-outline"
          class="mr-2"
        ></v-icon>
        <a class="small">Print Preview</a>
      </v-btn>
    </template>
    <template #actions>
      <v-container class="mb-8">
        <v-row no-gutters>
          <v-col>
            <v-btn
              v-if="wizardStore.step === userReviewStep"
              :loading="loadingStore.isLoading('reconsiderations_submit_put')"
              rounded="lg"
              variant="flat"
              color="primary"
              @click="handleSubmit"
            >
              Submit dispute
            </v-btn>
            <v-btn
              v-else
              rounded="lg"
              variant="flat"
              color="primary"
              @click="handleContinue"
            >
              Continue
            </v-btn>
          </v-col>
        </v-row>
      </v-container>
    </template>
  </Wizard>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import type { PropType } from "vue";
import { useRouter } from "vue-router";
import type { Components } from "@/types/openapi";

import {
  getReconsiderationsQuery,
  submitReconsideration,
} from "@/api/reconsideration";
import reconsiderationWizard from "@/config/reconsideration/reconsideration-wizard.ts";
import { useAlertStore } from "@/store/alert";
import { useLoadingStore } from "@/store/loading";
import { useWizardStore } from "@/store/wizard";
import type { ReconsiderationType } from "@/types/reconsideration";

import type { Wizard as WizardType } from "@/types/wizard";
import Wizard from "../Wizard.vue";
import Loading from "../Loading.vue";
import Breadcrumb from "../Breadcrumb.vue";

export default defineComponent({
  name: "Reconsideration",
  components: { Wizard, Loading, Breadcrumb },
  async setup(props) {
    const router = useRouter();
    const { data } = await getReconsiderationsQuery(props.reconsiderationId);
    let wizardConfigSetup: WizardType | undefined = undefined;

    const wizardStore = useWizardStore();
    const loadingStore = useLoadingStore();
    const alertStore = useAlertStore();

    switch (props.reconsiderationType) {
      case "application":
        wizardStore.initializeWizardForReconsideration(
          reconsiderationWizard,
          data?.[0],
        );
        wizardConfigSetup = reconsiderationWizard;
        break;
      case "investigation":
        // TODO implement later
        console.warn("Not implemented");
        break;
      default:
        console.error(
          `Unhandled reconsideration type ${props.reconsiderationType}`,
        );
    }
    return {
      alertStore,
      wizardStore,
      loadingStore,
      wizardConfigSetup,
      router,
    };
  },
  props: {
    reconsiderationType: {
      type: String as PropType<ReconsiderationType>,
      required: true,
    },
    reconsiderationId: { type: String, required: true },
  },
  computed: {
    userReviewStep(): number {
      return (
        this.wizardStore.steps.findIndex((step) => step.stage === "Review") + 1
      );
    },
  },
  methods: {
    async handleContinue() {
      const currentStepFormId = this.wizardStore.currentStep.form.id;
      const formRef = (this.$refs.wizard as typeof Wizard).$refs[
        currentStepFormId
      ][0].$refs[currentStepFormId];
      const { valid } = await formRef.validate();

      if (!valid) {
        this.alertStore.setFailureAlert(
          "You must enter all required fields in the valid format to continue",
        );
      } else {
        this.wizardStore.incrementStep();
      }
    },
    handleBack() {
      this.wizardStore.decrementStep();
    },
    async handleSubmit() {
      const currentStepFormId = this.wizardStore.currentStep.form.id;
      const formRef = (this.$refs.wizard as typeof Wizard).$refs[
        currentStepFormId
      ][0].$refs[currentStepFormId];
      const { valid } = await formRef.validate();

      if (!valid) {
        this.alertStore.setFailureAlert(
          "You must enter all required fields in the valid format to continue",
        );
        return;
      }

      switch (this.reconsiderationType) {
        case "application":
          await this.handleSubmitForApplicationReconsideration();
          break;
        case "investigation":
          this.alertStore.setWarningAlert(
            "Investigation reconsideration submission is not yet implemented",
          );
          break;
        default:
          console.error(
            `unhandled reconsideration type ${this.reconsiderationType}`,
          );
          this.alertStore.setFailureAlert(
            `unhandled reconsideration type ${this.reconsiderationType}`,
          );
      }
    },
    async handleSubmitForApplicationReconsideration() {
      const reconsiderationWizardId =
        this.wizardStore?.wizardConfig?.steps?.reconsideration?.form?.inputs
          ?.reconsideration?.id;
      const reconsiderationToSubmit = {
        id: this.reconsiderationId,
        explanationAndEvidence:
          this.wizardStore.wizardData[reconsiderationWizardId]
            .explanationAndEvidence,
        files: this.wizardStore.wizardData[reconsiderationWizardId].files,
      } as Components.Schemas.Reconsideration;

      const { data, error } = await submitReconsideration(
        reconsiderationToSubmit,
      );
      if (error) {
        this.alertStore.setFailureAlert(
          "There was an error submitting your dispute request. Please try again later.",
        );

        return;
      }

      this.router.push({
        name: "reconsideration-submitted",
        params: {
          reconsiderationId: data,
          reconsiderationType: this.reconsiderationType,
        },
      });
    },
    async printPage() {
      const currentStepFormId = this.wizardStore.currentStep.form.id;
      const formRef = (this.$refs.wizard as typeof Wizard).$refs[
        currentStepFormId
      ][0].$refs[currentStepFormId];
      const { valid } = await formRef.validate();

      if (!valid) {
        this.alertStore.setFailureAlert(
          "You must enter all required fields in the valid format before printing",
        );
      } else {
        globalThis.print();
      }
    },
  },
});
</script>
