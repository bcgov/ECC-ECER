<template>
  <div>
    <p class="d-flex flex-column pb-4">
      {{ text }}
      <p v-if="canRenew && !readyToRenew">{{ dateText }}</p>
    </p>
    <div>
      <v-btn id="btnRenew" v-if="showRenewLink" color="primary" @click="handleRenewClicked">Renew</v-btn>
      <div v-else-if="showRenewalRequirementsLink" @click="handleLearnAboutRenewalRequirementsClicked">
        <a href="#" @click.prevent>Learn about renewal requirements</a>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { defineComponent, type PropType } from "vue";

import ActionCard from "@/components/ActionCard.vue";
import { useApplicationStore } from "@/store/application";
import { useCertificationStore } from "@/store/certification";
import { useRouter } from "vue-router";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "RenewAction",
  components: {
    ActionCard,
  },
  props: {
    certification: {
      type: Object as PropType<Components.Schemas.Certification>,
      required: true,
    },
  },
  setup() {
    const certificationStore = useCertificationStore();
    const applicationStore = useApplicationStore();
    const router = useRouter();

    return {
      certificationStore,
      applicationStore,
      router,
    };
  },
  computed: {
    earliestRenewalDate() {
      return DateTime.fromISO(this.certification.expiryDate ?? "").minus({ months: 6 });
    },
    latestRenewalDate() {
      return DateTime.fromISO(this.certification.expiryDate ?? "").plus({ years: 5 });
    },
    todaysDate() {
      return DateTime.now();
    },
    readyToRenew() {
      return this.todaysDate >= this.earliestRenewalDate;
    },
    expiredOverFiveYears() {
      return this.todaysDate >= this.latestRenewalDate;
    },
    canRenew() {
      return (
        !(this.certificationStore.hasMultipleEceOneYearCertifications && this.certificationStore.isEceOneYear(this.certification.id)) &&
        !(this.certificationStore.isEceOneYear(this.certification.id) && this.expiredOverFiveYears) &&
        !(this.certificationStore.isEceAssistant(this.certification.id) && this.expiredOverFiveYears) 
      );
    },
    title() {
      return this.canRenew ? "Renew" : "Renew unavailable";
    },
    text() {
      // Not yet ready to renew
      if (this.canRenew && !this.readyToRenew) {
        return "You can renew starting 6 months before your certificate expires.";
      }

      // Assistant
      if (this.certificationStore.isEceAssistant(this.certification.id)) {
        if (this.expiredOverFiveYears) {
          return "You cannot renew your ECE Assistant certification because it's been expired for over 5 years.";
        }
        return "You can renew your ECE Assistant certification.";
      }

      // One Year
      if (this.certificationStore.isEceOneYear(this.certification.id)) {
        if (this.certificationStore.hasMultipleEceOneYearCertifications) {
          return "You cannot renew your ECE One Year certification again. It can only be renewed once.";
        } else if (this.expiredOverFiveYears) {
          return "You cannot renew your ECE One Year certification because it's been expired for over 5 years.";
        }
        return "You can renew your ECE One Year certification.";
      }

      // Five Year
      if (this.certificationStore.isEceFiveYear(this.certification.id)) {
        if (this.certificationStore.hasITE(this.certification.id) && this.certificationStore.hasSNE(this.certification.id)) {
          return "You can renew your ECE Five Year, SNE and ITE certification.";
        } else if (this.certificationStore.hasITE(this.certification.id)) {
          return "You can renew your ECE Five Year and ITE certification.";
        } else if (this.certificationStore.hasSNE(this.certification.id)) {
          return "You can renew your ECE Five Year and SNE certification.";
        } else {
          return "You can renew your ECE Five Year certification.";
        }
      }
      return "";
    },
    dateText() {
      return `You can renew after: ${this.earliestRenewalDate.toFormat("LLLL d, yyyy")}`;
    },
    showRenewLink(): boolean {
      return this.readyToRenew && this.canRenew;
    },
    showRenewalRequirementsLink(): boolean {
      return !this.readyToRenew && this.canRenew;
    },
  },
  methods: {
    handleLearnAboutRenewalRequirementsClicked() {
      this.router.push({
        name: "certification-requirements",
        query: { certificationTypes: this.certificationStore.certificationTypes(this.certification.id)},
      });
    },
    handleRenewClicked() {
      this.applicationStore.$patch({
        draftApplication: { applicationType: "Renewal", certificationTypes: this.certificationStore.certificationTypes(this.certification.id), fromCertificate: this.certification.id },
      });

      this.router.push({ name: "application-requirements" });
    },
  },
});
</script>
