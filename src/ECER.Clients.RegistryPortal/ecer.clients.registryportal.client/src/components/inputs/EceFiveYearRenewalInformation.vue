<template>
  <p>Why are you late renewing your certification?</p>
  <br />
  <v-row>
    <v-col>
      <RadioWithAdditionalOption
        :items="fiveYearRenewalInformationRadio"
        :trigger-values="['Other']"
        additional-info-key="renewalExplanationOther"
        value-key="fiveYearRenewalExplanationChoice"
        :radio-rules="[Rules.requiredRadio('Select an option')]"
        :model-value="wizardStore.wizardData.fiveYearRenewalExplanationChoice"
        :additional-info-props="{ autoGrow: true, counter: 200, maxlength: 200, rules: [Rules.required('Enter your response')] }"
        :text-input-value="wizardStore.wizardData.renewalExplanationOther"
        @update:model-value="updateFields"
      ></RadioWithAdditionalOption>
    </v-col>
  </v-row>
</template>
<script lang="ts">
import { defineComponent } from "vue";

import { useWizardStore } from "@/store/wizard";
import type { EceRenewalInformationProps } from "@/types/input";
import { fiveYearRenewalInformationRadio } from "@/utils/constant";
import * as Rules from "@/utils/formRules";

import RadioWithAdditionalOption from "./RadioWithAdditionalOption.vue";

interface FiveYearRenewalExplanationType {
  fiveYearRenewalExplanationChoice: string;
  renewalExplanationOther: string;
}

export default defineComponent({
  name: "EceFiveYearRenewalInformation",
  components: { RadioWithAdditionalOption },
  props: {
    props: {
      type: Object as () => EceRenewalInformationProps,
      required: true,
    },
    modelValue: { type: String, required: true },
  },
  setup() {
    const wizardStore = useWizardStore();
    return { fiveYearRenewalInformationRadio, Rules, wizardStore };
  },
  methods: {
    updateFields(data: FiveYearRenewalExplanationType) {
      this.wizardStore.setWizardData(data);
    },
  },
});
</script>
