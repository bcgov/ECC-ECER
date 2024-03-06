<template>
  <PreviewCard>
    <v-container>
      <v-row align="center">
        <v-col>
          <h3 class="font-black">Character Reference</h3>
        </v-col>
        <v-col align="end">
          <v-btn v-bind="props" icon="mdi-pencil" color="primary" variant="plain" />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Reference Last Name</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ referenceLastName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Reference First Name</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ referenceFirstName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Reference Email</p>
        </v-col>
        <v-col>
          <p class="small font>weight-bold">{{ referenceEmail }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Referece Phone Number</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ referencePhoneNumber }}</p>
        </v-col>
      </v-row>
    </v-container>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useWizardStore } from "@/store/wizard";
import type { EcePreviewProps } from "@/types/input";

export default defineComponent({
  name: "EceCharaterReferencePreview",
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
    referenceLastName() {
      return "—";
    },
    referenceFirstName() {
      return "—";
    },
    referenceEmail() {
      return "—";
    },
    referencePhoneNumber() {
      return "—";
    },
  },
});
</script>
