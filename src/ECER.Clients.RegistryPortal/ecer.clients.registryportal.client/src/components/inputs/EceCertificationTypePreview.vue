<template>
  <PreviewCard :title="generateTitle" portal-stage="CertificationType" :editable="false">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Certification type</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ certificationType }}</p>
        </v-col>
      </v-row>
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useApplicationStore } from "@/store/application";
export default defineComponent({
  name: "EceCertificationTypePreview",
  components: {
    PreviewCard,
  },
  setup: () => {
    const applicationStore = useApplicationStore();
    return {
      applicationStore,
    };
  },
  computed: {
    certificationType() {
      let certificationType = "";
      if (this.applicationStore.isDraftCertificateTypeEceAssistant) {
        certificationType = "ECE Assistant";
      } else if (this.applicationStore.isDraftCertificateTypeOneYear) {
        certificationType = "ECE One Year";
      } else if (this.applicationStore.isDraftCertificateTypeFiveYears) {
        certificationType = "ECE Five Year";

        if (this.applicationStore.isDraftCertificateTypeSne) {
          certificationType += " and Special Needs Educator (SNE)";
        }
        if (this.applicationStore.isDraftCertificateTypeIte) {
          certificationType += " and Infant and Toddler Educator (ITE)";
        }
      } else if (
        !this.applicationStore.isDraftCertificateTypeFiveYears &&
        this.applicationStore.isDraftCertificateTypeSne &&
        this.applicationStore.isDraftCertificateTypeIte
      ) {
        certificationType = "Special Needs Educator and Infant and Toddler Educator";
      } else if (!this.applicationStore.isDraftCertificateTypeFiveYears && this.applicationStore.isDraftCertificateTypeSne) {
        certificationType = "Special Needs Educator";
      } else if (!this.applicationStore.isDraftCertificateTypeFiveYears && this.applicationStore.isDraftCertificateTypeIte) {
        certificationType = "Infant and Toddler Educator";
      }
      return certificationType;
    },
    generateTitle() {
      if (this.applicationStore.isDraftApplicationRenewal) {
        return "Certification renewal";
      } else {
        return "Application type";
      }
    },
  },
});
</script>
