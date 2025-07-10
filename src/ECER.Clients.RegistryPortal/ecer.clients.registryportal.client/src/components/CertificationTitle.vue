<template>Early Childhood Educator - {{ titleArray.join(" ") }}</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "CertificationTitle",
  props: {
    certification: {
      type: Object as PropType<Components.Schemas.Certification>,
      required: true,
    },
  },
  computed: {
    titleArray() {
      if (!this.certification.levels) return [];
      return this.certification.levels
        ?.map((level: Components.Schemas.CertificationLevel) => {
          switch (level.type) {
            case "ITE":
              return "+ Infant and Toddler Educator (ITE)";
            case "SNE":
              return "+ Special Needs Educator (SNE)";
            case "ECE 1 YR":
              return "ECE One Year";
            case "ECE 5 YR":
              return "ECE Five Year";
            case "Assistant":
              return "ECE Assistant";
            default:
              return "";
          }
        })
        .sort((a: string, b: string) => {
          // Move strings starting with '+' to the end of the array
          if (a.startsWith("+") && !b.startsWith("+")) {
            return 1;
          } else if (!a.startsWith("+") && b.startsWith("+")) {
            return -1;
          } else {
            return 0;
          }
        });
    },
  },
});
</script>
