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
import { CertificationType } from "@/utils/constant";
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
      if (
        this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes(
          CertificationType.ECE_ASSISTANT,
        )
      ) {
        certificationType = "ECE Assistant";
      } else if (
        this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes(
          CertificationType.ONE_YEAR,
        )
      ) {
        certificationType = "One Year";
      } else if (
        this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes(
          CertificationType.FIVE_YEAR,
        )
      ) {
        certificationType = "Five Year";

        if (
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes(
            CertificationType.SNE,
          )
        ) {
          certificationType += " and Special Needs Educator (SNE)";
        }
        if (
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].includes(
            CertificationType.ITE,
          )
        ) {
          certificationType += " and Infant and Toddler Educator (ITE)";
        }
      }
      return certificationType;
    },
  },
});
</script>
