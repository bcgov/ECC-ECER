<template>
  <PreviewCard title="Employment experience" portal-stage="EmploymentExperience">
    <template #content>
      <!-- TODO: Add content -->
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useWizardStore } from "@/store/wizard";

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
    // TODO: Add computed properties
  },
});
</script>
