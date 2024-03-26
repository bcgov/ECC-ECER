<template>
  <v-card variant="outlined" :color="cardColor" rounded="lg">
    <div class="float-right">
      <v-btn icon="mdi-pencil" :color="isValid ? 'primary' : 'error'" variant="plain" @click="setWizard(portalStage)" />
    </div>
    <v-container>
      <v-row align="center">
        <v-col>
          <h3 :class="isValid ? 'text-black' : 'text-error'">{{ title }}</h3>
          <p v-if="!isValid" class="small text-error">
            <v-icon icon="mdi-alert-circle" color="error" class="mr-2"></v-icon>
            You must enter all required information in a valid format before submitting your application
          </p>
        </v-col>
      </v-row>
      <slot name="content"></slot>
    </v-container>
  </v-card>
</template>

<script lang="ts">
import type { PropType } from "vue";
import { defineComponent } from "vue";

import { useApplicationStore } from "@/store/application";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "PreviewCard",
  props: {
    isValid: {
      type: Boolean,
      default: true,
    },
    title: {
      type: String,
      required: true,
    },
    portalStage: {
      type: String as PropType<Components.Schemas.PortalStage>,
      required: true,
    },
  },
  setup: () => {
    const wizardStore = useWizardStore();
    const applicationStore = useApplicationStore();
    return {
      wizardStore,
      applicationStore,
    };
  },
  computed: {
    cardColor() {
      return this.isValid ? "grey-lightest" : "error";
    },
  },
  methods: {
    setWizard(stage: Components.Schemas.PortalStage) {
      this.wizardStore.setCurrentStep(stage);
      this.applicationStore.draftApplication.stage = this.wizardStore.currentStepStage;
    },
  },
});
</script>
