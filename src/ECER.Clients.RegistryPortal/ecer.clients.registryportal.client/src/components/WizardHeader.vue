<template>
  <v-container fluid class="bg-white">
    <v-row>
      <v-col cols="12">
        <v-breadcrumbs :items="items" color="primary">
          <template #divider>/</template>
        </v-breadcrumbs>
      </v-col>
    </v-row>
    <v-row justify="space-between" class="pb-6">
      <v-col offset-md="1" cols="12" sm="8">
        <h3>{{ `Application for ECE ${certificationType} Certification` }}</h3>
        <div v-if="certificationType === 'Five Year'" role="doc-subtitle">{{ certificationTypeSubTitleForFiveYear }}</div>
      </v-col>
      <v-col v-if="false" cols="auto" offset="1">
        <v-btn class="mr-2" rounded="lg" variant="outlined" color="primary">Cancel Application</v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useWizardStore } from "@/store/wizard";

export default defineComponent({
  name: "WizardHeader",
  setup() {
    const wizardStore = useWizardStore();

    return {
      wizardStore,
    };
  },
  data: () => ({
    items: [
      {
        title: "Home",
        disabled: false,
        href: "/",
      },

      {
        title: "Apply New Certification",
        disabled: true,
        href: "application",
      },
    ],
  }),
  computed: {
    certificationType() {
      let certificationType = "";
      if (this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes("EceAssistant")) {
        certificationType = "Assistant";
      } else if (this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes("OneYear")) {
        certificationType = "One Year";
      } else if (
        this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes("FiveYears")
      ) {
        certificationType = "Five Year";
      }
      return certificationType;
    },
    certificationTypeSubTitleForFiveYear() {
      if (
        this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes("Ite") &&
        this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes("Sne")
      ) {
        return "Including certification for Special Needs Education (SNE) and Infant and Toddler Educator (ITE)";
      }

      if (this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes("Sne")) {
        return "Including certification for Special Needs Educator (SNE)";
      }
      if (this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes("Ite")) {
        return "Including certification for Infant and Toddler Eductor (ITE)";
      }

      return "";
    },
  },
});
</script>
