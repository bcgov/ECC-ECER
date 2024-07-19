<template>
  <v-card :rounded="!isRounded ? '0' : ''" flat color="background-light" class="pa-4">
    <v-card-item class="ma-4">
      <p class="font-weight-bold">Certification</p>
      <div class="d-flex flex-column mt-2">
        <p v-for="(title, index) in certificationStore.latestTitleArray" :key="index" class="extra-large">
          {{ title }}
        </p>
      </div>
      <a :href="pdfUrl" target="_blank">{{ generateFileDisplayName() }}</a>

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
  async mounted() {
    if (this.certificationStore.certifications && this.certificationStore.certifications.length > 0) {
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
      if (this.certificationStore?.certifications?.[0].files?.[0]) {
        const file = this.certificationStore?.certifications?.[0].files?.[0];
        return `${file.name} (${this.fileSize})`;
      }
    },
  },
});
</script>
