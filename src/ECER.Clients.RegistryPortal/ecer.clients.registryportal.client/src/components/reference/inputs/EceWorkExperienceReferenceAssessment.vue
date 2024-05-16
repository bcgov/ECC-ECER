<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h3>Competencies Assessment</h3>
      <div role="doc-subtitle">
        Tell us your opinion on the competency of the applicant in the following areas. This should be based on your personal observations.
      </div>

      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTrigger.map((item) => item.value)"
            additional-info-key="childDevelopmentReason"
            value-key="childDevelopment"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.childDevelopment"
            :additional-info-props="{ autoGrow: true, counter: 1000, maxlength: 1000, rules: [Rules.required('Enter your response')] }"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <label>
                In
                <b>child development</b>
                , how competent do you think the applicant is?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTrigger.map((item) => item.value)"
            additional-info-key="childGuidanceReason"
            value-key="childGuidance"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.childGuidance"
            :additional-info-props="{ autoGrow: true, counter: 1000, maxlength: 1000, rules: [Rules.required('Enter your response')] }"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <label>
                In
                <b>child guidance</b>
                , how competent do you think the applicant is?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTrigger.map((item) => item.value)"
            additional-info-key="healthSafetyAndNutritionReason"
            value-key="healthSafetyAndNutrition"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.healthSafetyAndNutrition"
            :additional-info-props="{ autoGrow: true, counter: 1000, maxlength: 1000, rules: [Rules.required('Enter your response')] }"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <label>
                In
                <b>health, safety and nutrition</b>
                , how competent do you think the applicant is?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTrigger.map((item) => item.value)"
            additional-info-key="developAnEceCurriculumReason"
            value-key="developAnEceCurriculum"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.developAnEceCurriculum"
            :additional-info-props="{ autoGrow: true, counter: 1000, maxlength: 1000, rules: [Rules.required('Enter your response')] }"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <label>
                In
                <b>developing an early childhood education curriculum</b>
                , how competent do you think the applicant is?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTrigger.map((item) => item.value)"
            additional-info-key="implementAnEceCurriculumReason"
            value-key="implementAnEceCurriculum"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.implementAnEceCurriculum"
            :additional-info-props="{ autoGrow: true, counter: 1000, maxlength: 1000, rules: [Rules.required('Enter your response')] }"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <label>
                In
                <b>implementing an early childhood education curriculum</b>
                , how competent do you think the applicant is?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTrigger.map((item) => item.value)"
            additional-info-key="fosteringPositiveRelationChildReason"
            value-key="fosteringPositiveRelationChild"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.fosteringPositiveRelationChild"
            :additional-info-props="{ autoGrow: true, counter: 1000, maxlength: 1000, rules: [Rules.required('Enter your response')] }"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <label>
                In
                <b>fostering positive relationships with children under their care</b>
                , how competent do you think the applicant is?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTrigger.map((item) => item.value)"
            additional-info-key="fosteringPositiveRelationFamilyReason"
            value-key="fosteringPositiveRelationFamily"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.fosteringPositiveRelationFamily"
            :additional-info-props="{ autoGrow: true, counter: 1000, maxlength: 1000, rules: [Rules.required('Enter your response')] }"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <label>
                In
                <b>fostering positive relationships with the families of children</b>
                , how competent do you think the applicant is?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <RadioWithAdditionalOption
            :items="likertScaleRadio"
            :trigger-values="likertScaleTrigger.map((item) => item.value)"
            additional-info-key="fosteringPositiveRelationCoworkerReason"
            value-key="fosteringPositiveRelationCoworker"
            :radio-rules="[Rules.requiredRadio('Select an option')]"
            :model-value="modelValue.fosteringPositiveRelationCoworker"
            :additional-info-props="{ autoGrow: true, counter: 1000, maxlength: 1000, rules: [Rules.required('Enter your response')] }"
            @update:model-value="updateFields"
          >
            <template #radioLabel>
              <label>
                In
                <b>fostering positive relationships with co-workers</b>
                , how competent do you think the applicant is?
              </label>
            </template>
            <template #textAreaLabel>
              <label>Please explain why</label>
            </template>
          </RadioWithAdditionalOption>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <h3>Please confirm</h3>
          <div role="doc-subtitle">When answering the folowing question, please consider your responses above and your personal obserations.</div>
          <p class="mt-4">Do you believe that the applicant has the personality, ability and temperament necessary to work with children?</p>
          <v-radio-group hide-details="auto" :rules="[Rules.requiredRadio('Select an option')]" @update:model-value="applicantQualifiedChanged">
            <v-radio label="Yes" :value="true"></v-radio>
            <v-radio label="No" :value="false"></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <v-row v-if="modelValue.isApplicantQualified !== undefined">
        <v-col cols="12" md="8" lg="6" xl="4">
          <label for="applicantNotQualifiedTextArea">Please explain why</label>
          <v-textarea
            id="applicantNotQualifiedTextArea"
            class="mt-2"
            :rules="[Rules.required('Enter your response')]"
            counter="1000"
            variant="outlined"
            color="primary"
            maxlength="1000"
            hide-details="auto"
            :auto-grow="true"
            @update:model-value="updateTextField('applicantNotQualifiedReason', $event)"
          ></v-textarea>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { defineComponent } from "vue";

import RadioWithAdditionalOption from "@/components/inputs/RadioWithAdditionalOption.vue";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { likertScaleRadio, likertScaleTrigger } from "@/utils/constant";
import { formatDate } from "@/utils/format";
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
    "update:model-value": (_workExAssessmentData: Components.Schemas.WorkExperienceReferenceCompetenciesAssessment) => true,
  },
  setup: () => {
    const wizardStore = useWizardStore();

    return { wizardStore, likertScaleRadio, likertScaleTrigger };
  },

  data() {
    return {
      Rules,
    };
  },

  computed: {
    today() {
      return formatDate(DateTime.now().toString());
    },
  },
  methods: {
    updateFields(value: { [key in keyof Components.Schemas.WorkExperienceReferenceCompetenciesAssessment]: any }) {
      this.$emit("update:model-value", { ...this.modelValue, ...value });
    },
    updateTextField(fieldName: keyof Components.Schemas.WorkExperienceReferenceCompetenciesAssessment, value: any) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: value,
      });
    },
    applicantQualifiedChanged(value: any) {
      if (value === true) {
        this.$emit("update:model-value", { ...this.modelValue, isApplicantQualified: value, applicantNotQualifiedReason: "" });
      } else {
        this.$emit("update:model-value", { ...this.modelValue, isApplicantQualified: value });
      }
    },
  },
});
</script>
