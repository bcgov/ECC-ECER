<template>
  <PreviewCard title="Education" portal-stage="Education">
    <template #content>
      <div v-for="(education, id, index) in educations" :key="id">
        <v-divider v-if="index !== 0" :thickness="2" color="grey-lightest" class="border-opacity-100 my-6" />
        <v-row>
          <v-col>
            <h4 class="text-black">{{ education.educationalInstitutionName }}</h4>
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
            <p class="small">Your full name as shown on transcript</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ studentFullName(education) }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Student ID or number</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ education.studentNumber }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Language of institution</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ education.languageofInstruction }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Start date of program</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ formatDate(education.startDate, "LLLL d, yyyy") }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">End date of program</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ formatDate(education.endDate, "LLLL d, yyyy") }}</p>
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
    studentFullName(education: Components.Schemas.Transcript) {
      let fullName = education.studentFirstName;
      if (education.studentMiddleName) {
        fullName += ` ${education.studentMiddleName}`;
      }
      fullName += ` ${education.studentLastName}`;

      return fullName;
    },
  },
});
</script>
