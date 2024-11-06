<template>
  <p>You need to provide the reason why you were unable to either:</p>
  <br />
  <ul class="ml-10">
    <li>Complete the required 500 hours of supervised work experience during the term of your certificate</li>
    <li>Get a reference contact from the certified ECE who supervised the hours</li>
  </ul>
  <br />
  <v-row>
    <v-col>
      <RadioWithAdditionalOption
        :items="renewalInformationRadio"
        :trigger-values="['Other']"
        additional-info-key="renewalExplanationOther"
        value-key="oneYearRenewalExplanationChoice"
        :radio-rules="[Rules.requiredRadio('Select an option')]"
        :model-value="wizardStore.wizardData.oneYearRenewalExplanationChoice"
        :additional-info-props="{ autoGrow: true, counter: 500, maxlength: 500, rules: [Rules.required('Enter a reason')] }"
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
import { renewalInformationRadio } from "@/utils/constant";
import * as Rules from "@/utils/formRules";

import RadioWithAdditionalOption from "./RadioWithAdditionalOption.vue";

interface OneYearRenewalExplanationType {
  oneYearRenewalExplanation: string;
  explanationLetter: string;
}

export default defineComponent({
  name: "EceRenewalInformation",
  components: { RadioWithAdditionalOption },
  props: {
    modelValue: { type: String, required: true },
  },
  setup() {
    const wizardStore = useWizardStore();
    return { renewalInformationRadio, Rules, wizardStore };
  },
  methods: {
    updateFields(data: OneYearRenewalExplanationType) {
      this.wizardStore.setWizardData(data);
    },
  },
});
</script>
