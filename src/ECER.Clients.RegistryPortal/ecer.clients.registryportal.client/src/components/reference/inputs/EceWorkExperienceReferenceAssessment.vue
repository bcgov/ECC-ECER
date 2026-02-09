<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h2>
        Are you satisfied that the applicant is competent in the following
        areas?
      </h2>
      <div role="doc-subtitle">
        This must be based on your own observations of the applicant.
      </div>

      <v-row class="mt-5">
        <v-col>
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTriggerValues"
            additional-info-key="childDevelopmentReason"
            value-key="childDevelopment"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.childDevelopment"
            :additional-info-props="{
              autoGrow: true,
              counter: 1000,
              maxlength: 1000,
              rules: [Rules.required('Enter your response')],
            }"
            :text-input-value="modelValue.childDevelopmentReason"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <!-- prettier-ignore -->
              <label>
                Is the applicant competent in <b>child development</b>?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col>
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTriggerValues"
            additional-info-key="childGuidanceReason"
            value-key="childGuidance"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.childGuidance"
            :additional-info-props="{
              autoGrow: true,
              counter: 1000,
              maxlength: 1000,
              rules: [Rules.required('Enter your response')],
            }"
            :text-input-value="modelValue.childGuidanceReason"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <!-- prettier-ignore -->
              <label>
                Is the applicant competent in <b>child guidance</b>?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col>
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTriggerValues"
            additional-info-key="healthSafetyAndNutritionReason"
            value-key="healthSafetyAndNutrition"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.healthSafetyAndNutrition"
            :additional-info-props="{
              autoGrow: true,
              counter: 1000,
              maxlength: 1000,
              rules: [Rules.required('Enter your response')],
            }"
            :text-input-value="modelValue.healthSafetyAndNutritionReason"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <!-- prettier-ignore -->
              <label>
                Is the applicant competent in <b>health, safety and nutrition</b>?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col>
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTriggerValues"
            additional-info-key="developAnEceCurriculumReason"
            value-key="developAnEceCurriculum"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.developAnEceCurriculum"
            :additional-info-props="{
              autoGrow: true,
              counter: 1000,
              maxlength: 1000,
              rules: [Rules.required('Enter your response')],
            }"
            :text-input-value="modelValue.developAnEceCurriculumReason"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <!-- prettier-ignore -->
              <label>
                Is the applicant competent in <b>developing an early childhood education curriculum</b>?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col>
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTriggerValues"
            additional-info-key="implementAnEceCurriculumReason"
            value-key="implementAnEceCurriculum"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.implementAnEceCurriculum"
            :additional-info-props="{
              autoGrow: true,
              counter: 1000,
              maxlength: 1000,
              rules: [Rules.required('Enter your response')],
            }"
            :text-input-value="modelValue.implementAnEceCurriculumReason"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <!-- prettier-ignore -->
              <label>
                Is the applicant competent in <b>implementing an early childhood education curriculum</b>?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col>
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTriggerValues"
            additional-info-key="fosteringPositiveRelationChildReason"
            value-key="fosteringPositiveRelationChild"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.fosteringPositiveRelationChild"
            :additional-info-props="{
              autoGrow: true,
              counter: 1000,
              maxlength: 1000,
              rules: [Rules.required('Enter your response')],
            }"
            :text-input-value="modelValue.fosteringPositiveRelationChildReason"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <!-- prettier-ignore -->
              <label>
                Is the applicant competent in <b>fostering positive relationships with children under their care</b>?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col>
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTriggerValues"
            additional-info-key="fosteringPositiveRelationFamilyReason"
            value-key="fosteringPositiveRelationFamily"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.fosteringPositiveRelationFamily"
            :additional-info-props="{
              autoGrow: true,
              counter: 1000,
              maxlength: 1000,
              rules: [Rules.required('Enter your response')],
            }"
            :text-input-value="modelValue.fosteringPositiveRelationFamilyReason"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <!-- prettier-ignore -->
              <label>
                Is the applicant competent in <b>fostering positive relationships with the families of children</b>?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col>
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTriggerValues"
            additional-info-key="fosteringPositiveRelationCoworkerReason"
            value-key="fosteringPositiveRelationCoworker"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.fosteringPositiveRelationCoworker"
            :additional-info-props="{
              autoGrow: true,
              counter: 1000,
              maxlength: 1000,
              rules: [Rules.required('Enter your response')],
            }"
            :text-input-value="
              modelValue.fosteringPositiveRelationCoworkerReason
            "
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <!-- prettier-ignore -->
              <label>
                Is the applicant competent in <b>fostering positive relationships with co-workers</b>?
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
import { defineComponent } from "vue";

import RadioWithAdditionalOption from "@/components/inputs/RadioWithAdditionalOption.vue";
import type { Components } from "@/types/openapi";
import { likertScaleRadio } from "@/utils/constant";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceWorkExperienceReferenceAssessment",
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
    "update:model-value": (
      _workExAssessmentData: Components.Schemas.WorkExperienceReferenceCompetenciesAssessment,
    ) => true,
  },
  setup: () => {
    const likertScaleTriggerValues: Components.Schemas.LikertScale[] = ["No"];
    return { likertScaleRadio, likertScaleTriggerValues };
  },

  data() {
    return {
      Rules,
    };
  },

  methods: {
    updateFields(value: {
      [key in keyof Components.Schemas.WorkExperienceReferenceCompetenciesAssessment]: any;
    }) {
      this.$emit("update:model-value", { ...this.modelValue, ...value });
    },
  },
});
</script>
