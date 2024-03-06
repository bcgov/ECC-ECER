<template>
  <PreviewCard>
    <v-container>
      <v-row align="center">
        <v-col>
          <h3>Work Experience References</h3>
        </v-col>
        <v-col align="end">
          <v-btn v-bind="props" icon="mdi-pencil" color="primary" variant="plain" />
        </v-col>
      </v-row>
      <div v-for="(experience, index) in workExperienceList" :key="index">
        <v-divider v-if="index !== 0" :thickness="2" color="grey-lightest" class="border-opacity-100 my-6" />
        <v-row>
          <v-col>
            <h4 class="text-black">{{ experience.referenceName }}</h4>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Email</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ experience.referenceEmail }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Phone Number</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ experience.referencePhoneNumber }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Work Experience Hours</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ experience.workExperienceHours }}</p>
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
interface WorkExperience {
  id?: string | null;
  referenceName?: string | null;
  referenceEmail?: string | null;
  referencePhoneNumber?: string | null;
  workExperienceHours?: string | null;
}

const workExperienceList: WorkExperience[] = [
  {
    referenceName: "John Doe",
    referenceEmail: "johndoe@homtmail.com",
    referencePhoneNumber: "1234567890",
    workExperienceHours: "150",
  },
  {
    referenceName: "Jane Doe",
    referenceEmail: "janedoe@shaw.ca",
    referencePhoneNumber: "0987654321",
    workExperienceHours: "200",
  },
];
//////////////////////// TODO REMOVE THIS TEST DATA ////////////////////////

export default defineComponent({
  name: "EceWorkExperienceReferencePreview",
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
      workExperienceList,
    };
  },
});
</script>
