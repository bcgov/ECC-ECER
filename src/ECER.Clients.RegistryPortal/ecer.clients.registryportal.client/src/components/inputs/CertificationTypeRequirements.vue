<template>
  <v-container>
    <Breadcrumb :items="items" />

    <div v-for="certificationType in certificationTypes" :key="certificationType">
      <template v-if="certificationType === CertificationType.ECE_ASSISTANT">
        <ECEAssistantRequirements />
      </template>
      <template v-if="certificationType === CertificationType.ONE_YEAR">
        <ECEOneYearRequirements />
      </template>
      <template v-if="certificationType === CertificationType.FIVE_YEAR">
        <ECEFiveYearRequirements />
      </template>
    </div>
    <v-btn class="mt-6" rounded="lg" color="primary" @click="continueClick">Continue</v-btn>
  </v-container>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEAssistantRequirements from "@/components/ECEAssistantRequirements.vue";
import ECEFiveYearRequirements from "@/components/ECEFiveYearRequirements.vue";
import ECEOneYearRequirements from "@/components/ECEOneYearRequirements.vue";
import type { Components } from "@/types/openapi";
import { CertificationType } from "@/utils/constant";

export default defineComponent({
  name: "CertificationTypeRequirements",
  components: {
    ECEAssistantRequirements,
    ECEOneYearRequirements,
    ECEFiveYearRequirements,
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
  methods: {
    async continueClick() {
      this.$router.push({ name: "declaration" });
    },
  },
});
</script>
