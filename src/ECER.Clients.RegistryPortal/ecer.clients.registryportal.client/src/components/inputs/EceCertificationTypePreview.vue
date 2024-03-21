<template>
  <PreviewCard>
    <v-container>
      <v-row align="center">
        <v-col>
          <h3 class="font-black">Certification Selection</h3>
        </v-col>
        <v-col align="end">
          <v-btn @click="setWizard('CertificationType')" v-bind="props" icon="mdi-pencil" color="primary" variant="plain" />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Certification Type</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ certificationType }}</p>
        </v-col>
      </v-row>
    </v-container>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useApplicationStore } from "@/store/application";
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
    const applicationStore = useApplicationStore();
    return {
      wizardStore,applicationStore,
    };
  },
  methods:{
    setWizard(stage:string) {
      this.wizardStore.setCurrentStep(stage);
      this.applicationStore.draftApplication.stage = this.wizardStore.currentStepStage;
    },
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
      }
      return certificationType;
    },
  },
});
</script>
