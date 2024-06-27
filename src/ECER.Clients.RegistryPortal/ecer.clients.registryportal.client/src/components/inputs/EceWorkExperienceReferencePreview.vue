<template>
  <PreviewCard title="Work Experience References" portal-stage="WorkReferences">
    <template #content>
      <div v-for="(experience, id, index) in references" :key="id">
        <v-divider v-if="index !== 0" :thickness="2" color="grey-lightest" class="border-opacity-100 my-6" />
        <v-row>
          <v-col>
            <h4 class="text-black">{{ experience.firstName }} {{ experience.lastName }}</h4>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Email</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ experience.emailAddress }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Phone Number</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ experience.phoneNumber }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Work Experience Hours</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ experience.hours }}</p>
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
import type { EcePreviewProps } from "@/types/input";
import type { Components } from "@/types/openapi";

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
    };
  },
  computed: {
    references(): { [id: string]: Components.Schemas.WorkExperienceReference } {
      return this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.workReference.form.inputs.referenceList.id];
    },
  },
});
</script>
