<template>
  <v-card :rounded="!isRounded ? '0' : ''" flat color="background-light" class="pa-4">
    <v-card-item class="ma-4">
      <p class="font-weight-bold">Certification</p>
      <div class="d-flex flex-column mt-2">
        <p v-for="(title, index) in certificationStore.latestTitleArray" :key="index" class="extra-large">
          {{ title }}
        </p>
      </div>
      <p class="font-weight-bold mt-8">Expires on</p>
      <div class="d-flex flex-row align-center mt-2 ga-4">
        <p>{{ formattedExpiryDate }}</p>
        <v-chip :color="chipColor" variant="flat" size="small">{{ chipText }}</v-chip>
        <v-chip v-if="certificationStore.latestHasTermsAndConditions" color="grey-darkest" variant="outlined" size="small">Has Terms and Conditions</v-chip>
      </div>
    </v-card-item>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useCertificationStore } from "@/store/certification";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "CertificationCard",
  props: {
    isRounded: {
      type: Boolean,
      default: true,
    },
  },
  setup() {
    const certificationStore = useCertificationStore();

    return {
      certificationStore,
    };
  },
  computed: {
    formattedExpiryDate(): string {
      return formatDate(this.certificationStore.latestCertification?.expiryDate ?? "", "LLLL d, yyyy");
    },
    chipText() {
      // "Active" | "Cancelled" | "Expired" | "Inactive" | "Reprinted" | "Suspended"
      switch (this.certificationStore.latestCertification?.statusCode) {
        case "Active":
        case "Reprinted":
          return "Active";
        case "Expired":
        case "Inactive":
          return "Expired";
        case "Cancelled":
          return "Cancelled";
        case "Suspended":
          return "Suspended";
        default:
          return "Expired";
      }
    },
    chipColor(): string {
      // "success" | "error" | "warning" | "info"
      switch (this.chipText) {
        case "Active":
          return "success";
        case "Expired":
          return "error";
        case "Cancelled":
        case "Suspended":
          return "grey-darkest";
        default:
          return "grey-darkest";
      }
    },
  },
  methods: {},
});
</script>
