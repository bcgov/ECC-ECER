import EceCertificationTypePreview from "@/components/inputs/EceCertificationTypePreview.vue";
import EceCharacterReferencePreview from "@/components/inputs/EceCharacterReferencePreview.vue";
import EceContactInformationPreview from "@/components/inputs/EceContactInformationPreview.vue";
import EceOneYearRenewalExplanationPreview from "@/components/inputs/EceOneYearRenewalExplanationPreview.vue";
import type { Form } from "@/types/form";

const renewOneYearActiveReviewForm: Form = {
  id: "previewForm",
  title: "Review and submit",
  inputs: {
    certificationSelectionPreview: {
      id: "certificationSelectionPreview",
      component: EceCertificationTypePreview,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    contactInformationPreview: {
      id: "contactInformationPreview",
      component: EceContactInformationPreview,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    oneYearRenewalExplanationPreview: {
      id: "oneYearRenewalExplanationPreview",
      component: EceOneYearRenewalExplanationPreview,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    characterReferencePreview: {
      id: "characterReferencePreview",
      component: EceCharacterReferencePreview,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default renewOneYearActiveReviewForm;