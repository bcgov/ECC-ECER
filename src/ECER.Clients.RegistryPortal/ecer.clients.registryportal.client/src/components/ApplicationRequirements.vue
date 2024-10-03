<template>
  <v-container>
    <Breadcrumb :items="items" />

    <!-- Renewal -->
    <template v-if="applicationStore.isDraftApplicationRenewal">
      <ECEAssistantRenewalRequirements v-if="applicationStore.isDraftCertificateTypeEceAssistant" />
      <ECEOneYearRenewalRequirements v-if="applicationStore.isDraftCertificateTypeOneYear" />
      <ECEFiveYearRenewalRequirements
        v-if="applicationStore.isDraftCertificateTypeFiveYears"
        :expired="certificationStore.latestCertificateStatus === 'Expired'"
        :expired-more-than5-years="certificationStore.latestExpiredMoreThan5Years"
      />
    </template>

    <!-- Registrant -->
    <template v-if="userStore.isRegistrant">
      <ECEAssistantRequirements v-if="applicationStore.isDraftCertificateTypeEceAssistant" />
      <ECEOneYearRequirements v-if="applicationStore.isDraftCertificateTypeOneYear" />
      <ECEFiveYearRegistrantRequirements
        v-if="applicationStore.isDraftCertificateTypeFiveYears || applicationStore.draftApplication.certificationTypes?.length === 0"
        ref="ECEFiveYearRegistrantRequirements"
      />
      <ECESneRegistrantRequirements v-else-if="applicationStore.isDraftCertificateTypeSne" />
      <ECEIteRegistrantRequirements v-else-if="applicationStore.isDraftCertificateTypeIte" />
    </template>

    <!-- New -->
    <template v-else>
      <ECEAssistantRequirements v-if="applicationStore.isDraftCertificateTypeEceAssistant" />
      <ECEOneYearRequirements v-if="applicationStore.isDraftCertificateTypeOneYear" />
      <ECEFiveYearRequirements v-if="applicationStore.isDraftCertificateTypeFiveYears" />
    </template>

    <v-btn class="mt-6" rounded="lg" color="primary" @click="continueClick">Apply Now</v-btn>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import type { VForm } from "vuetify/components";

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
import type { Components } from "@/types/openapi";

import ECEFiveYearRegistrantRequirements from "./ECEFiveYearRegistrantRequirements.vue";
import ECEIteRegistrantRequirements from "./ECEIteRegistrantRequirements.vue";
import ECESneRegistrantRequirements from "./ECESneRegistrantRequirements.vue";

export default defineComponent({
  name: "ApplicationRequirements",
  components: {
    ECEAssistantRequirements,
    ECEOneYearRequirements,
    ECEFiveYearRequirements,
    ECEAssistantRenewalRequirements,
    ECEOneYearRenewalRequirements,
    ECEFiveYearRenewalRequirements,
    ECEFiveYearRegistrantRequirements,
    ECEIteRegistrantRequirements,
    ECESneRegistrantRequirements,
    Breadcrumb,
  },

  provide() {
    return {
      handleSpecializationSelection: this.handleSpecializationSelection,
    };
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

    return {
      items,
      specializationSelection: [] as Components.Schemas.CertificationType[],
    };
  },
  methods: {
    handleSpecializationSelection(payload?: Components.Schemas.CertificationType[]) {
      this.specializationSelection = payload ?? [];
    },
    async continueClick() {
      if (this.applicationStore.draftApplication.certificationTypes?.length === 0) {
        // Validate specialization form as we are in the add specialization(s) registrant flow

        const formRef = (this.$refs.ECEFiveYearRegistrantRequirements as typeof ECEFiveYearRegistrantRequirements).$refs.SpecializedCertificationOptions.$refs
          .specializationForm as VForm;

        const { valid } = await formRef.validate();
        if (valid) {
          this.applicationStore.$patch({ draftApplication: { certificationTypes: this.specializationSelection } });
          this.$router.push({ name: "declaration" });
        }
      } else {
        const currentTypes = this.applicationStore.draftApplication.certificationTypes || [];
        const updatedTypes = [...currentTypes, ...this.specializationSelection];

        // Remove duplicates if necessary
        const uniqueUpdatedTypes = Array.from(new Set(updatedTypes));

        // Patch the store with the updated types
        this.applicationStore.$patch({ draftApplication: { certificationTypes: uniqueUpdatedTypes } });
        this.$router.push({ name: "declaration" });
      }
    },
  },
});
</script>
