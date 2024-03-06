import EceCertificationTypePreview from "@/components/inputs/EceCertificationTypePreview.vue";
import EceCharacterReferencePreview from "@/components/inputs/EceCharacterReferencePreview.vue";
import EceContactInformationPreview from "@/components/inputs/EceContactInformationPreview.vue";
import EceEducationPreview from "@/components/inputs/EceEducationPreview.vue";
import EceWorkExperienceReferencePreview from "@/components/inputs/EceWorkExperienceReferencePreview.vue";
import type { Form } from "@/types/form";

const previewForm: Form = {
  id: "previewForm",
  title: "Preview",
  inputs: {
    certificationSelectionPreview: {
      id: "certificationSelectionPreview",
      component: EceCertificationTypePreview,
      props: {},
    },
    contactInformationPreview: {
      id: "contactInformationPreview",
      component: EceContactInformationPreview,
      props: {},
    },
    educationPreview: {
      id: "educationPreview",
      component: EceEducationPreview,
      props: {},
    },
    characterReferencePreview: {
      id: "characterReferencePreview",
      component: EceCharacterReferencePreview,
      props: {},
    },
    workExperienceReferencePreview: {
      id: "workExperienceReferencePreview",
      component: EceWorkExperienceReferencePreview,
      props: {},
    },
  },
};

export default previewForm;
