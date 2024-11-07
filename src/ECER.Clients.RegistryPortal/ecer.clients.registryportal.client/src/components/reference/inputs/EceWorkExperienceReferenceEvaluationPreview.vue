<template>
  <ReferencePreviewCard :is-valid="true" title="Work experience hours of applicant" reference-stage="ReferenceEvaluation">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Total hours you observed applicant</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.hours }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">How often the applicant worked or volunteered</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.workHoursType }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Name of child care program</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.childrenProgramName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Type of child care program</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.childrenProgramType }}</p>
          <p v-if="evaluation.childrenProgramType === 'Other'" class="small">{{ evaluation.childrenProgramTypeOther }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Ages of children present</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.childcareAgeRanges ? evaluation.childcareAgeRanges.join(", ") : "" }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">When applicant completed the hours</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ formatDate(evaluation.startDate!, "LLLL d, yyyy") }} to {{ formatDate(evaluation.endDate!, "LLLL d, yyyy") }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Your relationship to applicant</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.referenceRelationship }}</p>
          <p v-if="evaluation.referenceRelationship === 'Other'" class="small">{{ evaluation.referenceRelationshipOther }}</p>
        </v-col>
      </v-row>
    </template>
  </ReferencePreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ReferencePreviewCard from "@/components/reference/inputs/ReferencePreviewCard.vue";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import {
  childcareAgeRangesCheckBox,
  childrenProgramTypeDropdown,
  WorkExperienceType,
  workHoursTypeRadio,
  workReferenceRelationshipRadio,
} from "@/utils/constant";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "EceWorkExperienceReferenceEvaluationPreview",
  components: {
    ReferencePreviewCard,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    return {
      wizardStore,
    };
  },
  computed: {
    evaluation(): Components.Schemas.WorkExperienceReferenceDetails {
      const workHoursTypeDisplay = workHoursTypeRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id]?.workHoursType,
      )?.label;
      const childrenProgramTypeDisplay = childrenProgramTypeDropdown.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id]
            ?.childrenProgramType,
      )?.title;

      const childcareAgeRangesDisplay = childcareAgeRangesCheckBox
        .filter((value) =>
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id
          ]?.childcareAgeRanges.includes(value.value),
        )
        .map((value) => value.label);

      const referenceRelationshipDisplay = workReferenceRelationshipRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id]
            ?.referenceRelationship,
      )?.label;

      return {
        hours: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id]?.hours,
        workHoursType: workHoursTypeDisplay as Components.Schemas.WorkHoursType,
        childrenProgramName:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id]
            ?.childrenProgramName,
        childrenProgramType: childrenProgramTypeDisplay as Components.Schemas.ChildrenProgramType,
        childrenProgramTypeOther:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id]
            ?.childrenProgramTypeOther,
        childcareAgeRanges: childcareAgeRangesDisplay as Components.Schemas.ChildcareAgeRanges[],
        startDate: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id]?.startDate,
        endDate: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id]?.endDate,
        referenceRelationship: referenceRelationshipDisplay as Components.Schemas.ReferenceRelationship,
        referenceRelationshipOther:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id]
            ?.referenceRelationshipOther,
        workExperienceType: WorkExperienceType.IS_500_Hours,
      };
    },
  },
  methods: {
    formatDate,
  },
});
</script>
