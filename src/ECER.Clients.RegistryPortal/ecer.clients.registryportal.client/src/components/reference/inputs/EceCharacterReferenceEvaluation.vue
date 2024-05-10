<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h3>Reference evaluation</h3>
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
          <v-text-field
            label="Specify your relationship with applicant"
            variant="outlined"
            color="primary"
            :rules="[Rules.required('Enter your relationship with the applicant')]"
            hide-details="auto"
            @update:model-value="updateField('referenceRelationshipOther', $event)"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-autocomplete
            label="How long have you known the applicant? (Approximately)"
            variant="outlined"
            color="primary"
            :rules="[Rules.required('Select an option')]"
            :items="lengthOfAcquaintenceDropdown"
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
            hide-details="auto"
            :auto-grow="true"
            @update:model-value="updateField('childInteractionObservations', $event)"
          ></v-textarea>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <label for="applicantTempermentAssessmentTextField">
            Explain why you consider the applicant to have the temperment and ability to manage or work with young children.
          </label>
          <v-textarea
            id="applicantTempermentAssessmentTextField"
            :rules="[Rules.required('Enter your response')]"
            counter="1000"
            variant="outlined"
            color="primary"
            maxlength="1000"
            hide-details="auto"
            :auto-grow="true"
            @update:model-value="updateField('applicantTemperamentAssessment', $event)"
          ></v-textarea>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <h3>Please confirm</h3>
          <p>
            If certified, the applicant may work alone in a licensed childcare facility with children 0-5 years of age for extended periods of time. Do you
            believe the applicant should be granted authorization to be an ECE or ECE Assistant?
          </p>
          <v-radio-group hide-details="auto" :rules="[Rules.requiredRadio('Select an option')]" @update:model-value="applicantShouldNotBeECEChanged">
            <v-radio label="Yes" :value="false"></v-radio>
            <v-radio label="No" :value="true"></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <v-row v-if="modelValue.applicantShouldNotBeECE" class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <label for="applicantShouldNotBeECETextArea">Why do you believe the applicant should NOT be granted authorization to be ECE or ECE Assistant.</label>
          <v-textarea
            id="applicantShouldNotBeECETextArea"
            :rules="[Rules.required('Enter your response')]"
            counter="1000"
            variant="outlined"
            color="primary"
            maxlength="1000"
            hide-details="auto"
            :auto-grow="true"
            @update:model-value="updateField('applicantNotQualifiedReason', $event)"
          ></v-textarea>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { lengthOfAcquaintenceDropdown, referenceRelationshipDropdown } from "@/utils/constant";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceCharacterReferenceEvaluation",
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

    return { wizardStore, referenceRelationshipDropdown, lengthOfAcquaintenceDropdown };
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
    applicantShouldNotBeECEChanged(value: any) {
      if (value === true) {
        this.$emit("update:model-value", { ...this.modelValue, applicantShouldNotBeECE: value });
      } else {
        this.$emit("update:model-value", { ...this.modelValue, applicantShouldNotBeECE: value, applicantNotQualifiedReason: "" });
      }
    },
  },
});
</script>
