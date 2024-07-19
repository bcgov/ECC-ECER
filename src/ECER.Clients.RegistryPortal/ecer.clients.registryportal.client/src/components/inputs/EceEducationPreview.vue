<template>
  <PreviewCard title="Education" portal-stage="Education">
    <template #content>
      <div v-for="(education, id, index) in educations" :key="id">
        <v-divider v-if="index !== 0" :thickness="2" color="grey-lightest" class="border-opacity-100 my-6" />
        <v-row>
          <v-col>
            <h4>{{ education.educationalInstitutionName }}</h4>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">{{ "Name of program or course" }}</p>
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
            <p class="small font-weight-bold">{{ formatDate(education.startDate) }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">End Date of Program</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ formatDate(education.endDate) }}</p>
          </v-col>
        </v-row>
      </div>
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useApplicationStore } from "@/store/application";
import { useWizardStore } from "@/store/wizard";
import type { EcePreviewProps } from "@/types/input";
import type { Components } from "@/types/openapi";
import { formatDate } from "@/utils/format";

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
    const applicationStore = useApplicationStore();
    return {
      wizardStore,
      applicationStore,
    };
  },
  computed: {
    educations(): { [id: string]: Components.Schemas.Transcript } {
      return this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.education.form.inputs.educationList.id];
    },
  },
  methods: {
    formatDate,
  },
});
</script>
