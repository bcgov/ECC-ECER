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
        <ApplicationCertificationTypeHeader
          :certification-types="wizardStore.wizardData[wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id]"
        />
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

import ApplicationCertificationTypeHeader from "./ApplicationCertificationTypeHeader.vue";

export default defineComponent({
  name: "WizardHeader",
  components: { ApplicationCertificationTypeHeader },
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
});
</script>
