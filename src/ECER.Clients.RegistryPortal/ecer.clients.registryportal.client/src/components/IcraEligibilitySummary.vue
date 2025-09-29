<template>Not implemented you are viewing eligibility application id: {{ icraEligibilityId }}</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "IcraEligibilitySummary",
  props: {
    icraEligibilityId: {
      type: String,
      required: true,
    },
  },
});
</script>
