<template>
  <PreviewCard></PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import type { EcePreviewProps } from "@/types/input";

export default defineComponent({
  name: "EceCharacterReferencePreview",
  components: {
    PreviewCard,
  },
  props: {
    props: {
      type: Object as () => EcePreviewProps,
      required: true,
    },
  },
});
</script>
