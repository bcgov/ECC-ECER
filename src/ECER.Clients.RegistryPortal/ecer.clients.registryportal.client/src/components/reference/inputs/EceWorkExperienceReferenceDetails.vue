<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h3>Work experience hours</h3>
      <div role="doc-subtitle">Please provide as much detail as possible.</div>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          Applicant indicated you observed them work:
          <b>50 hours</b>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            type="number"
            :rules="[Rules.required('Enter a whole number greater than zero (0)')]"
            label="Total number of hours you observed the applicant working"
            variant="outlined"
            color="primary"
            maxlength="10"
            hide-details="auto"
            @input="updateField('hours', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <p>How often did the applicant work or valunteer?</p>
          <v-radio-group hide-details="auto" :rules="[Rules.requiredRadio('Select an option')]" @update:model-value="updateField('workHoursType', $event)">
            <v-radio v-for="(workHoursType, index) in workHoursTypeRadio" :key="index" :label="workHoursType.label" :value="workHoursType.value"></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <h3>Program and age of children</h3>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            :rules="[Rules.required('Enter the name of child care program')]"
            label="Name of child care program"
            variant="outlined"
            color="primary"
            maxlength="100"
            hide-details="auto"
            @input="updateField('childrenProgramName', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-autocomplete
            label="Type of child care program"
            variant="outlined"
            color="primary"
            :rules="[Rules.required('Select an option')]"
            :items="childrenProgramTypeDropdown"
            hide-details="auto"
            @update:model-value="updateField('childrenProgramType', $event)"
          ></v-autocomplete>
        </v-col>
      </v-row>

      <v-row v-if="modelValue.childrenProgramType === 'Other'" class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            label="Specify Child Care Program"
            variant="outlined"
            color="primary"
            :rules="[Rules.required('Please specify child care program')]"
            hide-details="auto"
            @update:model-value="updateField('childrenProgramTypeOther', $event)"
          ></v-text-field>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <p>What ages of children were present?</p>
          <p>Choose all that apply.</p>
          <v-checkbox
            v-for="(item, index) in childcareAgeRangesCheckBox"
            :key="index"
            :rules="[Rules.hasCheckbox()]"
            :label="item.label"
            :value="item.value"
            hide-details="auto"
            @update:model-value="updateField('childcareAgeRanges', $event)"
          ></v-checkbox>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <h3>When did the applicant complete the hours</h3>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            label="Start date of hours (DD MMM YYY)"
            variant="outlined"
            color="primary"
            hide-details="auto"
            type="date"
            :max="today"
            @input="updateField('startDate', $event)"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            label="End date of hours (DD MMM YYY)"
            variant="outlined"
            color="primary"
            hide-details="auto"
            type="date"
            :max="today"
            @input="updateField('endDate', $event)"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <h3>Relationship to applicant</h3>
          <div role="doc-subtitle">You must have directly supervised (observed) the applicant working with children</div>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <p>What is your relationship to the applicant?</p>
          <v-radio-group
            hide-details="auto"
            :rules="[Rules.requiredRadio('Select an option')]"
            @update:model-value="updateField('referenceRelationship', $event)"
          >
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
          <v-text-field
            label="Specify Your relationship"
            variant="outlined"
            color="primary"
            :rules="[Rules.required('Specify your relationship to the applicant. Your response should not exceed 100 characters')]"
            hide-details="auto"
            maxlength="100"
            @update:model-value="updateField('referenceRelationshipOther', $event)"
          ></v-text-field>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { defineComponent } from "vue";

import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { childrenProgramTypeDropdown } from "@/utils/constant";
import { workHoursTypeRadio } from "@/utils/constant";
import { childcareAgeRangesCheckBox } from "@/utils/constant";
import { workReferenceRelationshipRadio } from "@/utils/constant";
import { formatDate } from "@/utils/format";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceWorkExperienceReferenceDetails",
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
  methods: {
    updateField(fieldName: keyof Components.Schemas.WorkExperienceReferenceDetails, value: any) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: value,
      });
    },
    referenceRelationshipChanged(value: Components.Schemas.ReferenceRelationship) {
      if (value !== "Other") {
        this.$emit("update:model-value", {
          ...this.modelValue,
          referenceRelationship: value,
          referenceRelationshipOther: "",
        });
      } else {
        this.$emit("update:model-value", { ...this.modelValue, referenceRelationship: value });
      }
    },
    applicantShouldNotBeECEChanged(value: any) {
      // if (value === true) {
      //   this.$emit("update:model-value", { ...this.modelValue, applicantShouldNotBeECE: value });
      // } else {
      //   this.$emit("update:model-value", { ...this.modelValue, applicantShouldNotBeECE: value, applicantNotQualifiedReason: "" });
      // }
    },
  },
});
</script>
<!-- <style>
.v-checkbox {
  margin-top: 0;
  margin-bottom: 0;
  padding-top: 0;
  padding-bottom: 0;
}
.inline-checkbox {
  margin: 0;
  padding: 0;
}
</style> -->
