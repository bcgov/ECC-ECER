<template>
  <v-container>
    <Breadcrumb :items="items" />
    <template v-if="applicationStore.isDraftCertificateTypeEceAssistant">
      <ECEAssistantRenewalRequirements v-if="applicationStore.isDraftApplicationRenewal" />
      <ECEAssistantRegistrantRequirements v-else-if="userStore.isRegistrant" />
      <ECEAssistantRequirements v-else />
    </template>
    <template v-if="applicationStore.isDraftCertificateTypeOneYear">
      <ECEOneYearRenewalRequirements v-if="applicationStore.isDraftApplicationRenewal" :expired="certificationStore.latestCertificateStatus === 'Expired'" />
      <ECEOneYearRegistrantRequirements v-else-if="userStore.isRegistrant" />
      <ECEOneYearRequirements v-else />
    </template>
    <template v-if="applicationStore.isDraftCertificateTypeFiveYears">
      <ECEFiveYearRenewalRequirements
        v-if="applicationStore.isDraftApplicationRenewal"
        :expired="certificationStore.latestCertificateStatus === 'Expired'"
        :expired-more-than5-years="certificationStore.latestExpiredMoreThan5Years"
      />
      <ECEFiveYearRegistrantRequirements v-else-if="userStore.isRegistrant" />
      <ECEFiveYearRequirements v-else />
    </template>
    <template v-else-if="applicationStore.isDraftCertificateTypeSne && !applicationStore.isDraftApplicationRenewal && userStore.isRegistrant">
      <ECESneRegistrantRequirements />
    </template>
    <template v-else-if="applicationStore.isDraftCertificateTypeIte && !applicationStore.isDraftApplicationRenewal && userStore.isRegistrant">
      <ECEIteRegistrantRequirements />
    </template>

    <v-btn class="mt-6" rounded="lg" color="primary" @click="continueClick">Apply Now</v-btn>
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
import { useUserStore } from "@/store/user";

import ECEAssistantRegistrantRequirements from "./ECEAssistantRegistrantRequirements.vue";
import ECEFiveYearRegistrantRequirements from "./ECEFiveYearRegistrantRequirements.vue";
import ECEIteRegistrantRequirements from "./ECEIteRegistrantRequirements.vue";
import ECEOneYearRegistrantRequirements from "./ECEOneYearRegistrantRequirements.vue";
import ECESneRegistrantRequirements from "./ECESneRegistrantRequirements.vue";

export default defineComponent({
  name: "CertificationTypeRequirements",
  components: {
    ECEAssistantRequirements,
    ECEOneYearRequirements,
    ECEFiveYearRequirements,
    ECEAssistantRenewalRequirements,
    ECEOneYearRenewalRequirements,
    ECEFiveYearRenewalRequirements,
    ECEAssistantRegistrantRequirements,
    ECEOneYearRegistrantRequirements,
    ECEFiveYearRegistrantRequirements,
    ECEIteRegistrantRequirements,
    ECESneRegistrantRequirements,
    Breadcrumb,
  },

  setup: () => {
    const applicationStore = useApplicationStore();
    const certificationStore = useCertificationStore();
    const userStore = useUserStore();

    return { applicationStore, certificationStore, userStore };
  },
  data() {
    const applicationStore = useApplicationStore();
    const userStore = useUserStore();

    const items =
      applicationStore.isDraftApplicationRenewal || userStore.isRegistrant
        ? [
            {
              title: "Home",
              disabled: false,
              href: "/",
            },
            {
              title: "Requirements",
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
