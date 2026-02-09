<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h2>Independent practice details</h2>
      <div class="mb-10" role="doc-subtitle">
        Please confirm the following information about the applicant's
        employment
      </div>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          Country
          <v-select
            id="certificateCountrySelect"
            class="pt-2"
            :items="
              configStore.countryList.filter(
                (country) => country.isICRA === true,
              )
            "
            variant="outlined"
            label=""
            @update:model-value="updateField('countryId', $event)"
            item-title="countryName"
            item-value="countryId"
            :rules="[
              Rules.required(
                'Select the country of the applicant\'s employment',
                'countryId',
              ),
            ]"
          ></v-select>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            id="employerNameTextInput"
            :rules="[
              Rules.required(
                'Enter the name of the applicant\'s employer, organization, or child care program',
              ),
            ]"
            label="Name of employer, organization, or child care program"
            @update:model-value="updateField('employerName', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            id="positionTitleTextInput"
            :rules="[
              Rules.required(
                'Enter the name of the applicant\'s job or position title',
              ),
            ]"
            label="Applicant's job or position title"
            @update:model-value="updateField('positionTitle', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceDateInput
            id="startDateInput"
            label="Start date of employment"
            :model-value="modelValue.startDate"
            :max="today"
            :rules="[
              Rules.required('Enter the applicant\'s employment start date'),
              Rules.futureDateNotAllowedRule(
                'Start date cannot be in the future',
              ),
            ]"
            @update:model-value="updateField('startDate', $event)"
          ></EceDateInput>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceDateInput
            id="endDateInput"
            clearable
            label="End date of employment"
            :model-value="modelValue.endDate"
            :rules="[Rules.dateBeforeRule(modelValue.startDate || '')]"
            @update:model-value="updateField('endDate', $event)"
          ></EceDateInput>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <p>Have you observed the applicant working with young children?</p>
          <v-radio-group
            id="workedWithChildrenRadioGroup"
            hide-details="auto"
            :rules="[
              Rules.requiredRadio(
                'Select if you have observed the applicant working with children in the above child care program',
              ),
            ]"
            @update:model-value="updateField('workedWithChildren', $event)"
          >
            <v-radio label="Yes" :value="true"></v-radio>
            <v-radio label="No" :value="false"></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <br />
      <h2>Child care program details</h2>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <p>What ages of children were present?</p>
          <br />
          <p>Choose all that apply.</p>
          <CheckboxMultiple
            :items="childcareAgeRangesCheckBox"
            :select-all="true"
            :rules="[
              Rules.atLeastOneOptionRequired(
                'Select the ages of children present',
              ),
            ]"
            @update:model-value="updateField('childcareAgeRanges', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <p>What is your relationship to the applicant?</p>
          <v-radio-group
            id="referenceRelationshipRadio"
            hide-details="auto"
            :rules="[
              Rules.requiredRadio('Select your relationship to the applicant'),
            ]"
            @update:model-value="updateField('referenceRelationship', $event)"
          >
            <v-radio
              v-for="(
                workReferenceRelationship, index
              ) in workReferenceRelationshipRadioFiltered"
              :key="index"
              :label="workReferenceRelationship.label"
              :value="workReferenceRelationship.value"
            ></v-radio>
          </v-radio-group>
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
import { useConfigStore } from "@/store/config";
import type { Components } from "@/types/openapi";
import {
  childcareAgeRangesCheckBox,
  childrenProgramTypeDropdown,
  workHoursTypeRadio,
  workReferenceRelationshipRadio,
} from "@/utils/constant";
import { formatDate } from "@/utils/format";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";
import EceDateInput from "@/components/inputs/EceDateInput.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";

export default defineComponent({
  name: "EceIcraEligibilityWorkExperienceReferenceEvaluation",
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
    "update:model-value": (
      _workExReferenceData: Components.Schemas.WorkExperienceReferenceDetails,
    ) => true,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    const configStore = useConfigStore();

    return {
      configStore,
      wizardStore,
      childrenProgramTypeDropdown,
      workHoursTypeRadio,
      childcareAgeRangesCheckBox,
      workReferenceRelationshipRadio,
    };
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
    workReferenceRelationshipRadioFiltered() {
      return workReferenceRelationshipRadio.filter(
        (relationship) => relationship.value !== "Other",
      );
    },
  },
  mounted() {},
  methods: {
    isNumber,
    updateField(
      fieldName: keyof Components.Schemas.ICRAWorkExperienceReferenceSubmissionRequest,
      value: any,
    ) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: value,
      });
    },
  },
});
</script>
