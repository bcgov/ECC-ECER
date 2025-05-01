<template>
  <PreviewCard title="TODO" portal-stage="CertificateInformation" :editable="false">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">PLACEHOLDER FOR CERTIFICATE INFORMATION PREVIEW COMPONENT</p>
        </v-col>
        <v-col>
          <p id="certificateInformation" class="small font-weight-bold">TODO</p>
        </v-col>
      </v-row>
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
export default defineComponent({
  name: "EceCertificateInformationPreview",
  components: {
    PreviewCard,
  },
});
</script>
