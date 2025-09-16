<template>
  <v-row>
    <v-col cols="12">
      <div class="d-flex">
        <p class="align-self-center mr-4"><strong>SHOW:</strong></p>
        <v-sheet class="">
          <v-btn-toggle v-model="filter" color="primary" mandatory>
            <v-btn value="latest">Latest</v-btn>
            <v-btn value="all">All</v-btn>
          </v-btn-toggle>
        </v-sheet>
      </div>
    </v-col>
    <v-col cols="12">
      <p v-if="filter === 'latest'">Showing only your most recent certificate of each type</p>
      <p v-else>Showing your entire certification history</p>
    </v-col>
    <v-col cols="12" v-for="(cert, index) in displayCertifications" :key="index">
      <CertificationCard :certification="cert" :has-application="applicationStore.hasApplication" :is-latest-of-type="latestCertifications.includes(cert)" />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import { orderBy } from "lodash";
import type { Components } from "@/types/openapi";
import CertificationCard from "@/components/CertificationCard.vue";
import { useApplicationStore } from "@/store/application";

export default defineComponent({
  name: "CertificationList",
  components: {
    CertificationCard,
  },
  props: {
    certifications: {
      type: Array as PropType<Components.Schemas.Certification[]>,
      required: true,
    },
  },
  computed: {
    // Helper function to get the highest priority certification type
    getCertificationTypePriority() {
      return (cert: Components.Schemas.Certification) => {
        if (cert.levels?.some((level) => level.type === "ECE 5 YR")) {
          return 1; // Highest priority
        } else if (cert.levels?.some((level) => level.type === "ECE 1 YR")) {
          return 2; // Medium priority
        } else if (cert.levels?.some((level) => level.type === "Assistant")) {
          return 3; // Lowest priority
        }
        return 4; // Other types
      };
    },

    // Helper function to get status priority
    getStatusPriority() {
      return (statusCode: string) => {
        switch (statusCode) {
          case "Active":
            return 1;
          case "Cancelled":
            return 2;
          case "Suspended":
            return 3;
          case "Expired":
            return 4;
          default:
            return 5;
        }
      };
    },

    // Sort certifications by status, expiry date, and certification type priority
    sortedCertifications() {
      return orderBy(
        this.certifications,
        [({ statusCode }: Components.Schemas.Certification) => this.getStatusPriority(statusCode || ""), "expiryDate", this.getCertificationTypePriority],
        ["asc", "desc", "asc"], // asc for status (Active first), desc for expiry date (latest first), asc for type priority (ECE 5 YR = 1, Assistant = 3)
      );
    },

    latestCertifications() {
      // Get the best certification from each type from the already sorted list
      const seenTypes = new Set<string>();
      const result: Components.Schemas.Certification[] = [];

      for (const cert of this.sortedCertifications) {
        const certType = cert.levels?.find((level) => level.type === "ECE 5 YR" || level.type === "ECE 1 YR" || level.type === "Assistant")?.type;

        if (certType && !seenTypes.has(certType)) {
          seenTypes.add(certType);
          result.push(cert);
        }
      }

      return result;
    },

    displayCertifications() {
      return this.filter === "latest" ? this.latestCertifications : this.sortedCertifications;
    },
  },
  data() {
    return {
      filter: "latest",
    };
  },
  setup() {
    const applicationStore = useApplicationStore();
    return { applicationStore };
  },
});
</script>

<style scoped>
.v-btn-toggle > .v-btn {
  border-radius: 0 !important;
}
</style>
