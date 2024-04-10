<template>
  <v-container fluid class="bg-white">
    <v-row>
      <v-col cols="12">
        <v-breadcrumbs :items="items" color="primary">
          <template #divider>
            <v-icon icon="mdi-chevron-right" size="sm"></v-icon>
          </template>
        </v-breadcrumbs>
      </v-col>
    </v-row>
    <v-row justify="space-between" class="pb-6">
      <v-col offset-md="1" cols="12" sm="8">
        <h3>{{ `Application for ${certificationType} Certification` }}</h3>
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
  data: () => ({
    items: [
      {
        title: "Dashboard",
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
  setup() {
    const wizardStore = useWizardStore();

    return {
      wizardStore,
    };
  },
  computed: {
    certificationType() {
      let certificationType = "";
      if (this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes("EceAssistant")) {
        certificationType = "ECE Assistant";
      } else if (this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes("OneYear")) {
        certificationType = "One Year";
      } else if (
        this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes("FiveYears")
      ) {
        certificationType = "Five Year";

        if (this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes("Sne")) {
          certificationType += " & Special Needs Educator (SNE)";
        }
        if (this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes("Ite")) {
          certificationType += " & Infant and Toddle Educator (ITE)";
        }
      } else {
        certificationType += "ECE Assistant";
      }
      return certificationType;
    },
  },
});
</script>
