<template>
  <ECEHeader title="Specialized certification options" />
  <div class="d-flex flex-column ga-3 my-6">
    <p>
      If you've completed a
      <a
        href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/recognized-ece-institutions"
      >
        specialized program recognized by the ECE Registry
      </a>
      you can add these to your application.
    </p>
    <p>
      You'll need to request an official transcript from your educational institution. It must be sent directly from the educational institute to the ECE
      Registry.
    </p>
    <p>Do you want to apply for Infant and Toddler Educator (ITE) certification?</p>
    <v-radio-group
      :model-value="applicationStore.isDraftCertificateTypeIte"
      hide-details="auto"
      :rules="[Rules.requiredRadio('Select an option')]"
      @update:model-value="handleIteSelection"
    >
      <v-radio label="Yes - and I've completed an infant and toddler educator training program" :value="true"></v-radio>
      <v-radio label="No" :value="false"></v-radio>
    </v-radio-group>
    <p>Do you want to apply for Special Needs Educator (SNE) certification?</p>
    <v-radio-group
      :model-value="applicationStore.isDraftCertificateTypeSne"
      hide-details="auto"
      :rules="[Rules.requiredRadio('Select an option')]"
      @update:model-value="handleSneSelection"
    >
      <v-radio label="Yes - and I've completed a special needs early childhood educator training program" :value="true"></v-radio>
      <v-radio label="No" :value="false"></v-radio>
    </v-radio-group>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ECEHeader from "@/components/ECEHeader.vue";
import { useApplicationStore } from "@/store/application";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "ECEOneYearRenewalRequirements",
  components: { ECEHeader },

  setup() {
    const applicationStore = useApplicationStore();

    return { applicationStore };
  },
  data() {
    return {
      Rules,
    };
  },

  methods: {
    handleIteSelection(selection: boolean | null) {
      if (selection) {
        this.applicationStore.draftApplication.certificationTypes?.push("Ite");
      } else {
        this.applicationStore.draftApplication.certificationTypes = this.applicationStore.draftApplication.certificationTypes?.filter(
          (certificationType) => certificationType !== "Ite",
        );
      }
    },
    handleSneSelection(selection: boolean | null) {
      if (selection) {
        this.applicationStore.draftApplication.certificationTypes?.push("Sne");
      } else {
        this.applicationStore.draftApplication.certificationTypes = this.applicationStore.draftApplication.certificationTypes?.filter(
          (certificationType) => certificationType !== "Sne",
        );
      }
    },
  },
});
</script>
