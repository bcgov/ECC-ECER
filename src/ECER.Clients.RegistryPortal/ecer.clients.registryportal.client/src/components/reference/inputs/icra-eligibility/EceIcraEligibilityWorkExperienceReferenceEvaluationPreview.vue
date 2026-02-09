<template>
  <ReferencePreviewCard
    :is-valid="true"
    title="Independent practice details"
    reference-stage="ReferenceEvaluation"
  >
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Country</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">
            {{ configStore.countryName(reference.countryId || "") }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">
            Name of employer, organization, or child care program
          </p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ reference.employerName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Applicant's position or job title</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ reference.positionTitle }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Start date of employment</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">
            {{
              reference?.startDate
                ? formatDate(reference.startDate || "", "LLLL d, yyyy")
                : "-"
            }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">End date of employment</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">
            {{
              reference?.endDate
                ? formatDate(reference.endDate || "", "LLLL d, yyyy")
                : "-"
            }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <h2>Child care program details</h2>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">
            Have you observed the applicant working with children in the above
            child care program?
          </p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">
            {{ reference.workedWithChildren ? "Yes" : "No" }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">What ages of children were present?</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ childAgeRangeDisplay }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">What is your relationship to the applicant?</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">
            {{ workReferenceRelationshipDisplay }}
          </p>
        </v-col>
      </v-row>
    </template>
  </ReferencePreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ReferencePreviewCard from "@/components/reference/inputs/ReferencePreviewCard.vue";
import { useConfigStore } from "@/store/config";
import { useWizardStore } from "@/store/wizard";
import {
  childcareAgeRangesCheckBox,
  workReferenceRelationshipRadio,
} from "@/utils/constant";
import type { Components } from "@/types/openapi";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "EceIcraEligibilityWorkExperienceReferenceEvaluationPreview",
  components: {
    ReferencePreviewCard,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    const configStore = useConfigStore();
    return {
      wizardStore,
      configStore,
    };
  },
  computed: {
    reference(): Components.Schemas.ICRAWorkExperienceReferenceSubmissionRequest {
      return this.wizardStore.wizardData[
        this.wizardStore?.wizardConfig?.steps?.workExperienceEvaluation?.form
          ?.inputs?.workExperienceEvaluation?.id || ""
      ];
    },
    childAgeRangeDisplay(): string {
      return childcareAgeRangesCheckBox
        .filter((ageRange) =>
          this.reference.childcareAgeRanges?.includes(ageRange.value),
        )
        .map((ageRange) => ageRange.label)
        .join(", ");
    },
    workReferenceRelationshipDisplay(): string {
      return (
        workReferenceRelationshipRadio.find(
          (relationship) =>
            relationship.value === this.reference.referenceRelationship,
        )?.label || ""
      );
    },
  },
  methods: {
    formatDate,
  },
});
</script>
