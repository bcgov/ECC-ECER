<template>
  <v-card variant="outlined" :color="cardColor" rounded="lg">
    <div class="float-right">
      <v-tooltip v-model="show" location="top">
        <template #activator="{ props }">
          <v-btn
            icon="mdi-pencil"
            v-bind="props"
            :color="isValid ? 'primary' : 'error'"
            variant="plain"
            @click="setWizard(referenceStage)"
          />
        </template>
        <span>Edit {{ title }}</span>
      </v-tooltip>
    </div>
    <v-container>
      <v-row align="center">
        <v-col>
          <h2 :class="isValid ? 'text-black' : 'text-error'">{{ title }}</h2>
          <div class="small text-black">{{ subtitle }}</div>
          <p v-if="!isValid" class="small text-error">
            <v-icon icon="mdi-alert-circle" color="error" class="mr-2"></v-icon>
            You must enter all required information in a valid format before
            submitting your application
          </p>
          <div class="mt-5 text-grey-dark"><slot name="content"></slot></div>
        </v-col>
      </v-row>
    </v-container>
  </v-card>
</template>

<script lang="ts">
import type { PropType } from "vue";
import { defineComponent } from "vue";

import { useApplicationStore } from "@/store/application";
import { useWizardStore } from "@/store/wizard";
import type { ReferenceStage } from "@/types/wizard";

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
    subtitle: {
      type: String,
      default: "",
    },
    referenceStage: {
      type: String as PropType<ReferenceStage>,
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
    setWizard(stage: ReferenceStage) {
      this.wizardStore.setCurrentStep(stage);
    },
  },
});
</script>
