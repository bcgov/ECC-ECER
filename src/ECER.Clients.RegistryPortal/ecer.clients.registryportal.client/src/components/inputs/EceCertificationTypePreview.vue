<template>
  <PreviewCard title="Certification Selection" portal-stage="CertificationType" :editable="false">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Certification Type</p>
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
import type { EcePreviewProps } from "@/types/input";
import { CertificationType } from "@/utils/constant";
export default defineComponent({
  name: "EceCertificationTypePreview",
  components: {
    PreviewCard,
  },
  props: {
    props: {
      type: Object as () => EcePreviewProps,
      required: true,
    },
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
      if (this.applicationStore.draftApplicationIncludesCertification(CertificationType.ECE_ASSISTANT)) {
        certificationType = "ECE Assistant";
      } else if (this.applicationStore.draftApplicationIncludesCertification(CertificationType.ONE_YEAR)) {
        certificationType = "One Year";
      } else if (this.applicationStore.draftApplicationIncludesCertification(CertificationType.FIVE_YEAR)) {
        certificationType = "Five Year";

        if (this.applicationStore.draftApplicationIncludesCertification(CertificationType.SNE)) {
          certificationType += " and Special Needs Educator (SNE)";
        }
        if (this.applicationStore.draftApplicationIncludesCertification(CertificationType.ITE)) {
          certificationType += " and Infant and Toddler Educator (ITE)";
        }
      }
      return certificationType;
    },
  },
});
</script>
