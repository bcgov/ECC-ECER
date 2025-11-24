<template>
  <PreviewCard title="Employment experience references" portal-stage="EmploymentExperience">
    <template #content>
      <div v-for="(experience, index) in employmentExperiences" :key="index">
        <v-divider v-if="index !== 0" :thickness="2" color="grey-lightest" class="border-opacity-100 my-6" />
        <v-row>
          <v-col cols="4">
            <p class="small">Reference last name</p>
          </v-col>
          <v-col>
            <p id="employmentExperienceLastName" class="small font-weight-bold">{{ experience.lastName }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Reference first name</p>
          </v-col>
          <v-col>
            <p id="employmentExperienceLastName" class="small font-weight-bold">{{ experience.firstName }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Reference email</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ experience.emailAddress }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Reference phone number (optional)</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ experience?.phoneNumber || "—" }}</p>
          </v-col>
        </v-row>
      </div>
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "EceEmploymentExperiencePreview",
  components: {
    PreviewCard,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    return {
      wizardStore,
    };
  },
  computed: {
    employmentExperiences(): Components.Schemas.EmploymentReference[] {
      return this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps?.employmentExperience?.form?.inputs?.employmentExperience?.id || ""];
    },
  },
});
</script>
