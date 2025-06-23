<template>
  <ECEHeader title="Specialized certification options" />
  <div class="d-flex flex-column ga-3 my-6">
    <template v-if="!isLaborMobility">
      <p>There are 2 types of specialized certifications in B.C. that you can add to your ECE Five Year certificate.</p>
      <ol class="ml-10">
        <li>Infant and Toddler Educator (ITE) - you must have completed an infant and toddler educator training program</li>
        <li>Special Needs Educator (SNE) - you must have completed a special needs early childhood educator training program</li>
      </ol>
      <p>The program(s) must be either:</p>
      <ul class="ml-10">
        <li>
          <a
            target="_blank"
            href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/recognized-ece-institutions"
          >
            Recognized
          </a>
          by the ECE Registry
        </li>
        <li>Considered equivalent by the ECE Registry</li>
      </ul>
      <p>
        You'll need to request an official transcript from your educational institution. It must be sent directly from the educational institution to the ECE
        Registry.
      </p>
      <p class="mt-6">What do you want to add to your certificate?</p>
    </template>
    <template v-if="isLaborMobility">
      <p>You are also eligible to apply for the following specialized certificates as part of your out-of-province transfer application:</p>
      <ol class="ml-10">
        <li>Infant and Toddler Educator (ITE)</li>
        <ul class="bullet-list ml-10">
          <li>allows you to work alone and/or as the primary educator with children birth to 5 years</li>
        </ul>
        <li>Special Needs Educator (SNE)</li>
        <ul class="bullet-list ml-10">
          <li>alone and/or as the primary educator in inclusive settings with children 3-5 years of age</li>
        </ul>
      </ol>
      <p class="mt-6">Please confirm the specialized certificates you want to add to your ECE Five Year:</p>
    </template>
    <v-form ref="specializationForm" validate-on="lazy">
      <v-checkbox
        v-model="selected"
        class="ml-n2"
        value="Ite"
        color="primary"
        label="Add Infant and Toddler Educator (ITE)"
        hide-details="auto"
        @update:model-value="handleSelection"
      ></v-checkbox>

      <v-checkbox
        v-model="selected"
        class="ml-n2"
        value="Sne"
        color="primary"
        label="Add Special Needs Educator (SNE)"
        hide-details="auto"
        @update:model-value="handleSelection"
      ></v-checkbox>
      <v-input
        v-if="applicationStore.draftApplication.certificationTypes?.length === 0"
        auto-hide="auto"
        :model-value="selected"
        :rules="[hasSelection]"
      ></v-input>
    </v-form>
  </div>
</template>

<script lang="ts">
import { defineComponent, inject } from "vue";

import ECEHeader from "@/components/ECEHeader.vue";
import { useApplicationStore } from "@/store/application";
import type { Components } from "@/types/openapi";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "ECEOneYearRenewalRequirements",
  components: { ECEHeader },

  setup() {
    const applicationStore = useApplicationStore();

    const handleSpecializationSelection = inject<(newValue: Components.Schemas.CertificationType[]) => void>("handleSpecializationSelection");

    return { applicationStore, handleSpecializationSelection };
  },
  props: {
    preSelected: {
      type: Array as () => Components.Schemas.CertificationType[],
      default: () => [],
    },
    isLaborMobility: {
      type: Boolean,
      default: false,
    },
  },
  mounted() {
    // we need to seed the preselected options for application.vue
    if (this.preSelected.length > 0 && this.handleSpecializationSelection) {
      this.handleSpecializationSelection(this.preSelected);
    }
  },
  data() {
    return {
      Rules,
      selected: [...this.preSelected] as Components.Schemas.CertificationType[],
    };
  },

  computed: {
    handleSelection() {
      return () => {
        if (this.handleSpecializationSelection) {
          this.handleSpecializationSelection(this.selected);
        }
      };
    },
    hasSelection() {
      return () => {
        if (this.selected.length > 0) {
          return true;
        }
        return "You must select at least one specialized certification to apply for.";
      };
    },
  },
});
</script>
<style scoped>
.bullet-list {
  list-style-type: disc;
}
</style>
