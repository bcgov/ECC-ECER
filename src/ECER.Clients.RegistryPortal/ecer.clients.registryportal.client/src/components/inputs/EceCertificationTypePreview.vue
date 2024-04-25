<template>
  <PreviewCard :is-valid="wizardStore.validationState.CertificationType" title="Certification Selection" portal-stage="CertificationType">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Certification Type</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ certificationType }}</p>
        </v-col>
      </v-row>
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useWizardStore } from "@/store/wizard";
import type { EcePreviewProps } from "@/types/input";
export default defineComponent({
  name: "EceCertificationTypePreview",
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
          certificationType += " and Special Needs Educator (SNE)";
        }
        if (this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes("Ite")) {
          certificationType += " and Infant and Toddler Educator (ITE)";
        }
      }
      return certificationType;
    },
  },
});
</script>
