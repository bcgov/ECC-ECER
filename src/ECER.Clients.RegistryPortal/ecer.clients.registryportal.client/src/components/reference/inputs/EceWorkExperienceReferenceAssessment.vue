<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h3>Competencies Assessment</h3>
      <div role="doc-subtitle">
        Tell us your opinion on the competency of the applicant in the following areas. This should be based on your personal observations.
      </div>

      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="['SomewhatCompetent', 'NotCompetent']"
            additional-info-key="additional"
            value-key="valuekey"
            :radio-rules="[Rules.requiredRadio()]"
            :model-value="modelValue.childDevelopment"
            @update:model-value="update"
          >
            <template #radioLabel>
              <label>
                In
                <b>child development</b>
                , How Competent do you think the applicant is?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { defineComponent } from "vue";

import RadioWithAdditionalOption from "@/components/inputs/RadioWithAdditionalOption.vue";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { likertScaleRadio } from "@/utils/constant";
import { formatDate } from "@/utils/format";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceWorkExperienceReferenceEvaluation",
  components: {
    RadioWithAdditionalOption,
  },
  props: {
    modelValue: {
      type: Object as () => Components.Schemas.WorkExperienceReferenceCompetenciesAssessment,
      required: true,
    },
  },
  emits: {
    //"update:model-value": (_workExAssessmentData: Components.Schemas.WorkExperienceReferenceCompetenciesAssessment) => true,
  },
  setup: () => {
    const wizardStore = useWizardStore();

    return { wizardStore, likertScaleRadio };
  },

  data() {
    return {
      Rules,
      selectedOption: null,
      test: "",
    };
  },

  computed: {
    today() {
      return formatDate(DateTime.now().toString());
    },
  },
  methods: {
    update(value: any) {
      console.log(value);
      this.test = value.valuekey;
    },
  },
});
</script>
