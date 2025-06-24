<template>
  <Card :hasTopBorder="true" topBorderSize="large" class="px-8 pb-9 pt-4">
    <v-card-item>
      <v-row>
        <v-col :cols="mdAndUp && isCertificateActive ? 8 : 12">
          <div class="d-flex flex-column ga-5">
            <h1><CertificationTitle :certification="certification" /></h1>
            <div>
              <CertificationChip :certification="certification" />
            </div>
            <div v-if="certification.hasConditions">
              <v-btn
                prepend-icon="mdi-newspaper-variant-outline"
                base-color="alert-warning"
                class="border-sm border-warning-border border-opacity-100"
                @click="handleViewTermsAndConditions"
              >
                View terms and conditions
              </v-btn>
            </div>
            <p>{{ subText }}</p>
            <div>
              <CertificationDates :certification="certification" />
            </div>

            <!-- Certificate Inline on mobile -->
            <template v-if="!mdAndUp">
              <a v-if="isCertificateActive && doesCertificateFileExist" :href="pdfUrl" target="_blank">{{ generateFileDisplayName() }}</a>
              <span v-if="certificateGenerationRequested" class="d-flex align-center ga-4">
                <v-progress-circular class="mb-2" color="primary" indeterminate></v-progress-circular>
                <h4>Your certificate is being generated. This may take up to 10 minutes. Please check back later to download it.</h4>
              </span>
            </template>

            <RenewAction v-if="!hasApplication" :certification="certification" />
          </div>
        </v-col>
        <v-col v-if="mdAndUp" cols="4" class="text-center d-flex justify-end align-center" style="min-width: 215px">
          <div v-if="isCertificateActive && doesCertificateFileExist" class="d-flex flex-column align-center justify-center">
            <img src="../assets/certificate.svg" width="215" class="logo" alt="Certificate" />
            <a v-if="isCertificateActive && doesCertificateFileExist" :href="pdfUrl" target="_blank">{{ generateFileDisplayName() }}</a>
          </div>
          <div v-if="certificateGenerationRequested" class="mt-8">
            <v-progress-circular class="mb-2" color="primary" indeterminate></v-progress-circular>
            <h4>Your certificate is being generated. This may take up to 10 minutes. Please check back later to download it.</h4>
          </div>
        </v-col>
      </v-row>
    </v-card-item>
  </Card>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import { getCertificateFileById, requestCertificateFileGeneration } from "@/api/certification";
import { useCertificationStore } from "@/store/certification";
import { humanFileSize } from "@/utils/functions";
import type { Components } from "@/types/openapi";
import RenewAction from "@/components/RenewAction.vue";
import CertificationChip from "@/components/CertificationChip.vue";
import CertificationDates from "@/components/CertificationDates.vue";
import CertificationTitle from "@/components/CertificationTitle.vue";
import { useDisplay } from "vuetify";
import Card from "./Card.vue";

export default defineComponent({
  name: "CertificationCard",
  components: {
    RenewAction,
    CertificationChip,
    CertificationDates,
    CertificationTitle,
    Card,
  },
  props: {
    certification: {
      type: Object as PropType<Components.Schemas.Certification>,
      required: true,
    },
    hasApplication: {
      type: Boolean,
      default: false,
    },
  },
  setup() {
    const certificationStore = useCertificationStore();
    const { mdAndUp } = useDisplay();

    return {
      certificationStore,
      mdAndUp,
    };
  },
  data() {
    return {
      pdfUrl: "",
      fileSize: "",
    };
  },
  computed: {
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
    certificateGenerationRequested(): boolean {
      return this.certification.certificatePDFGeneration === "Requested";
    },
    doesCertificateFileExist(): boolean {
      return this.certification.certificatePDFGeneration === "Yes";
    },
    isCertificateActive(): boolean {
      return this.certification.statusCode === "Active" || this.certification.statusCode === "Renewed";
    },
  },
  async mounted() {
    if (import.meta.env?.STORYBOOK) {
      console.warn("Skipping API requests in Storybook");
      return;
    }

    if (this.isCertificateActive) {
      if (this.certification.certificatePDFGeneration === "No") {
        const response = await requestCertificateFileGeneration(this.certification.id ?? "");
        if (response) {
          this.certification.certificatePDFGeneration = "Requested";
        }
      } else if (this.certification.certificatePDFGeneration === "Yes") {
        const file = await getCertificateFileById(this.certification.id ?? "");
        this.pdfUrl = window.URL.createObjectURL(file.data);
        this.fileSize = humanFileSize(file.data.size);
      }
    }
  },
  unmounted() {
    window.URL.revokeObjectURL(this.pdfUrl);
  },
  methods: {
    generateFileDisplayName() {
      return `Download certificate (PDF, ${this.fileSize})`;
    },
    handleViewTermsAndConditions() {
      this.$router.push(`/certificate-terms-and-conditions/${this.certification.id}`);
    },
  },
});
</script>
