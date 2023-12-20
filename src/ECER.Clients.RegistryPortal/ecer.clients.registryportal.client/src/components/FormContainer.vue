<template>
  <div :style="{ 'max-width': '600px' }" class="px-2">
    <slot> </slot>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "FormContainer",
});
</script>
