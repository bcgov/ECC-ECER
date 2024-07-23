<template>
  <ActionCard :title="title" icon="mdi-autorenew" :links="links">
    <div class="d-flex flex-column ga-3">
      {{ text }}
      <div v-if="canRenew && !readyToRenew">{{ dateText }}</div>
    </div>
  </ActionCard>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { defineComponent } from "vue";

import ActionCard from "@/components/ActionCard.vue";
import { useCertificationStore } from "@/store/certification";

import type { Link } from "./ActionCard.vue";

export default defineComponent({
  name: "RenewCard",
  components: {
    ActionCard,
  },
  setup() {
    const certificationStore = useCertificationStore();

    return {
      certificationStore,
    };
  },
  computed: {
    earliestRenewalDate() {
      return DateTime.fromISO(this.certificationStore.latestCertification?.expiryDate ?? "").minus({ months: 6 });
    },
    latestRenewalDate() {
      return DateTime.fromISO(this.certificationStore.latestCertification?.expiryDate ?? "").plus({ years: 5 });
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
      return !this.certificationStore.hasMultipleEceOneYearCertifications && !this.expiredOverFiveYears;
    },
    title() {
      return this.canRenew ? "Renew" : "Renew unavailable";
    },
    text() {
      // Not yet ready to renew
      if (this.canRenew && !this.readyToRenew) {
        return "You can renew starting 6 months before your certificate expires.";
      }

      // Too late to renew
      if (this.expiredOverFiveYears) {
        return "You cannot renew your ECE One Year certification because it's been expired for over 5 years.";
      }

      // Assistant
      if (this.certificationStore.latestIsEceAssistant) {
        if (this.expiredOverFiveYears) {
          return "You cannot renew your ECE Assistant certification because it's been expired for over 5 years.";
        }
        return "You can renew your ECE Assistant certification.";
      }

      // One Year
      if (this.certificationStore.latestIsEceOneYear) {
        if (this.certificationStore.hasMultipleEceOneYearCertifications) {
          return "You cannot renew your ECE One Year certification again. It can only be renewed once.";
        } else if (this.expiredOverFiveYears) {
          return "You cannot renew your ECE One Year certification because it's been expired for over 5 years.";
        }
        return "You can renew your ECE One Year certification.";
      }

      // Five Year
      if (this.certificationStore.latestIsEceFiveYear) {
        if (this.expiredOverFiveYears) {
          return "You cannot renew your ECE Five Year certification because it's been expired for over 5 years.";
        }
        if (this.certificationStore.latestHasITE && this.certificationStore.latestHasSNE) {
          return "You can renew your ECE Five Year, SNE and ITE certification.";
        } else if (this.certificationStore.latestHasITE) {
          return "You can renew your ECE Five Year and ITE certification.";
        } else if (this.certificationStore.latestHasSNE) {
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
    links(): Link[] {
      const links: Link[] = [];
      if (this.showRenewLink) {
        links.push({
          text: "Renew",
          to: "/application",
        });
      }
      if (this.showRenewalRequirementsLink) {
        links.push({
          text: "Learn about renewal requirements",
          to: "/renewal-requirements",
        });
      }
      return links;
    },
  },
  methods: {},
});
</script>
