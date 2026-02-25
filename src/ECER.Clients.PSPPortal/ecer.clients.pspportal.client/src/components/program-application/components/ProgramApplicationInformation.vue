<template>Info component</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ProgramApplicationInformation",
  components: {},
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
  },
  setup() {},
});
</script>
