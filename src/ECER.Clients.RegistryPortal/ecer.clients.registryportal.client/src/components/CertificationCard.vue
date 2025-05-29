<template>
  <v-card :rounded="true" :border="true" flat class="px-8 pb-9 pt-4 custom-border">
    <v-card-item>
      <div class="d-flex flex-column ga-5">
        <h1>Early Childhood Educator - {{ titleArray.join(" ") }}</h1>
        <div>
          <v-chip :color="chipColor" variant="flat" size="small">{{ chipText }}</v-chip>
        </div>
        <div v-if="certification.hasConditions">
          <v-btn prepend-icon="mdi-newspaper-variant-outline" base-color="alert-warning" class="border-sm border-warning-border">
            View terms and conditions
          </v-btn>
        </div>
        <p>{{ subText }}</p>
        <div>
          <p class="font-weight-bold">Effective date: {{ formattedEffectiveDate }}</p>
          <p class="font-weight-bold">Expiry date: {{ formattedExpiryDate }}</p>
        </div>
        <RenewAction :certification="certification" />
        <!-- <a v-if="isLatestCertificateActive && doesCertificateFileExist" :href="pdfUrl" target="_blank">{{ generateFileDisplayName() }}</a> -->
        <!-- <div v-if="certificateGenerationRequested" class="mt-8">
          <v-progress-circular class="mb-2" color="primary" indeterminate></v-progress-circular>
          <p>Your certificate is being generated. This may take up to 10 minutes. Please check back later to download it.</p>
        </div> -->
      </div>
    </v-card-item>
  </v-card>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import { getCertificateFileById, requestCertificateFileGeneration } from "@/api/certification";
import { useCertificationStore } from "@/store/certification";
import { formatDate } from "@/utils/format";
import { humanFileSize } from "@/utils/functions";
import type { Components } from "@/types/openapi";
import RenewAction from "@/components/RenewAction.vue";

export default defineComponent({
  name: "CertificationCard",
  components: {
    RenewAction,
  },
  props: {
    certification: {
      type: Object as PropType<Components.Schemas.Certification>,
      required: true,
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
      return formatDate(this.certification.expiryDate ?? "", "LLLL d, yyyy");
    },
    formattedEffectiveDate(): string {
      return formatDate(this.certification.effectiveDate ?? "", "LLLL d, yyyy");
    },
    chipText() {
      let text;
      switch (this.certification.statusCode) {
        case "Active":
        case "Renewed":
        case "Reprinted":
          text = "Active";
          break;
        case "Expired":
        case "Inactive":
          text = "Expired";
          break;
        case "Cancelled":
          text = "Cancelled";
          break;
        case "Suspended":
          text = "Suspended";
          break;
        default:
          text = "Expired";
      }

      if (this.certification.hasConditions) {
        text += " with terms and conditions";
      }

      return text;
    },
    chipColor(): string {
      // "success" | "error" | "warning" | "info"
      switch (this.chipText) {
        case "Active":
        case "Active with terms and conditions":
          return "success";
        case "Expired":
        case "Expired with terms and conditions":
          return "error";
        case "Cancelled":
        case "Cancelled with terms and conditions":
        case "Suspended":
        case "Suspended with terms and conditions":
          return "grey-darkest";
        default:
          return "grey-darkest";
      }
    },
    titleArray() {
      if (!this.certification.levels) return [];
      return this.certification.levels
        ?.map((level: Components.Schemas.CertificationLevel) => {
          switch (level.type) {
            case "ITE":
              return "+ Infant and Toddler Educator (ITE)";
            case "SNE":
              return "+ Special Needs Educator (SNE)";
            case "ECE 1 YR":
              return "ECE One Year";
            case "ECE 5 YR":
              return "ECE Five Year";
            case "Assistant":
              return "ECE Assistant";
            default:
              return "";
          }
        })
        .sort((a: string, b: string) => {
          // Move strings starting with '+' to the end of the array
          if (a.startsWith("+") && !b.startsWith("+")) {
            return 1;
          } else if (!a.startsWith("+") && b.startsWith("+")) {
            return -1;
          } else {
            return 0;
          }
        });
    },
    subText() {
      if (!this.certification.levels) return "";
      const level = this.certification.levels.find((level) => level.type === "ECE 1 YR" || level.type === "ECE 5 YR" || level.type === "Assistant");
      if (!level) return "";

      switch (level.type) {
        case "ECE 1 YR":
          return "This certification allows you to work and complete work experience requirements for ECE Five Year certification. It is valid for 1 year.";
        case "ECE 5 YR":
          return "This is the highest level of certification in BC and allows you to work alone and/or as the primary educator. It is valid for 5 years.";
        case "Assistant":
          return "This certificate allows you to work alongside an ECE if you do not have the requirements (e.g., full educational program, work experience, professional development, etc.) for higher certification levels. It is valid for 5 years.";
        default:
          return "";
      }
    },
    hasTermsAndConditions(): boolean {
      return this.certification.hasConditions ?? false;
    },
    certificateGenerationRequested(): boolean {
      return this.certification.certificatePDFGeneration === "Requested";
    },
    doesCertificateFileExist(): boolean {
      return this.certification.certificatePDFGeneration === "Yes";
    },
    isLatestCertificateActive(): boolean {
      return this.certification.statusCode === "Active" || this.certification.statusCode === "Renewed";
    },
  },
  // async mounted() {
  //   if (this.certificationStore.certifications && this.certificationStore.certifications.length > 0 && this.isLatestCertificateActive) {
  //     if (this.certification.certificatePDFGeneration === "No") {
  //       const response = await requestCertificateFileGeneration(this.certificationStore.certifications[0].id ?? "");
  //       if (response) {
  //         this.certificationStore.latestCertification.certificatePDFGeneration = "Requested";
  //       }
  //     } else if (this.certification.certificatePDFGeneration === "Yes") {
  //       const file = await getCertificateFileById(this.certificationStore.certifications[0].id ?? "");
  //       this.pdfUrl = window.URL.createObjectURL(file.data);
  //       this.fileSize = humanFileSize(file.data.size);
  //     }
  //   }
  // },
  // unmounted() {
  //   window.URL.revokeObjectURL(this.pdfUrl);
  // },
  methods: {
    generateFileDisplayName() {
      return `Download my certificate (PDF, ${this.fileSize})`;
    },
  },
});
</script>

<style lang="css" scoped>
.custom-border {
  border-radius: 5px;
  border-top: 16px solid rgba(var(--v-theme-primary, #013366));
  border-right: 1px solid rgba(var(--v-theme-primary, #013366));
  border-bottom: 1px solid rgba(var(--v-theme-primary, #013366));
  border-left: 1px solid rgba(var(--v-theme-primary, #013366));
}
</style>
