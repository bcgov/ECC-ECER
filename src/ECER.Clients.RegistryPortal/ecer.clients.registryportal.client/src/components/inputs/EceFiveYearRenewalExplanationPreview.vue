<template>
  <PreviewCard title="Renewal information" portal-stage="ExplanationLetter">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Why are you late renewing your certification?</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">
            {{ generateReasonToRenew }}
          </p>
        </v-col>
      </v-row>
      <v-row v-if="hasOtherReasonToRenew">
        <v-col cols="4">
          <p class="small">Other reasons</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">
            {{ wizardStore.wizardData[wizardStore.wizardConfig.steps.fiveYearRenewalExplanation.form.inputs.renewalExplanationOther.id] || "—" }}
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
import { fiveYearRenewalInformationRadio } from "@/utils/constant";
export default defineComponent({
  name: "EceOneYearRenewalExplanationPreview",
  components: {
    PreviewCard,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    return {
      wizardStore,
    };
  },
  computed: {
    hasOtherReasonToRenew() {
      return !!this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.fiveYearRenewalExplanation.form.inputs.renewalExplanationOther.id];
    },
    generateReasonToRenew() {
      const renewalReason = fiveYearRenewalInformationRadio.find(
        (reason) =>
          reason.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.fiveYearRenewalExplanation.form.inputs.fiveYearRenewalExplanation.id],
      )?.label;

      return renewalReason ?? "-";
    },
  },
});
</script>
