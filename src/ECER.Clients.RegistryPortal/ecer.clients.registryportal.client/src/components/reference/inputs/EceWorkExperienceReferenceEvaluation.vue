<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h2>Work experience hours</h2>
      <div role="doc-subtitle">Please provide as much detail as possible.</div>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          Applicant indicated you observed them work:
          <b>{{ `${wizardStore.wizardData.workExperienceReferenceHours}` }} hours</b>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            :rules="[Rules.required('Enter a whole number greater than zero (0)')]"
            label="Total number of hours you observed the applicant working"
            maxlength="10"
            @keypress="isNumber($event)"
            @update:model-value="updateField('hours', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <p>How often did the applicant work or volunteer?</p>
          <v-radio-group hide-details="auto" :rules="[Rules.requiredRadio('Select an option')]" @update:model-value="updateField('workHoursType', $event)">
            <v-radio v-for="(workHoursType, index) in workHoursTypeRadio" :key="index" :label="workHoursType.label" :value="workHoursType.value"></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <h2>Program and age of children</h2>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            :rules="[Rules.required('Enter the name of child care program')]"
            label="Name of child care program"
            maxlength="100"
            @update:model-value="updateField('childrenProgramName', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <label>
            Type of child care program
            <v-autocomplete
              label=""
              variant="outlined"
              color="primary"
              :rules="[Rules.required('Select an option')]"
              :items="childrenProgramTypeDropdown"
              hide-details="auto"
              class="pt-2"
              @update:model-value="childrenProgramTypeChanged"
            ></v-autocomplete>
          </label>
        </v-col>
      </v-row>

      <v-row v-if="modelValue.childrenProgramType === 'Other'" class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            label="Specify Child Care Program"
            :rules="[Rules.required('Please specify child care program')]"
            @update:model-value="updateField('childrenProgramTypeOther', $event)"
          ></EceTextField>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <p>What ages of children were present?</p>
          <p>Choose all that apply.</p>
          <CheckboxMultiple
            :items="childcareAgeRangesCheckBox"
            :select-all="true"
            :rules="[Rules.atLeastOneOptionRequired()]"
            @update:model-value="updateField('childcareAgeRanges', $event)"
          />
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <h2>When did the applicant complete the hours</h2>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceDateInput
            label="Start date of hours"
            :model-value="modelValue.startDate"
            :max="today"
            :rules="[Rules.required('Enter the start date of hours'), Rules.futureDateNotAllowedRule('Start date of hours cannot be in the future')]"
            @update:model-value="updateField('startDate', $event)"
          ></EceDateInput>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceDateInput
            label="End date of hours"
            :model-value="modelValue.endDate"
            :max="today"
            :rules="[
              Rules.required('Enter the end date of hours'),
              Rules.dateBeforeRule(modelValue.startDate || ''),
              Rules.futureDateNotAllowedRule('End date of hours cannot be in the future'),
            ]"
            @update:model-value="updateField('endDate', $event)"
          ></EceDateInput>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <h2>Relationship to applicant</h2>
          <div role="doc-subtitle">You must have directly supervised (observed) the applicant working with children</div>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <p>What is your relationship to the applicant?</p>
          <v-radio-group hide-details="auto" :rules="[Rules.requiredRadio('Select an option')]" @update:model-value="referenceRelationshipChanged">
            <v-radio
              v-for="(workReferenceRelationship, index) in workReferenceRelationshipRadio"
              :key="index"
              :label="workReferenceRelationship.label"
              :value="workReferenceRelationship.value"
            ></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <v-row v-if="modelValue.referenceRelationship === 'Other'" class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            label="Specify Your relationship"
            :rules="[Rules.required('Specify your relationship to the applicant. Your response should not exceed 100 characters')]"
            maxlength="100"
            @update:model-value="updateField('referenceRelationshipOther', $event)"
          ></EceTextField>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { defineComponent } from "vue";

import CheckboxMultiple from "@/components/inputs/CheckboxMultiple.vue";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { childcareAgeRangesCheckBox, childrenProgramTypeDropdown, workHoursTypeRadio, workReferenceRelationshipRadio } from "@/utils/constant";
import { WorkExperienceType } from "@/utils/constant";
import { formatDate } from "@/utils/format";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";
import EceDateInput from "@/components/inputs/EceDateInput.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";

export default defineComponent({
  name: "EceWorkExperienceReferenceEvaluation",
  components: {
    CheckboxMultiple,
    EceDateInput,
    EceTextField,
  },
  props: {
    modelValue: {
      type: Object as () => Components.Schemas.WorkExperienceReferenceDetails,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_workExReferenceData: Components.Schemas.WorkExperienceReferenceDetails) => true,
  },
  setup: () => {
    const wizardStore = useWizardStore();

    return { wizardStore, childrenProgramTypeDropdown, workHoursTypeRadio, childcareAgeRangesCheckBox, workReferenceRelationshipRadio };
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
  mounted() {
    // Set Work Exp Ref Type as 500 Hours
    this.updateField("workExperienceType", WorkExperienceType.IS_500_Hours);
  },
  methods: {
    isNumber,
    updateField(fieldName: keyof Components.Schemas.WorkExperienceReferenceDetails, value: any) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: value,
      });
    },

    childrenProgramTypeChanged(value: Components.Schemas.ChildrenProgramType) {
      if (value !== "Other") {
        this.$emit("update:model-value", {
          ...this.modelValue,
          childrenProgramType: value,
          childrenProgramTypeOther: "",
        });
      } else {
        this.$emit("update:model-value", { ...this.modelValue, childrenProgramType: value });
      }
    },

    referenceRelationshipChanged(value: any) {
      if (value === "Other") {
        this.$emit("update:model-value", { ...this.modelValue, referenceRelationship: value });
      } else {
        this.$emit("update:model-value", { ...this.modelValue, referenceRelationship: value, referenceRelationshipOther: "" });
      }
    },
  },
});
</script>
