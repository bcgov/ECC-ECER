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
          <p class="small">Where did the applicant complete their work experience hours?</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.childrenProgramName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">What was their role while completing the work experience hours?</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.role }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">If they cared for children, what was the age range? (Optional)</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.ageofChildrenCaredFor }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Start date of hours</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ formatDate(evaluation.startDate!, "LLLL d, yyyy") }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">End date of hours</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ formatDate(evaluation.endDate!, "LLLL d, yyyy") }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">What is your relationship to applicant?</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.referenceRelationship }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Comments</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ evaluation.additionalComments }}</p>
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
import { workHoursTypeRadio, workReference400HoursRelationshipRadio } from "@/utils/constant";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "EceWorkExperienceReferenceEvaluationPreview",
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
    evaluation(): Components.Schemas.WorkExperienceReferenceDetails {
      const workHoursTypeDisplay = workHoursTypeRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperience400HoursEvaluation.form.inputs.workExperience400HoursEvaluation.id]
            ?.workHoursType,
      )?.label;

      const referenceRelationshipDisplay = workReference400HoursRelationshipRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperience400HoursEvaluation.form.inputs.workExperience400HoursEvaluation.id]
            ?.referenceRelationship,
      )?.label;

      return {
        hours:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperience400HoursEvaluation.form.inputs.workExperience400HoursEvaluation.id]
            ?.hours,
        workHoursType: workHoursTypeDisplay as Components.Schemas.WorkHoursType,
        childrenProgramName:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperience400HoursEvaluation.form.inputs.workExperience400HoursEvaluation.id]
            ?.childrenProgramName,
        role: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperience400HoursEvaluation.form.inputs.workExperience400HoursEvaluation.id]
          ?.role,
        ageofChildrenCaredFor:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperience400HoursEvaluation.form.inputs.workExperience400HoursEvaluation.id]
            ?.ageofChildrenCaredFor,
        startDate:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperience400HoursEvaluation.form.inputs.workExperience400HoursEvaluation.id]
            ?.startDate,
        endDate:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperience400HoursEvaluation.form.inputs.workExperience400HoursEvaluation.id]
            ?.endDate,
        referenceRelationship: referenceRelationshipDisplay as Components.Schemas.ReferenceRelationship,
        additionalComments:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workExperience400HoursEvaluation.form.inputs.workExperience400HoursEvaluation.id]
            ?.additionalComments,
      };
    },
  },
  methods: {
    formatDate,
  },
});
</script>
