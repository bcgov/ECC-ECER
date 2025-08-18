<!-- this component will render the child based on the outcome of the ICRA flag from dynamics -->
<template>
  <slot v-if="configStore.applicationConfiguration.icraFeatureEnabled"></slot>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useConfigStore } from "@/store/config";

export default defineComponent({
  name: "ConditionalRenderICRA",
  setup() {
    const configStore = useConfigStore();
    return { configStore };
  },
});
</script>
