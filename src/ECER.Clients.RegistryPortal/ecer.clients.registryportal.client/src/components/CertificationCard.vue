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
        <v-chip color="success" variant="flat" size="small">Active</v-chip>
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
    chipText(): string {
      // "Active" | "Cancelled" | "Expired" | "Inactive" | "Reprinted" | "Suspended"
      switch (this.certificationStore.latestCertification?.statusCode) {
        case "Active":
          return "Active";
        case "Expired":
          return "Expired";
        case "Cancelled":
          return "Cancelled";
        case "Inactive":
          return "Inactive";
        case "Reprinted":
          return "Reprinted";
        case "Suspended":
          return "Suspended";
        default:
          return "Inactive";
      }
    },
  },
  methods: {},
});
</script>
