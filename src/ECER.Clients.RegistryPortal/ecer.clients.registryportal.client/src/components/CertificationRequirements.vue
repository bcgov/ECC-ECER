<template>
  <v-container>
    <Breadcrumb :items="items" />
    <div v-for="certificationType in certificationTypes" :key="certificationType">
      <template v-if="certificationType === CertificationType.ECE_ASSISTANT">
        <ECEAssistantRenewalRequirements v-if="isRenewal" />
        <ECEAssistantLaborMobilityRequirements v-else-if="isLaborMobility" />
        <ECEAssistantRequirements v-else />
      </template>
      <template v-if="certificationType === CertificationType.ONE_YEAR">
        <ECEOneYearRenewalRequirements v-if="isRenewal" />
        <ECEOneYearLaborMobilityRequirements v-else-if="isLaborMobility" />
        <ECEOneYearRequirements v-else />
      </template>
      <template v-if="certificationType === CertificationType.FIVE_YEAR">
        <ECEFiveYearRenewalRequirements v-if="isRenewal" />
        <ECEFiveYearLaborMobilityRequirements v-else-if="isLaborMobility" />
        <ECEFiveYearRequirements v-else />
      </template>
    </div>
  </v-container>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEAssistantRenewalRequirements from "@/components/ECEAssistantRenewalRequirements.vue";
import ECEAssistantLaborMobilityRequirements from "@/components/ECEAssistantLaborMobilityRequirements.vue";
import ECEOneYearLaborMobilityRequirements from "@/components/ECEOneYearLaborMobilityRequirements.vue";
import ECEFiveYearLaborMobilityRequirements from "@/components/ECEFiveYearLaborMobilityRequirements.vue";
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
    ECEAssistantLaborMobilityRequirements,
    ECEOneYearLaborMobilityRequirements,
    ECEFiveYearLaborMobilityRequirements,
    Breadcrumb,
  },
  props: {
    certificationTypes: {
      type: Array as PropType<Components.Schemas.CertificationType[]>,
      required: true,
    },
    applicationType: {
      type: String as PropType<Components.Schemas.ApplicationTypes>,
      required: true,
    },
  },
  setup: () => {
    return { CertificationType };
  },
  data() {
    return { items: [] };
  },
  computed: {
    isRenewal() {
      return this.applicationType === "Renewal";
    },
    isLaborMobility() {
      return this.applicationType === "LabourMobility";
    },
    items() {
      return this.isRenewal
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
    },
  },
});
</script>
