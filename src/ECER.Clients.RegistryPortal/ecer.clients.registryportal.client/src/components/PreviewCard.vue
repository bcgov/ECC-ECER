<template>
  <v-card variant="outlined" :color="cardColor" rounded="lg">
    <v-card-title>
      <div class="d-flex justify-space-between align-center">
        <div>
          <h2 class="text-wrap" :class="isValid ? 'text-black' : 'text-error'">{{ title }}</h2>
          <p v-if="!isValid" class="small text-error text-wrap">
            <v-icon icon="mdi-alert-circle" color="error" class="mr-2"></v-icon>
            You must enter all required information in a valid format before submitting your application
          </p>
        </div>
        <div>
          <v-tooltip v-model="show" location="top">
            <template #activator="{ props }">
              <v-btn icon="mdi-pencil" v-bind="props" :color="isValid ? 'primary' : 'error'" variant="plain" @click="setWizard(portalStage)" />
            </template>
            <span>Edit {{ title }}</span>
          </v-tooltip>
        </div>
      </div>
    </v-card-title>
    <v-card-text class="text-grey-dark">
      <slot name="content"></slot>
    </v-card-text>
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
  data: () => ({
    show: false,
  }),
  computed: {
    cardColor() {
      return this.isValid ? "grey-lightest" : "error";
    },
  },
  methods: {
    setWizard(stage: Components.Schemas.PortalStage) {
      this.wizardStore.setCurrentStep(stage);
      this.applicationStore.draftApplication.stage = stage;
    },
  },
});
</script>
