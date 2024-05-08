<template>
  <ReferencePreviewCard :is-valid="true" title="Reference evaluation" reference-stage="ReferenceEvaluation">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">What is your relationship with the applicant?</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ evaluation.referenceRelationship }}</p>
        </v-col>
      </v-row>
      <v-row v-if="evaluation.referenceRelationship === 'Other'">
        <v-col cols="4">
          <p class="small">Specify your relationship with the applicant</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ evaluation.referenceRelationshipOther }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">How long have you known the applicant?</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ evaluation.lengthOfAcquaintance }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Have you observed the applicant working with young children?</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ evaluation.workedWithChildren ? "Yes" : "No" }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">
            {{
              evaluation.workedWithChildren
                ? "Describe sutuation(s) in which you have observed the applicant working with young chidlren"
                : "What characteristics and/or qualities have you seen the applicant demonstrate that would be valuable when working with children?"
            }}
          </p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ evaluation.childInteractionObservations }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Explain why you consider the applicant to have the temperment and ability to manage or work with young children.</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ evaluation.applicantTemperamentAssessment }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">
            Once certified, an ECE or ECE Assistant may work alone in a licensed childcare facility with children 0-5 years of age for extended periods of time.
            Do you believe/think the applicant should be granted authorization to be an ECE or ECE Assistant
          </p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ evaluation.applicantShouldNotBeECE ? "No" : "Yes" }}</p>
        </v-col>
      </v-row>
      <v-row v-if="evaluation.applicantShouldNotBeECE">
        <v-col cols="4">
          <p class="small">Why do you think the applicant should NOT be granted authorization to be an ECE or ECE Assistant</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ evaluation.applicantNotQualifiedReason }}</p>
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
import { referenceRelationshipDropdown } from "@/utils/constant";

export default defineComponent({
  name: "EceEcharacterReferenceEvaluationPreview",
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

      return {
        referenceRelationship: referenceReferenceDisplay as Components.Schemas.ReferenceRelationship,
        referenceRelationshipOther:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]
            ?.referenceRelationshipOther,
        lengthOfAcquaintance:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]
            ?.lengthOfAcquaintance,
        workedWithChildren:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]?.workedWithChildren,
        childInteractionObservations:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]
            ?.childInteractionObservations,
        applicantTemperamentAssessment:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]
            ?.applicantTemperamentAssessment,
        applicantShouldNotBeECE:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]
            ?.applicantShouldNotBeECE,
        applicantNotQualifiedReason:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]
            ?.applicantNotQualifiedReason,
      };
    },
  },
});
</script>
