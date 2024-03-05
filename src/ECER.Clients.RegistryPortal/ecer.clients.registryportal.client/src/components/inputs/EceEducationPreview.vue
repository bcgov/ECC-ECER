<template>
  <PreviewCard>
    <v-container>
      <v-row align="center">
        <v-col>
          <h3>Education</h3>
        </v-col>
        <v-col align="end">
          <v-btn v-bind="props" icon="mdi-pencil" color="primary" variant="plain" />
        </v-col>
      </v-row>
      <div v-for="(education, index) in transcriptList" :key="index">
        <v-divider v-if="index !== 0" :thickness="2" color="grey-lightest" class="border-opacity-100 my-6" />
        <v-row>
          <v-col>
            <h4 class="text-black">{{ education.educationalInstitutionName }}</h4>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Name of Program</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ education.programName }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Campus Location</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ education.campusLocation }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Student Name</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ education.studentName }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Student Number</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ education.studentNumber }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Language of Instruction</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ education.languageofInstruction }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Start Date of Program</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ education.startDate }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">End Date of Program</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ education.endDate }}</p>
          </v-col>
        </v-row>
      </div>
    </v-container>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useWizardStore } from "@/store/wizard";
import type { EcePreviewProps } from "@/types/input";

//////////////////////// TODO REMOVE THIS TEST DATA ////////////////////////
interface Transcript {
  id?: string | null;
  educationalInstitutionName?: string | null;
  programName?: string | null;
  campusLocation?: string | null;
  studentName?: string | null;
  studentNumber?: string | null;
  languageofInstruction?: string | null;
  startDate?: string; // date-time
  endDate?: string; // date-time
}

const transcriptList: Transcript[] = [
  {
    educationalInstitutionName: "University of Alberta",
    programName: "Bachelor of Science in Computer Science",
    campusLocation: "Edmonton",
    studentName: "John Doe",
    studentNumber: "123456789",
    languageofInstruction: "English",
    startDate: "2021-01-01T00:00:00Z",
    endDate: "2025-01-01T00:00:00Z",
  },
  {
    educationalInstitutionName: "University of British Columbia",
    programName: "Bachelor of Psychology",
    campusLocation: "Vancouver",
    studentName: "Jane Doe",
    studentNumber: "987654321",
    languageofInstruction: "English",
    startDate: "2021-01-01T00:00:00Z",
    endDate: "2025-01-01T00:00:00Z",
  },
];
//////////////////////// TODO REMOVE THIS TEST DATA ////////////////////////

export default defineComponent({
  name: "EceEducationPreview",
  components: {
    PreviewCard,
  },
  props: {
    props: {
      type: Object as () => EcePreviewProps,
      required: true,
    },
  },
  setup: () => {
    const wizardStore = useWizardStore();

    return {
      wizardStore,
      transcriptList,
    };
  },
});
</script>
