import EceCertificationTypePreview from "@/components/inputs/EceCertificationTypePreview.vue";
import EceCharacterReferencePreview from "@/components/inputs/EceCharacterReferencePreview.vue";
import EceEducationPreview from "@/components/inputs/EceEducationPreview.vue";
import type { Form } from "@/types/form";

const previewForm: Form = {
  id: "previewForm",
  title: "Review",
  inputs: {
    certificationSelectionPreview: {
      id: "certificationSelectionPreview",
      component: EceCertificationTypePreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    educationPreview: {
      id: "educationPreview",
      component: EceEducationPreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    characterReferencePreview: {
      id: "characterReferencePreview",
      component: EceCharacterReferencePreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default previewForm;
