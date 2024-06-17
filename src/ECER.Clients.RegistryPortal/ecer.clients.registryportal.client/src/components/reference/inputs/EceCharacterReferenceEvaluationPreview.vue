<template>
  <ReferencePreviewCard :is-valid="true" title="Reference evaluation" reference-stage="ReferenceEvaluation">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">What is your relationship with the applicant?</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.referenceRelationship }}</p>
        </v-col>
      </v-row>
      <v-row v-if="evaluation.referenceRelationship === 'Other'">
        <v-col cols="4">
          <p class="small">Specify your relationship with the applicant</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.referenceRelationshipOther }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">How long have you known the applicant?</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.lengthOfAcquaintance }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Have you observed the applicant working with young children?</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.workedWithChildren ? "Yes" : "No" }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">
            {{
              evaluation.workedWithChildren
                ? "Describe situation(s) in which you have observed the applicant working with young children"
                : "What characteristics and/or qualities have you seen the applicant demonstrate that would be valuable when working with children?"
            }}
          </p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.childInteractionObservations }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Explain why you consider the applicant to have the temperament and ability to manage or work with young children.</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.applicantTemperamentAssessment }}</p>
        </v-col>
      </v-row>
    </template>
  </ReferencePreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ReferencePreviewCard from "@/components/reference/inputs/ReferencePreviewCard.vue";
import { useWizardStore } from "@/store/wizard";
import type { EcePreviewProps } from "@/types/input";
import type { Components } from "@/types/openapi";
import { lengthOfAcquaintanceDropdown, referenceRelationshipDropdown } from "@/utils/constant";

export default defineComponent({
  name: "EceCharacterReferenceEvaluationPreview",
  components: {
    ReferencePreviewCard,
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
    evaluation(): Components.Schemas.CharacterReferenceEvaluation {
      const referenceReferenceDisplay = referenceRelationshipDropdown.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]
            ?.referenceRelationship,
      )?.title;

      const lengthOfAcquaintanceDisplay = lengthOfAcquaintanceDropdown.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]
            ?.lengthOfAcquaintance,
      )?.title;

      return {
        referenceRelationship: referenceReferenceDisplay as Components.Schemas.ReferenceRelationship,
        referenceRelationshipOther:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]
            ?.referenceRelationshipOther,
        lengthOfAcquaintance: lengthOfAcquaintanceDisplay as Components.Schemas.ReferenceKnownTime,
        workedWithChildren:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]?.workedWithChildren,
        childInteractionObservations:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]
            ?.childInteractionObservations,
        applicantTemperamentAssessment:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]
            ?.applicantTemperamentAssessment,
      };
    },
  },
});
</script>
