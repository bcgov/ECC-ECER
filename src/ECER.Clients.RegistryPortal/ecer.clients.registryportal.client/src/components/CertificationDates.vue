<template>
  <p :class="{ 'font-weight-bold': isBold }">Effective date: {{ formattedEffectiveDate }}</p>
  <p :class="{ 'font-weight-bold': isBold }">Expiry date: {{ formattedExpiryDate }}</p>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import { formatDate } from "@/utils/format";

import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "CertificationDates",
  props: {
    certification: {
      type: Object as PropType<Components.Schemas.Certification>,
      required: true,
    },
    isBold: {
      type: Boolean,
      required: false,
      default: true,
    },
  },
  computed: {
    formattedExpiryDate(): string {
      return formatDate(this.certification.expiryDate ?? "", "LLLL d, yyyy");
    },
    formattedEffectiveDate(): string {
      return formatDate(this.certification.effectiveDate ?? "", "LLLL d, yyyy");
    },
  },
});
</script>
