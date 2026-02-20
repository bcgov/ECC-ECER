<template>Component ID:{{ componentGroupId }}</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ProgramApplicationComponent",
  components: {},
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
    componentGroupId: {
      type: String,
      required: true,
    },
  },
  setup() {},
});
</script>
<style scoped></style>
