<template>
  <v-container>
    <Breadcrumb :items="items" />
    <template v-if="applicationStore.isDraftCertificateTypeEceAssistant">
      <ECEAssistantRequirements v-if="!applicationStore.isDraftApplicationRenewal" />
      <ECEAssistantRenewalRequirements v-else />
    </template>
    <template v-if="applicationStore.isDraftCertificateTypeOneYear">
      <ECEOneYearRequirements v-if="!applicationStore.isDraftApplicationRenewal" />
      <ECEOneYearRenewalRequirements v-else :expired="certificationStore.latestCertificateStatus === 'Expired'" />
    </template>
    <template v-if="applicationStore.isDraftCertificateTypeFiveYears">
      <ECEFiveYearRequirements v-if="!applicationStore.isDraftApplicationRenewal" />
      <ECEFiveYearRenewalRequirements
        v-else
        :expired="certificationStore.latestCertificateStatus === 'Expired'"
        :expired-more-than5-years="certificationStore.latestExpiredMoreThan5Years"
      />
    </template>
    <v-btn class="mt-6" rounded="lg" color="primary" @click="continueClick">Continue</v-btn>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEAssistantRenewalRequirements from "@/components/ECEAssistantRenewalRequirements.vue";
import ECEAssistantRequirements from "@/components/ECEAssistantRequirements.vue";
import ECEFiveYearRenewalRequirements from "@/components/ECEFiveYearRenewalRequirements.vue";
import ECEFiveYearRequirements from "@/components/ECEFiveYearRequirements.vue";
import ECEOneYearRenewalRequirements from "@/components/ECEOneYearRenewalRequirements.vue";
import ECEOneYearRequirements from "@/components/ECEOneYearRequirements.vue";
import { useApplicationStore } from "@/store/application";
import { useCertificationStore } from "@/store/certification";

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

  setup: () => {
    const applicationStore = useApplicationStore();
    const certificationStore = useCertificationStore();

    return { applicationStore, certificationStore };
  },
  data() {
    const applicationStore = useApplicationStore();

    const items = applicationStore.isDraftApplicationRenewal
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
  methods: {
    async continueClick() {
      this.$router.push({ name: "declaration" });
    },
  },
});
</script>
