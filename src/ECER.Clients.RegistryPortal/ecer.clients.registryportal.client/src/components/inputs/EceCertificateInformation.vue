<template>
  <v-row>
    <v-col>
      <h1>PLACEHOLDER FOR CERTIFICATE INFORMATION INPUT COMPONENT</h1>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "EceCertificateInformation",
  props: {
    modelValue: {
      type: String,
      required: true,
    },
  },
});
</script>
