<template>
  <div>
    <h1 v-if="!isRenewal">{{ `Application for ECE ${certificationType} certification` }}</h1>
    <h1 v-else>{{ `ECE ${certificationType} renewal` }}</h1>
    <div v-if="certificationTypes.includes(CertificationType.FIVE_YEAR) && !isRenewal" role="doc-subtitle">
      {{ certificationTypeSubtitleForFiveYear }}
    </div>
  </div>
</template>

<script lang="ts">
import type { PropType } from "vue";
import { defineComponent } from "vue";

import type { Components } from "@/types/openapi";
import { CertificationType } from "@/utils/constant";

export default defineComponent({
  name: "ApplicationCertificationTypeHeader",
  components: {},
  props: {
    certificationTypes: {
      type: Array as PropType<Components.Schemas.CertificationType[]>,
      required: true,
      default: () => [],
    },
    isRenewal: {
      type: Boolean,
      default: false,
    },
  },
  setup: async () => {
    return { CertificationType };
  },
  computed: {
    certificationType() {
      let certificationType = "";
      if (this.certificationTypes?.includes(CertificationType.ECE_ASSISTANT)) {
        certificationType = "Assistant";
      } else if (this.certificationTypes?.includes(CertificationType.ONE_YEAR)) {
        certificationType = "One Year";
      } else if (this.certificationTypes?.includes(CertificationType.FIVE_YEAR)) {
        certificationType = "Five Year";
      }
      return certificationType;
    },
    certificationTypeSubtitleForFiveYear() {
      let certificationTypeSubtitle = "";
      if (this.certificationTypes?.includes(CertificationType.ITE) && this.certificationTypes?.includes(CertificationType.SNE)) {
        certificationTypeSubtitle = "Including certification for Special Needs Education (SNE) and Infant and Toddler Educator (ITE)";
      } else if (this.certificationTypes?.includes(CertificationType.SNE)) {
        certificationTypeSubtitle = "Including certification for Special Needs Educator (SNE)";
      } else if (this.certificationTypes?.includes(CertificationType.ITE)) {
        certificationTypeSubtitle = "Including certification for Infant and Toddler Eductor (ITE)";
      }

      return certificationTypeSubtitle;
    },
  },
  methods: {},
});
</script>
