<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h3>Reference evaluation</h3>
      <div role="doc-subtitle">
        {{
          `Tell us about your personal experience with the applicant ${wizardStore.wizardData.applicantFirstName} ${wizardStore.wizardData.applicantLastName}`
        }}
      </div>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-autocomplete
            v-model="referenceRelationship"
            label="What is your relationship with the applicant?"
            variant="outlined"
            color="primary"
            :rules="[Rules.required()]"
            :items="referenceRelationshipDropdown"
            hide-details="auto"
            @update:model-value="referenceRelationshipChanged"
          ></v-autocomplete>
        </v-col>
      </v-row>
      <v-row class="mt-5" v-if="referenceRelationship === 'Other'">
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            v-model="referenceRelationshipOther"
            label="Specify your relationship with applicant"
            variant="outlined"
            color="primary"
            :rules="[Rules.required()]"
            hide-details="auto"
            @update:model-value="(value) => $emit('update:model-value', value, { referenceRelationship: value })"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-autocomplete
            label="How long have you known the applicant?"
            variant="outlined"
            color="primary"
            :rules="[Rules.required()]"
            :items="['6 months to 1 year', '1 to 2 years', '2 to 5 years', '5 or more years']"
            hide-details="auto"
            @update:model-value="(value) => $emit('update:model-value', value, { lengthOfAcquaintance: value })"
          ></v-autocomplete>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <p>Have you observed the applicant working with young children?</p>
          <v-radio-group
            v-model="workedWithChildren"
            @update:model-value="(value) => $emit('update:model-value', value, { workedWithChildren: value })"
            hide-details="auto"
            :rules="[Rules.requiredRadio()]"
          >
            <v-radio label="Yes" :value="true"></v-radio>
            <v-radio label="No" :value="false"></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <v-row v-if="workedWithChildren !== null">
        <v-col cols="12" md="8" lg="6" xl="4">
          <label for="childInteractionObservations">
            {{
              workedWithChildren
                ? "Describe situation(s) in which you have observed the applicant working with young children."
                : "What charastics and/or qualities have you seen the applicant demonstrate that would be valuable when working with children?"
            }}
          </label>
          <v-textarea
            id="childInteractionObservations"
            :rules="[Rules.required()]"
            counter="1000"
            variant="outlined"
            color="primary"
            maxlength="1000"
            hide-details="auto"
            @update:model-value="(value) => $emit('update:model-value', value, { childInteractionObservations: value })"
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
            :rules="[Rules.required()]"
            counter="1000"
            variant="outlined"
            color="primary"
            maxlength="1000"
            hide-details="auto"
            @update:model-value="(value) => $emit('update:model-value', value, { applicantTemperamentAssessment: value })"
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
          <v-radio-group
            v-model="applicantShouldNotBeECE"
            @update:model-value="applicantShouldNotBeECEChanged"
            hide-details="auto"
            :rules="[Rules.requiredRadio()]"
          >
            <v-radio label="Yes" :value="false"></v-radio>
            <v-radio label="No" :value="true"></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <v-row class="mt-5" v-if="applicantShouldNotBeECE">
        <v-col cols="12" md="8" lg="6" xl="4">
          <label for="applicantShouldNotBeECETextArea">
            Explain why you consider the applicant to have the temperment and ability to manage or work with young children.
          </label>
          <v-textarea
            id="applicantShouldNotBeECETextArea"
            :rules="[Rules.required()]"
            counter="1000"
            variant="outlined"
            color="primary"
            maxlength="1000"
            hide-details="auto"
            @update:model-value="(value) => $emit('update:model-value', value, { applicantNotQualifiedReason: value })"
          ></v-textarea>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import type { VTextField } from "vuetify/components";

import type { FormData } from "@/store/form";
import { useWizardStore } from "@/store/wizard";
import * as Rules from "@/utils/formRules";
import { referenceRelationshipDropdown } from "@/utils/constant";

export default defineComponent({
  name: "EceCharacterReferenceEvaluation",
  components: {},
  emits: {
    "update:model-value": (_value: any, _updateFormData?: FormData) => true,
  },
  setup: () => {
    const wizardStore = useWizardStore();

    return { wizardStore, referenceRelationshipDropdown };
  },
  data() {
    return {
      Rules,
      workedWithChildren: undefined,
      referenceRelationshipOther: "",
      referenceRelationship: "",
      childInteractionObservations: "",
      applicantShouldNotBeECE: undefined,
      applicantNotQualifiedReason: "",
    };
  },
  methods: {
    referenceRelationshipChanged(value: string) {
      if (value !== "Other") {
        this.referenceRelationshipOther = "";
        this.$emit("update:model-value", null, { referenceRelationship: value, referenceRelationshipOther: "" });
      } else {
        this.$emit("update:model-value", null, { referenceRelationship: value });
      }
    },
    applicantShouldNotBeECEChanged(value: boolean | undefined | null) {
      if (value === true) {
        this.$emit("update:model-value", null, { applicantShouldNotBeECE: value });
      } else {
        this.applicantNotQualifiedReason = "";
        this.$emit("update:model-value", null, { applicantShouldNotBeECE: value, applicantNotQualifiedReason: "" });
      }
    },
  },
});
</script>
