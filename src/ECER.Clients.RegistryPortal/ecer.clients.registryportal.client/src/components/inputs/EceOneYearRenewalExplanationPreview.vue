<template>
  <PreviewCard title="Renewal Information" portal-stage="ExplanationLetter">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Why do you need to renew your ECE One Year certification</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">
            {{ generateReasonToRenew }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Other reason</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">
            {{ wizardStore.wizardData[wizardStore.wizardConfig.steps.oneYearRenewalExplanation.form.inputs.explanationLetter.id] || "—" }}
          </p>
        </v-col>
      </v-row>
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useWizardStore } from "@/store/wizard";
import type { EcePreviewProps } from "@/types/input";
import { renewalInformationRadio } from "@/utils/constant";
export default defineComponent({
  name: "EceOneYearRenewalExplanationPreview",
  components: {
    PreviewCard,
  },
  props: {
    props: {
      type: Object as () => EcePreviewProps,
      required: true,
    },
  },
  setup: () => {
    const wizardStore = useWizardStore();
    return {
      wizardStore,
    };
  },
  computed: {
    generateReasonToRenew() {
      const renewalReason = renewalInformationRadio.find(
        (reason) =>
          reason.value === this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.oneYearRenewalExplanation.form.inputs.oneYearRenewalExplanation.id],
      )?.label;

      return renewalReason ?? "-";
    },
  },
});
</script>
