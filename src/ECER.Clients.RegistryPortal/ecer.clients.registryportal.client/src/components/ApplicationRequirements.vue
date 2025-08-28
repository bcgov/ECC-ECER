<template>
  <v-container>
    <Breadcrumb />

    <!-- Renewal -->
    <template v-if="applicationStore.isDraftApplicationRenewal">
      <ECEAssistantRenewalRequirements v-if="applicationStore.isDraftCertificateTypeEceAssistant" />
      <ECEOneYearRenewalRequirements
        v-if="applicationStore.isDraftCertificateTypeOneYear"
        :expired="certificationStore.certificateStatus(applicationStore.draftApplication.fromCertificate) === 'Expired'"
      />
      <ECEFiveYearRenewalRequirements
        v-if="applicationStore.isDraftCertificateTypeFiveYears"
        :expired="certificationStore.certificateStatus(applicationStore.draftApplication.fromCertificate) === 'Expired'"
        :expired-more-than5-years="certificationStore.expiredMoreThan5Years(applicationStore.draftApplication.fromCertificate)"
      />
    </template>

    <!-- Labor Mobility -->
    <template v-else-if="applicationStore.isDraftApplicationLaborMobility">
      <ECEAssistantLaborMobilityRequirements v-if="applicationStore.isDraftCertificateTypeEceAssistant" />
      <ECEOneYearLaborMobilityRequirements v-if="applicationStore.isDraftCertificateTypeOneYear" />
      <ECEFiveYearLaborMobilityRequirements
        v-if="applicationStore.isDraftCertificateTypeFiveYears"
        ref="ECEFiveYearLaborMobilityRequirements"
        :is-post-basic="isPostBasic"
      />
    </template>

    <!-- Registrant -->
    <template v-else-if="userStore.isRegistrant">
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

    <v-btn class="mt-6" rounded="lg" color="primary" @click="continueClick" id="btnApplyNow">Apply now</v-btn>
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
import ECEAssistantLaborMobilityRequirements from "./ECEAssistantLaborMobilityRequirements.vue";
import ECEOneYearLaborMobilityRequirements from "./ECEOneYearLaborMobilityRequirements.vue";
import ECEFiveYearLaborMobilityRequirements from "./ECEFiveYearLaborMobilityRequirements.vue";
import ECEOneYearRequirements from "@/components/ECEOneYearRequirements.vue";
import { useApplicationStore } from "@/store/application";
import { useCertificationStore } from "@/store/certification";
import { useUserStore } from "@/store/user";
import type { Components } from "@/types/openapi";

import ECEFiveYearRegistrantRequirements from "./ECEFiveYearRegistrantRequirements.vue";
import ECEIteRegistrantRequirements from "./ECEIteRegistrantRequirements.vue";
import ECESneRegistrantRequirements from "./ECESneRegistrantRequirements.vue";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "ApplicationRequirements",
  components: {
    ECEAssistantRequirements,
    ECEOneYearRequirements,
    ECEFiveYearRequirements,
    ECEAssistantLaborMobilityRequirements,
    ECEOneYearLaborMobilityRequirements,
    ECEFiveYearLaborMobilityRequirements,
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
    const router = useRouter();

    return { applicationStore, certificationStore, userStore, router };
  },
  data() {
    return {
      specializationSelection: [] as Components.Schemas.CertificationType[],
    };
  },
  computed: {
    isPostBasic() {
      return this.applicationStore.draftApplication.certificationTypes?.some((type) => type === "Ite" || type === "Sne");
    },
  },
  methods: {
    handleSpecializationSelection(payload?: Components.Schemas.CertificationType[]) {
      this.specializationSelection = payload ?? [];
    },
    async continueClick() {
      if (this.applicationStore.draftApplication.certificationTypes?.length === 0) {
        // Validate specialization form

        const formRef = (this.$refs.ECEFiveYearRegistrantRequirements as typeof ECEFiveYearRegistrantRequirements).$refs.SpecializedCertificationOptions.$refs
          .specializationForm as VForm;

        const { valid } = await formRef.validate();
        if (valid) {
          this.applicationStore.$patch({
            draftApplication: { certificationTypes: this.specializationSelection },
          });
          this.router.push({ name: "declaration" });
        }
      } else if (this.applicationStore.isDraftApplicationLaborMobility) {
        // handle case where user chooses 5 year specializations
        if (
          this.applicationStore.isDraftCertificateTypeFiveYears &&
          this.applicationStore.isDraftCertificateTypeIte &&
          this.applicationStore.isDraftCertificateTypeSne
        ) {
          this.applicationStore.$patch({
            draftApplication: { certificationTypes: ["FiveYears", ...this.specializationSelection] },
          });
        }
        this.router.push({ name: "declaration" });
      } else if (this.applicationStore.isDraftApplicationRenewal) {
        //for renewal applications we do not need to perform any additional checks. The certification types should be correctly set in the draft application store.
        this.router.push({ name: "declaration" });
      } else {
        let currentTypes = this.applicationStore.draftApplication.certificationTypes || [];

        // Filter out "Ite" and "Sne" unless currentTypes *only* contains "Ite" or "Sne".
        // This corrects edge case where user clicks requirements and does not select a new path. We need to remove ITE + SNE or it will persist.
        currentTypes =
          currentTypes.length === 1 && (currentTypes.includes("Ite") || currentTypes.includes("Sne"))
            ? currentTypes
            : currentTypes.filter((certification) => certification !== "Ite" && certification !== "Sne");

        const updatedTypes = [...currentTypes, ...this.specializationSelection];

        // Remove duplicates if necessary
        const uniqueUpdatedTypes = Array.from(new Set(updatedTypes));

        // Patch the store with the updated types
        this.applicationStore.$patch({
          draftApplication: { certificationTypes: uniqueUpdatedTypes },
        });
        this.router.push({ name: "declaration" });
      }
    },
  },
});
</script>
