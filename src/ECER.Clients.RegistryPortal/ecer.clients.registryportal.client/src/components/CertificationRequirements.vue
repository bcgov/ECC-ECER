<template>
  <v-container>
    <Breadcrumb :items="items" />
    <div v-for="certificationType in certificationTypes" :key="certificationType">
      <ECEAssistantRenewalRequirements v-if="certificationType === CertificationType.ECE_ASSISTANT" />
      <ECEOneYearRenewalRequirements v-if="certificationType === CertificationType.ONE_YEAR" />
      <ECEFiveYearRenewalRequirements v-if="certificationType === CertificationType.FIVE_YEAR" />
    </div>
  </v-container>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEAssistantRenewalRequirements from "@/components/ECEAssistantRenewalRequirements.vue";
import ECEFiveYearRenewalRequirements from "@/components/ECEFiveYearRenewalRequirements.vue";
import ECEOneYearRenewalRequirements from "@/components/ECEOneYearRenewalRequirements.vue";
import type { Components } from "@/types/openapi";
import { CertificationType } from "@/utils/constant";

export default defineComponent({
  name: "CertificationTypeRequirements",
  components: {
    ECEAssistantRenewalRequirements,
    ECEOneYearRenewalRequirements,
    ECEFiveYearRenewalRequirements,
    Breadcrumb,
  },
  props: {
    certificationTypes: {
      type: Array as PropType<Components.Schemas.CertificationType[]>,
      required: true,
    },
  },
  setup: () => {
    return { CertificationType };
  },
  data() {
    return {
      items: [
        {
          title: "Home",
          disabled: false,
          href: "/",
        },
        {
          title: "Application types",
          disabled: false,
          href: "/application/certification",
        },
        {
          title: "Requirements",
          disabled: true,
          href: "/application/certification/requirements",
        },
      ],
    };
  },
});
</script>
