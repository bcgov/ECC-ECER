<template>
  <div>
    <h1 class="white" v-if="isLaborMobility">{{ `Transfer certification to ${certificationType}` }}</h1>
    <h1 class="white" v-else-if="isRenewal">{{ `Application to renew ${certificationType} certification` }}</h1>
    <h1 class="white" v-else>{{ `Application for ${certificationType} certification` }}</h1>
    <div class="white" v-if="certificationTypes.includes(CertificationType.FIVE_YEAR)" role="doc-subtitle">
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
    isLaborMobility: {
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
        certificationType = "ECE Assistant";
      } else if (this.certificationTypes?.includes(CertificationType.ONE_YEAR)) {
        certificationType = "ECE One Year";
      } else if (this.certificationTypes?.includes(CertificationType.FIVE_YEAR)) {
        certificationType = "ECE Five Year";
      } else if (this.certificationTypes?.includes(CertificationType.SNE) && this.certificationTypes?.includes(CertificationType.ITE)) {
        certificationType = "ITE and SNE";
      } else if (this.certificationTypes?.includes(CertificationType.SNE)) {
        certificationType = "Special Needs Educator";
      } else if (this.certificationTypes?.includes(CertificationType.ITE)) {
        certificationType = "Infant and Toddler Educator";
      }
      return certificationType;
    },
    certificationTypeSubtitleForFiveYear() {
      let certificationTypeSubtitle = "";
      if (this.certificationTypes?.includes(CertificationType.ITE) && this.certificationTypes?.includes(CertificationType.SNE)) {
        certificationTypeSubtitle = "Including certification for Special Needs Educator (SNE) and Infant and Toddler Educator (ITE)";
      } else if (this.certificationTypes?.includes(CertificationType.SNE)) {
        certificationTypeSubtitle = "Including certification for Special Needs Educator (SNE)";
      } else if (this.certificationTypes?.includes(CertificationType.ITE)) {
        certificationTypeSubtitle = "Including certification for Infant and Toddler Eductor (ITE)";
      }

      return certificationTypeSubtitle;
    },
  },
});
</script>
