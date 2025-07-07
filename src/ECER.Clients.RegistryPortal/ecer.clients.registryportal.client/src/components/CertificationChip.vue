<template>
  <v-chip :color="chipColor" variant="flat" size="small">{{ chipText }}</v-chip>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "CertificationChip",
  props: {
    certification: {
      type: Object as PropType<Components.Schemas.Certification>,
      required: true,
    },
  },
  computed: {
    chipText() {
      let text;
      switch (this.certification.statusCode) {
        case "Active":
        case "Renewed":
        case "Reprinted":
          text = "Active";
          break;
        case "Expired":
        case "Inactive":
          text = "Expired";
          break;
        case "Cancelled":
          text = "Cancelled";
          break;
        case "Suspended":
          text = "Suspended";
          break;
        default:
          text = "Expired";
      }

      if (this.certification.hasConditions) {
        text += " with terms and conditions";
      }

      return text;
    },
    chipColor(): string {
      // "success" | "error" | "warning" | "info"
      switch (this.chipText) {
        case "Active":
        case "Active with terms and conditions":
          return "success";
        case "Expired":
        case "Expired with terms and conditions":
          return "error";
        case "Cancelled":
        case "Cancelled with terms and conditions":
        case "Suspended":
        case "Suspended with terms and conditions":
          return "grey-darkest";
        default:
          return "grey-darkest";
      }
    },
  },
});
</script>
