<template>
  <ReferencePreviewCard
    :is-valid="true"
    title="Competencies assessment"
    subtitle="This must be based on your own observations of the applicant."
    reference-stage="assessment"
  >
    <template #content>
      <div>
        <v-row class="py-3" no-gutters>
          <v-col cols="8" class="pr-6">
            <p class="small mb-0">Is the applicant competent in child development?</p>
          </v-col>
          <v-col cols="4">
            <p class="small font-weight-bold mb-0">{{ assessment.childDevelopment }}</p>
            <p v-if="assessment.childDevelopmentReason" class="small mb-0 mt-1">{{ assessment.childDevelopmentReason }}</p>
          </v-col>
        </v-row>
        <v-divider />

        <v-row class="py-3" no-gutters>
          <v-col cols="8" class="pr-6">
            <p class="small mb-0">Is the applicant competent in child guidance?</p>
          </v-col>
          <v-col cols="4">
            <p class="small font-weight-bold mb-0">{{ assessment.childGuidance }}</p>
            <p v-if="assessment.childGuidanceReason" class="small mb-0 mt-1">{{ assessment.childGuidanceReason }}</p>
          </v-col>
        </v-row>
        <v-divider />

        <v-row class="py-3" no-gutters>
          <v-col cols="8" class="pr-6">
            <p class="small mb-0">Is the applicant competent in health, safety and nutrition?</p>
          </v-col>
          <v-col cols="4">
            <p class="small font-weight-bold mb-0">{{ assessment.healthSafetyAndNutrition }}</p>
            <p v-if="assessment.healthSafetyAndNutritionReason" class="small mb-0 mt-1">{{ assessment.healthSafetyAndNutritionReason }}</p>
          </v-col>
        </v-row>
        <v-divider />

        <v-row class="py-3" no-gutters>
          <v-col cols="8" class="pr-6">
            <p class="small mb-0">Is the applicant competent in developing an early childhood education curriculum?</p>
          </v-col>
          <v-col cols="4">
            <p class="small font-weight-bold mb-0">{{ assessment.developAnEceCurriculum }}</p>
            <p v-if="assessment.developAnEceCurriculumReason" class="small mb-0 mt-1">{{ assessment.developAnEceCurriculumReason }}</p>
          </v-col>
        </v-row>
        <v-divider />

        <v-row class="py-3" no-gutters>
          <v-col cols="8" class="pr-6">
            <p class="small mb-0">Is the applicant competent in implementing an early childhood education curriculum?</p>
          </v-col>
          <v-col cols="4">
            <p class="small font-weight-bold mb-0">{{ assessment.implementAnEceCurriculum }}</p>
            <p v-if="assessment.implementAnEceCurriculumReason" class="small mb-0 mt-1">{{ assessment.implementAnEceCurriculumReason }}</p>
          </v-col>
        </v-row>
        <v-divider />

        <v-row class="py-3" no-gutters>
          <v-col cols="8" class="pr-6">
            <p class="small mb-0">Is the applicant competent in fostering positive relationships with children under their care?</p>
          </v-col>
          <v-col cols="4">
            <p class="small font-weight-bold mb-0">{{ assessment.fosteringPositiveRelationChild }}</p>
            <p v-if="assessment.fosteringPositiveRelationChildReason" class="small mb-0 mt-1">
              {{ assessment.fosteringPositiveRelationChildReason }}
            </p>
          </v-col>
        </v-row>
        <v-divider />

        <v-row class="py-3" no-gutters>
          <v-col cols="8" class="pr-6">
            <p class="small mb-0">Is the applicant competent in fostering positive relationships with the families of children?</p>
          </v-col>
          <v-col cols="4">
            <p class="small font-weight-bold mb-0">{{ assessment.fosteringPositiveRelationFamily }}</p>
            <p v-if="assessment.fosteringPositiveRelationFamilyReason" class="small mb-0 mt-1">
              {{ assessment.fosteringPositiveRelationFamilyReason }}
            </p>
          </v-col>
        </v-row>
        <v-divider />

        <v-row class="py-3" no-gutters>
          <v-col cols="8" class="pr-6">
            <p class="small mb-0">Is the applicant competent in fostering positive relationships with co-workers?</p>
          </v-col>
          <v-col cols="4">
            <p class="small font-weight-bold mb-0">{{ assessment.fosteringPositiveRelationCoworker }}</p>
            <p v-if="assessment.fosteringPositiveRelationCoworkerReason" class="small mb-0 mt-1">
              {{ assessment.fosteringPositiveRelationCoworkerReason }}
            </p>
          </v-col>
        </v-row>
      </div>
    </template>
  </ReferencePreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ReferencePreviewCard from "@/components/reference/inputs/ReferencePreviewCard.vue";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { likertScaleRadio } from "@/utils/constant";

export default defineComponent({
  name: "EceWorkExperienceReferenceAssessmentPreview",
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
    assessment(): Components.Schemas.WorkExperienceReferenceCompetenciesAssessment {
      const childDevelopmentDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]?.childDevelopment,
      )?.label;

      const childGuidanceDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]?.childGuidance,
      )?.label;

      const healthSafetyAndNutritionDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]
            ?.healthSafetyAndNutrition,
      )?.label;

      const developAnEceCurriculumDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]
            ?.developAnEceCurriculum,
      )?.label;

      const implementAnEceCurriculumDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]
            ?.implementAnEceCurriculum,
      )?.label;

      const fosteringPositiveRelationChildDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]
            ?.fosteringPositiveRelationChild,
      )?.label;

      const fosteringPositiveRelationFamilyDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]
            ?.fosteringPositiveRelationFamily,
      )?.label;

      const fosteringPositiveRelationCoworkerDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]
            ?.fosteringPositiveRelationCoworker,
      )?.label;

      return {
        childDevelopment: childDevelopmentDisplay as Components.Schemas.LikertScale,
        childGuidance: childGuidanceDisplay as Components.Schemas.LikertScale,
        healthSafetyAndNutrition: healthSafetyAndNutritionDisplay as Components.Schemas.LikertScale,
        developAnEceCurriculum: developAnEceCurriculumDisplay as Components.Schemas.LikertScale,
        implementAnEceCurriculum: implementAnEceCurriculumDisplay as Components.Schemas.LikertScale,
        fosteringPositiveRelationChild: fosteringPositiveRelationChildDisplay as Components.Schemas.LikertScale,
        fosteringPositiveRelationFamily: fosteringPositiveRelationFamilyDisplay as Components.Schemas.LikertScale,
        fosteringPositiveRelationCoworker: fosteringPositiveRelationCoworkerDisplay as Components.Schemas.LikertScale,
        childDevelopmentReason:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]
            ?.childDevelopmentReason,
        childGuidanceReason:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]?.childGuidanceReason,
        healthSafetyAndNutritionReason:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]
            ?.healthSafetyAndNutritionReason,
        developAnEceCurriculumReason:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]
            ?.developAnEceCurriculumReason,
        implementAnEceCurriculumReason:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]
            ?.implementAnEceCurriculumReason,
        fosteringPositiveRelationChildReason:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]
            ?.fosteringPositiveRelationChildReason,
        fosteringPositiveRelationFamilyReason:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]
            ?.fosteringPositiveRelationFamilyReason,
        fosteringPositiveRelationCoworkerReason:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id || ""]
            ?.fosteringPositiveRelationCoworkerReason,
      };
    },
  },
});
</script>
