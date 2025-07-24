import EceCertificateInformationPreview from "@/components/inputs/EceCertificateInformationPreview.vue";
import EceCharacterReferencePreview from "@/components/inputs/EceCharacterReferencePreview.vue";
import EceContactInformationPreview from "@/components/inputs/EceContactInformationPreview.vue";
import type { Form } from "@/types/form";

const previewForm: Form = {
  id: "previewForm",
  title: "Review",
  inputs: {
    certificateInformationPreview: {
      id: "certificateInformationPreview",
      component: EceCertificateInformationPreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    contactInformationPreview: {
      id: "contactInformationPreview",
      component: EceContactInformationPreview,
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
