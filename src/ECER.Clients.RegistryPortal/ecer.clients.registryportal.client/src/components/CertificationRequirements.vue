<template>
  <v-container>
    <Breadcrumb :items="items" />
    <div v-for="certificationType in certificationTypes" :key="certificationType">
      <template v-if="certificationType === CertificationType.ECE_ASSISTANT">
        <ECEAssistantRequirements v-if="!isRenewal" />
        <ECEAssistantRenewalRequirements v-else />
      </template>
      <template v-if="certificationType === CertificationType.ONE_YEAR">
        <ECEOneYearRequirements v-if="!isRenewal" />
        <ECEOneYearRenewalRequirements v-else />
      </template>
      <template v-if="certificationType === CertificationType.FIVE_YEAR">
        <ECEFiveYearRequirements v-if="!isRenewal" />
        <ECEFiveYearRenewalRequirements v-else />
      </template>
    </div>
  </v-container>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEAssistantRenewalRequirements from "@/components/ECEAssistantRenewalRequirements.vue";
import ECEAssistantRequirements from "@/components/ECEAssistantRequirements.vue";
import ECEFiveYearRenewalRequirements from "@/components/ECEFiveYearRenewalRequirements.vue";
import ECEFiveYearRequirements from "@/components/ECEFiveYearRequirements.vue";
import ECEOneYearRenewalRequirements from "@/components/ECEOneYearRenewalRequirements.vue";
import ECEOneYearRequirements from "@/components/ECEOneYearRequirements.vue";
import type { Components } from "@/types/openapi";
import { CertificationType } from "@/utils/constant";

export default defineComponent({
  name: "CertificationTypeRequirements",
  components: {
    ECEAssistantRequirements,
    ECEOneYearRequirements,
    ECEFiveYearRequirements,
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
    isRenewal: {
      type: Boolean,
      default: false,
    },
  },
  setup: () => {
    return { CertificationType };
  },
  data(props) {
    const items = props.isRenewal
      ? [
          {
            title: "Home",
            disabled: false,
            href: "/",
          },
          {
            title: "Renew",
            disabled: true,
            href: "/application/certification/requirements",
          },
        ]
      : [
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
        ];

    return { items };
  },
});
</script>
