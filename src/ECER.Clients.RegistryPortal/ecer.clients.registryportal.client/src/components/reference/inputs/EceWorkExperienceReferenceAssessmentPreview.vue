<template>
  <ReferencePreviewCard
    :is-valid="true"
    title="Are you satisfied that the applicant is competent in the following areas?"
    subtitle="This must be based on your own observations of the applicant."
    reference-stage="Assessment"
  >
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Child development</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">
            {{ assessment.childDevelopment }}
          </p>
          <p v-if="assessment.childDevelopmentReason" class="small">
            {{ assessment.childDevelopmentReason }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Child guidance</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ assessment.childGuidance }}</p>
          <p v-if="assessment.childGuidanceReason" class="small">
            {{ assessment.childGuidanceReason }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Health, safety and nutrition</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">
            {{ assessment.healthSafetyAndNutrition }}
          </p>
          <p v-if="assessment.healthSafetyAndNutritionReason" class="small">
            {{ assessment.healthSafetyAndNutritionReason }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">
            Developing an early childhood education curriculum
          </p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">
            {{ assessment.developAnEceCurriculum }}
          </p>
          <p v-if="assessment.developAnEceCurriculumReason" class="small">
            {{ assessment.developAnEceCurriculumReason }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">
            Implementing an early childhood education curriculum
          </p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">
            {{ assessment.implementAnEceCurriculum }}
          </p>
          <p v-if="assessment.implementAnEceCurriculumReason" class="small">
            {{ assessment.implementAnEceCurriculumReason }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">
            Fostering positive relationships with children under their care
          </p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">
            {{ assessment.fosteringPositiveRelationChild }}
          </p>
          <p
            v-if="assessment.fosteringPositiveRelationChildReason"
            class="small"
          >
            {{ assessment.fosteringPositiveRelationChildReason }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">
            Fostering positive relationships with the families of children
          </p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">
            {{ assessment.fosteringPositiveRelationFamily }}
          </p>
          <p
            v-if="assessment.fosteringPositiveRelationFamilyReason"
            class="small"
          >
            {{ assessment.fosteringPositiveRelationFamilyReason }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Fostering positive relationships with co-workers</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">
            {{ assessment.fosteringPositiveRelationCoworker }}
          </p>
          <p
            v-if="assessment.fosteringPositiveRelationCoworkerReason"
            class="small"
          >
            {{ assessment.fosteringPositiveRelationCoworkerReason }}
          </p>
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
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.childDevelopment,
      )?.label;

      const childGuidanceDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.childGuidance,
      )?.label;

      const healthSafetyAndNutritionDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.healthSafetyAndNutrition,
      )?.label;

      const developAnEceCurriculumDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.developAnEceCurriculum,
      )?.label;

      const implementAnEceCurriculumDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.implementAnEceCurriculum,
      )?.label;

      const fosteringPositiveRelationChildDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.fosteringPositiveRelationChild,
      )?.label;

      const fosteringPositiveRelationFamilyDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.fosteringPositiveRelationFamily,
      )?.label;

      const fosteringPositiveRelationCoworkerDisplay = likertScaleRadio.find(
        (value) =>
          value.value ===
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.fosteringPositiveRelationCoworker,
      )?.label;

      return {
        childDevelopment:
          childDevelopmentDisplay as Components.Schemas.LikertScale,
        childGuidance: childGuidanceDisplay as Components.Schemas.LikertScale,
        healthSafetyAndNutrition:
          healthSafetyAndNutritionDisplay as Components.Schemas.LikertScale,
        developAnEceCurriculum:
          developAnEceCurriculumDisplay as Components.Schemas.LikertScale,
        implementAnEceCurriculum:
          implementAnEceCurriculumDisplay as Components.Schemas.LikertScale,
        fosteringPositiveRelationChild:
          fosteringPositiveRelationChildDisplay as Components.Schemas.LikertScale,
        fosteringPositiveRelationFamily:
          fosteringPositiveRelationFamilyDisplay as Components.Schemas.LikertScale,
        fosteringPositiveRelationCoworker:
          fosteringPositiveRelationCoworkerDisplay as Components.Schemas.LikertScale,
        childDevelopmentReason:
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.childDevelopmentReason,
        childGuidanceReason:
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.childGuidanceReason,
        healthSafetyAndNutritionReason:
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.healthSafetyAndNutritionReason,
        developAnEceCurriculumReason:
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.developAnEceCurriculumReason,
        implementAnEceCurriculumReason:
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.implementAnEceCurriculumReason,
        fosteringPositiveRelationChildReason:
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.fosteringPositiveRelationChildReason,
        fosteringPositiveRelationFamilyReason:
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.fosteringPositiveRelationFamilyReason,
        fosteringPositiveRelationCoworkerReason:
          this.wizardStore.wizardData[
            this.wizardStore.wizardConfig.steps?.assessment?.form?.inputs
              ?.workExperienceAssessment?.id || ""
          ]?.fosteringPositiveRelationCoworkerReason,
      };
    },
  },
});
</script>
