<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h2>Work experience hours</h2>
      <div role="doc-subtitle">Please provide as much detail as possible.</div>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <h2 class="mb-5">Hours</h2>
          <p class="mb-3">The hours must:</p>
          <ul class="ml-10">
            <li>Be related to the field of early childhood education</li>
            <li v-if="latestCertificateStatus === 'Expired'">Have been completed within the last 5 years</li>
            <li v-else-if="latestCertificateStatus === 'Active' && latestCertificateExpiryDate > today">
              Have been completed between the {{ formatDate(latestCertificateEffectiveDate, "LLL d, yyyy") }} and the {{ formatDate(today, "LLL d, yyyy") }}
            </li>
          </ul>
        </v-col>
      </v-row>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          Applicant indicated you observed them work:
          <b>{{ `${wizardStore.wizardData.workExperienceReferenceHours}` }} hours</b>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            label="Total number of hours the applicant worked"
            :rules="[Rules.required('Enter a whole number greater than zero (0)')]"
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
          <h2>Work experience information</h2>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            :rules="[Rules.required()]"
            label="Where did the applicant complete their work experience hours?"
            maxlength="100"
            @update:model-value="updateField('childrenProgramName', $event)"
          />
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            :rules="[Rules.required('Enter role of the applicant')]"
            label="What was their role while completing the work experience hours?"
            maxlength="100"
            @update:model-value="updateField('role', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            label="If they cared for children, what was the age range? (Optional)"
            maxlength="100"
            @update:model-value="updateField('ageofChildrenCaredFor', $event)"
          />
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <h2>When did the applicant complete the hours?</h2>
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
              v-for="(workReferenceRelationship, index) in workReference400HoursRelationshipRadio"
              :key="index"
              :label="workReferenceRelationship.label"
              :value="workReferenceRelationship.value"
            ></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <h2>Additional comments</h2>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12">
          <v-textarea
            label="Comments (Optional)"
            class="mt-2"
            counter="1000"
            maxlength="1000"
            color="primary"
            variant="outlined"
            hide-details="auto"
            @update:model-value="updateField('additionalComments', $event)"
          ></v-textarea>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { defineComponent } from "vue";

import EceTextField from "@/components/inputs/EceTextField.vue";
import EceDateInput from "@/components/inputs/EceDateInput.vue";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { workHoursTypeRadio, workReference400HoursRelationshipRadio } from "@/utils/constant";
import { WorkExperienceType } from "@/utils/constant";
import { formatDate } from "@/utils/format";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceWorkExperienceReference400HoursEvaluation",
  components: { EceTextField, EceDateInput },
  props: {
    modelValue: {
      type: Object as () => Components.Schemas.WorkExperienceReferenceDetails,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_workExReference400HoursData: Components.Schemas.WorkExperienceReferenceDetails) => true,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    return { wizardStore, workHoursTypeRadio, workReference400HoursRelationshipRadio };
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
    fiveYearsAgo() {
      const fiveYearsBack = DateTime.now().minus({ years: 5 });
      return formatDate(fiveYearsBack.toString());
    },
    latestCertificateStatus(): Components.Schemas.CertificateStatusCode {
      return this.wizardStore.wizardData.latestCertification.statusCode;
    },
    latestCertificateExpiryDate() {
      return formatDate(this.wizardStore.wizardData.latestCertification.expiryDate);
    },
    latestCertificateEffectiveDate() {
      return formatDate(this.wizardStore.wizardData.latestCertification.effectiveDate);
    },
  },
  mounted() {
    // Set Work Exp Ref Type as 400 Hours
    this.updateField("workExperienceType", WorkExperienceType.IS_400_Hours);
  },
  methods: {
    isNumber,
    updateField(fieldName: keyof Components.Schemas.WorkExperienceReferenceDetails, value: any) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: value,
      });
    },
    formatDate,
  },
});
</script>
