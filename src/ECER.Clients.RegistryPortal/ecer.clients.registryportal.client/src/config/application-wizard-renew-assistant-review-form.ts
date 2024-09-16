import EceCertificationTypePreview from "@/components/inputs/EceCertificationTypePreview.vue";
import EceCharacterReferencePreview from "@/components/inputs/EceCharacterReferencePreview.vue";
import EceContactInformationPreview from "@/components/inputs/EceContactInformationPreview.vue";
import EceEducationPreview from "@/components/inputs/EceEducationPreview.vue";
import EceWorkExperienceReferencePreview from "@/components/inputs/EceWorkExperienceReferencePreview.vue";
import type { Form } from "@/types/form";

const renewAssistantReviewForm: Form = {
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
    educationPreview: {
      id: "educationPreview",
      component: EceEducationPreview,
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
    workExperienceReferencePreview: {
      id: "workExperienceReferencePreview",
      component: EceWorkExperienceReferencePreview,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default renewAssistantReviewForm;
