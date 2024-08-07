<template>
  <v-card :rounded="!isRounded ? '0' : ''" flat color="background-light" class="pa-4">
    <v-card-item class="ma-4">
      <p class="font-weight-bold">Certification</p>
      <div class="d-flex flex-column mt-2">
        <p v-for="(title, index) in certificationStore.latestTitleArray" :key="index" class="extra-large">
          {{ title }}
        </p>
      </div>
      <a v-if="isLatestCertificateActive" :href="pdfUrl" target="_blank">{{ generateFileDisplayName() }}</a>

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

import { getCertificateFileById } from "@/api/certification";
import { useCertificationStore } from "@/store/certification";
import { formatDate } from "@/utils/format";
import { humanFileSize } from "@/utils/functions";

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
  data() {
    return {
      pdfUrl: "",
      fileSize: "",
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
    isLatestCertificateActive(): boolean {
      return this.certificationStore.latestCertification?.statusCode === "Active";
    },
  },
  async mounted() {
    if (this.certificationStore.certifications && this.certificationStore.certifications.length > 0 && this.isLatestCertificateActive) {
      const file = await getCertificateFileById(this.certificationStore.certifications[0].id ?? "");

      this.pdfUrl = window.URL.createObjectURL(file.data);
      this.fileSize = humanFileSize(file.data.size);
    }
  },
  unmounted() {
    window.URL.revokeObjectURL(this.pdfUrl);
  },
  methods: {
    generateFileDisplayName() {
      return `Download my certificate (PDF, ${this.fileSize})`;
    },
  },
});
</script>
