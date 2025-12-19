<template>
  <div>
    <ReferencePreviewCard :is-valid="true" title="Application type" class="mb-6" reference-stage="CertificationType">
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

    <ReferencePreviewCard :is-valid="true" title="Education" class="mb-6" reference-stage="Education">
      <template #content>
        <div v-if="education.educationalInstitutionName " class="mb-4">
          <p class="small font-weight-bold mb-0">{{ education.educationalInstitutionName  }}</p>
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
          <v-col cols="8">
            <p class="small font-weight-bold">{{ education.country?.countryName || "-" }}</p>
          </v-col>
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

    <ReferencePreviewCard :is-valid="true" title="Character reference" class="mb-6" reference-stage="CharacterReferences">
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
          <v-col cols="8"><p class="small font-weight-bold">{{ characterReference.emailAddress || "-" }}</p></v-col>
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
    wizardData(): any {
      return this.wizardStore.wizardData as any;
    },

    educationSource(): any {
      const list = this.wizardData?.educationList;
      if (!list || typeof list !== "object") return {};

      const numericKeys = Object.keys(list).filter((k) => !Number.isNaN(Number(k)));
      if (!numericKeys.length) return {};

      const highest = numericKeys
        .map((k) => Number(k))
        .sort((a, b) => a - b)
        .pop();

      return highest !== undefined ? list[String(highest)] : {};
    },


    characterReferenceSource(): any {
      const list = this.wizardData?.characterReferences;
      if (Array.isArray(list)) {
        return list.length ? list[list.length - 1] : {};
      }
      return list && typeof list === "object" ? list : {};
    },
    applicationType(): { certificationType: string } {
      return { certificationType: "ECE Five Year" };
    },

    education(): {
      educationalInstitutionName : string;
      programName: string;
      startDate: any;
      endDate: any;
      country: any;
      campusLocation: string;
      studentNumber: string;
      fullNameOnTranscript: string;
    } {
      const e = this.educationSource;

      return {
        educationalInstitutionName : e?.educationalInstitutionName  ?? "",
        programName: e?.programName ?? "",
        startDate: e?.startDate ?? "",
        endDate: e?.endDate ?? "",
        country: e?.country ?? null,
        campusLocation: e?.campusLocation ?? "",
        studentNumber: e?.studentNumber ?? "",
        fullNameOnTranscript: e?.fullNameOnTranscript ?? "",
      };
    },

    characterReference(): {
      lastName: string;
      firstName: string;
      emailAddress: string;
      phoneNumber: string;
    } {
      const r = this.characterReferenceSource || {};
      return {
        lastName: r?.lastName ?? "",
        firstName: r?.firstName ?? "",
        emailAddress: r?.emailAddress ?? "",
        phoneNumber: r?.phoneNumber ?? "",
      };
    },
  },

  methods: {
    formatDate(value: any): string {
      if (!value) return "-";
      const d = value instanceof Date ? value : new Date(value);
      if (Number.isNaN(d.getTime())) return String(value);

      return new Intl.DateTimeFormat("en-US", {
        year: "numeric",
        month: "long",
        day: "numeric",
        timeZone: "UTC",
      }).format(d);
    },
  },


});
</script>
