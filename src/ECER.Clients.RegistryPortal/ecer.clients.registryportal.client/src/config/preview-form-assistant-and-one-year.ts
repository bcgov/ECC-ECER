import EceCertificationTypePreview from "@/components/inputs/EceCertificationTypePreview.vue";
import EceCharacterReferencePreview from "@/components/inputs/EceCharacterReferencePreview.vue";
import EceContactInformationPreview from "@/components/inputs/EceContactInformationPreview.vue";
import EceEducationPreview from "@/components/inputs/EceEducationPreview.vue";
import type { Form } from "@/types/form";

const previewForm: Form = {
  id: "previewForm",
  title: "Preview",
  inputs: {
    certificationSelectionPreview: {
      id: "certificationSelectionPreview",
      component: EceCertificationTypePreview,
      props: {},
      cols: {
        md: 8,
        lg: 8,
        xl: 6,
      },
    },
    contactInformationPreview: {
      id: "contactInformationPreview",
      component: EceContactInformationPreview,
      props: {},
      cols: {
        md: 8,
        lg: 8,
        xl: 6,
      },
    },
    educationPreview: {
      id: "educationPreview",
      component: EceEducationPreview,
      props: {},
      cols: {
        md: 8,
        lg: 8,
        xl: 6,
      },
    },
    characterReferencePreview: {
      id: "characterReferencePreview",
      component: EceCharacterReferencePreview,
      props: {},
      cols: {
        md: 8,
        lg: 8,
        xl: 6,
      },
    },
  },
};

export default previewForm;
