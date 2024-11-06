<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h2>Reference evaluation</h2>
      <div role="doc-subtitle">
        {{
          `Tell us about your personal experience with the applicant ${wizardStore.wizardData.applicantFirstName} ${wizardStore.wizardData.applicantLastName}.`
        }}
      </div>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-autocomplete
            label="What is your relationship with the applicant?"
            variant="outlined"
            color="primary"
            :rules="[Rules.required('Select an option')]"
            :items="referenceRelationshipDropdown"
            hide-details="auto"
            @update:model-value="referenceRelationshipChanged"
          ></v-autocomplete>
        </v-col>
      </v-row>
      <v-row v-if="modelValue.referenceRelationship === 'Other'" class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            label="Specify your relationship with applicant"
            maxlength="100"
            :rules="[Rules.required('Enter your relationship with the applicant')]"
            @update:model-value="updateField('referenceRelationshipOther', $event)"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-autocomplete
            label="How long have you known the applicant? (Approximately)"
            variant="outlined"
            color="primary"
            :rules="[Rules.required('Select an option')]"
            :items="lengthOfAcquaintanceDropdown"
            hide-details="auto"
            @update:model-value="updateField('lengthOfAcquaintance', $event)"
          ></v-autocomplete>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <p>Have you observed the applicant working with young children?</p>
          <v-radio-group hide-details="auto" :rules="[Rules.requiredRadio('Select an option')]" @update:model-value="updateField('workedWithChildren', $event)">
            <v-radio label="Yes" :value="true"></v-radio>
            <v-radio label="No" :value="false"></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <v-row v-if="modelValue.workedWithChildren !== undefined">
        <v-col cols="12" md="8" lg="6" xl="4">
          <label for="childInteractionObservations">
            {{
              modelValue.workedWithChildren
                ? "Describe situation(s) in which you have observed the applicant working with young children."
                : "What characteristics and/or qualities have you seen the applicant demonstrate that would be valuable when working with children?"
            }}
          </label>
          <v-textarea
            id="childInteractionObservations"
            :rules="[Rules.required('Enter your response')]"
            counter="1000"
            variant="outlined"
            color="primary"
            maxlength="1000"
            persistent-counter
            hide-details="auto"
            :auto-grow="true"
            @update:model-value="updateField('childInteractionObservations', $event)"
          ></v-textarea>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <label for="applicantTemperamentAssessmentTextField">
            Explain why you consider the applicant to have the temperament and ability to manage or work with young children.
          </label>
          <v-textarea
            id="applicantTemperamentAssessmentTextField"
            :rules="[Rules.required('Enter your response')]"
            counter="1000"
            variant="outlined"
            color="primary"
            maxlength="1000"
            persistent-counter
            hide-details="auto"
            :auto-grow="true"
            @update:model-value="updateField('applicantTemperamentAssessment', $event)"
          ></v-textarea>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import EceTextField from "@/components/inputs/EceTextField.vue";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { lengthOfAcquaintanceDropdown, referenceRelationshipDropdown } from "@/utils/constant";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceCharacterReferenceEvaluation",
  components: { EceTextField },
  props: {
    modelValue: {
      type: Object as () => Components.Schemas.CharacterReferenceEvaluation,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_characterReferenceData: Components.Schemas.CharacterReferenceEvaluation) => true,
  },
  setup: () => {
    const wizardStore = useWizardStore();

    return { wizardStore, referenceRelationshipDropdown, lengthOfAcquaintanceDropdown };
  },
  data() {
    return {
      Rules,
      applicantShouldNotBeECE: undefined,
    };
  },
  methods: {
    updateField(fieldName: keyof Components.Schemas.CharacterReferenceEvaluation, value: any) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: value,
      });
    },
    referenceRelationshipChanged(value: Components.Schemas.ReferenceRelationship) {
      if (value !== "Other") {
        this.$emit("update:model-value", {
          ...this.modelValue,
          referenceRelationship: value,
          referenceRelationshipOther: "",
        });
      } else {
        this.$emit("update:model-value", { ...this.modelValue, referenceRelationship: value });
      }
    },
  },
});
</script>
