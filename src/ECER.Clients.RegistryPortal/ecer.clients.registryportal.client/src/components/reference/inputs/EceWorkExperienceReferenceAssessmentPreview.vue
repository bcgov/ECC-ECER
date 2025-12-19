<template>
  <div>
    <!-- Application type -->
    <ReferencePreviewCard :is-valid="true" title="Application type" reference-stage="ApplicationType">
      <template #content>
        <v-row>
          <v-col cols="4">
            <p class="small">Certification type</p>
          </v-col>
          <v-col cols="8">
            <p class="small font-weight-bold">{{ applicationType.certificationType || "-" }}</p>
          </v-col>
        </v-row>
      </template>
    </ReferencePreviewCard>

    <!-- Education -->
    <ReferencePreviewCard :is-valid="true" title="Education" reference-stage="Education">
      <template #content>
        <div v-if="education.institutionName" class="mb-4">
          <p class="small font-weight-bold mb-0">{{ education.institutionName }}</p>
        </div>

        <v-row>
          <v-col cols="4"><p class="small">Name of program or course</p></v-col>
          <v-col cols="8"><p class="small font-weight-bold">{{ education.programName || "-" }}</p></v-col>
        </v-row>

        <v-row>
          <v-col cols="4"><p class="small">Start date of program or course</p></v-col>
          <v-col cols="8"><p class="small font-weight-bold">{{ formatDate(education.startDate) }}</p></v-col>
        </v-row>

        <v-row>
          <v-col cols="4"><p class="small">End date of program or course</p></v-col>
          <v-col cols="8"><p class="small font-weight-bold">{{ formatDate(education.endDate) }}</p></v-col>
        </v-row>

        <v-row>
          <v-col cols="4"><p class="small">Country</p></v-col>
          <v-col cols="8"><p class="small font-weight-bold">{{ education.country || "-" }}</p></v-col>
        </v-row>

        <v-row>
          <v-col cols="4"><p class="small">Campus location</p></v-col>
          <v-col cols="8"><p class="small font-weight-bold">{{ education.campusLocation || "-" }}</p></v-col>
        </v-row>

        <v-row>
          <v-col cols="4"><p class="small">Student number or ID</p></v-col>
          <v-col cols="8"><p class="small font-weight-bold">{{ education.studentNumber || "-" }}</p></v-col>
        </v-row>

        <v-row>
          <v-col cols="4"><p class="small">Your full name as shown on transcript</p></v-col>
          <v-col cols="8"><p class="small font-weight-bold">{{ education.fullNameOnTranscript || "-" }}</p></v-col>
        </v-row>
      </template>
    </ReferencePreviewCard>

    <!-- Character reference -->
    <ReferencePreviewCard :is-valid="true" title="Character reference" reference-stage="CharacterReference">
      <template #content>
        <v-row>
          <v-col cols="4"><p class="small">Last Name</p></v-col>
          <v-col cols="8"><p class="small font-weight-bold">{{ characterReference.lastName || "-" }}</p></v-col>
        </v-row>

        <v-row>
          <v-col cols="4"><p class="small">First Name</p></v-col>
          <v-col cols="8"><p class="small font-weight-bold">{{ characterReference.firstName || "-" }}</p></v-col>
        </v-row>

        <v-row>
          <v-col cols="4"><p class="small">Email</p></v-col>
          <v-col cols="8"><p class="small font-weight-bold">{{ characterReference.email || "-" }}</p></v-col>
        </v-row>

        <v-row>
          <v-col cols="4"><p class="small">Phone number</p></v-col>
          <v-col cols="8"><p class="small font-weight-bold">{{ characterReference.phoneNumber || "-" }}</p></v-col>
        </v-row>
      </template>
    </ReferencePreviewCard>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ReferencePreviewCard from "@/components/reference/inputs/ReferencePreviewCard.vue";
import { useWizardStore } from "@/store/wizard";

type AnyObj = Record<string, any>;

export default defineComponent({
  name: "EceApplicationReviewSubmitPreview",
  components: {
    ReferencePreviewCard,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    return { wizardStore };
  },
  computed: {
    applicationType(): { certificationType: string } {
      // Try a few likely step/input locations; adjust these keys if your wizardConfig differs.
      const candidateInputIds: string[] = [
        this.getInputId("applicationType", "applicationType"),
        this.getInputId("applicationType", "certificationType"),
        this.getInputId("review", "applicationType"),
        this.getInputId("reviewAndSubmit", "applicationType"),
      ].filter(Boolean) as string[];

      const data = this.firstWizardData(candidateInputIds);

      // Common field names we might see
      const certificationType =
        data?.certificationType ??
        data?.certification ??
        data?.certificationTypes?.[0] ??
        data?.certificationTypeDisplay ??
        "";

      return { certificationType };
    },

    education(): {
      institutionName: string;
      programName: string;
      startDate: string;
      endDate: string;
      country: string;
      campusLocation: string;
      studentNumber: string;
      fullNameOnTranscript: string;
    } {
      const candidateInputIds: string[] = [
        this.getInputId("education", "education"),
        this.getInputId("education", "eceEducation"),
        this.getInputId("education", "training"),
        this.getInputId("review", "education"),
        this.getInputId("reviewAndSubmit", "education"),
      ].filter(Boolean) as string[];

      const data = this.firstWizardData(candidateInputIds);

      return {
        institutionName: data?.institutionName ?? data?.schoolName ?? data?.institution ?? data?.nameOfSchool ?? "",
        programName: data?.programName ?? data?.courseName ?? data?.programOrCourseName ?? "",
        startDate: data?.startDate ?? data?.programStartDate ?? "",
        endDate: data?.endDate ?? data?.programEndDate ?? "",
        country: data?.country ?? "",
        campusLocation: data?.campusLocation ?? data?.campus ?? "",
        studentNumber: data?.studentNumber ?? data?.studentId ?? data?.studentNumberOrId ?? "",
        fullNameOnTranscript: data?.fullNameOnTranscript ?? data?.nameOnTranscript ?? "",
      };
    },

    characterReference(): {
      lastName: string;
      firstName: string;
      email: string;
      phoneNumber: string;
    } {
      const candidateInputIds: string[] = [
        this.getInputId("characterReference", "characterReference"),
        this.getInputId("characterReference", "reference"),
        this.getInputId("reference", "characterReference"),
        this.getInputId("review", "characterReference"),
        this.getInputId("reviewAndSubmit", "characterReference"),
      ].filter(Boolean) as string[];

      const data = this.firstWizardData(candidateInputIds);

      return {
        lastName: data?.lastName ?? "",
        firstName: data?.firstName ?? "",
        email: data?.email ?? "",
        phoneNumber: data?.phoneNumber ?? data?.phone ?? "",
      };
    },
  },

  methods: {
    /**
     * Safe getter for wizardConfig.steps[stepKey].form.inputs[inputKey].id
     */
    getInputId(stepKey: string, inputKey: string): string {
      return (
        this.wizardStore.wizardConfig?.steps?.[stepKey as any]?.form?.inputs?.[inputKey as any]?.id ??
        ""
      );
    },

    /**
     * Return the first wizardData object found from the provided input IDs
     */
    firstWizardData(inputIds: string[]): AnyObj {
      for (const id of inputIds) {
        const value = (this.wizardStore.wizardData as AnyObj)?.[id];
        if (value) return value as AnyObj;
      }
      return {};
    },

    formatDate(value: any): string {
      if (!value) return "-";

      // Accept ISO strings, Date, or other parseable values.
      const d = value instanceof Date ? value : new Date(value);
      if (Number.isNaN(d.getTime())) return String(value);

      // Matches the screenshot style like "January 1, 2020"
      return new Intl.DateTimeFormat("en-US", {
        year: "numeric",
        month: "long",
        day: "numeric",
      }).format(d);
    },
  },
});
</script>
