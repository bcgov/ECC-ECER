import EceCertificationTypePreview from "@/components/inputs/EceCertificationTypePreview.vue";
import EceCharacterReferencePreview from "@/components/inputs/EceCharacterReferencePreview.vue";
import EceContactInformationPreview from "@/components/inputs/EceContactInformationPreview.vue";
import EceFiveYearRenewalExplanationPreview from "@/components/inputs/EceFiveYearRenewalExplanationPreview.vue";
import EceProfessionalDevelopmentPreview from "@/components/inputs/EceProfessionalDevelopmentPreview.vue";
import EceWorkExperienceReferencePreview from "@/components/inputs/EceWorkExperienceReferencePreview.vue";
import type { Form } from "@/types/form";

const renewFiveYearExpiredLessThenFiveReviewForm: Form = {
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
    FiveYearRenewalExplanationPreview: {
      id: "FiveYearRenewalExplanationPreview",
      component: EceFiveYearRenewalExplanationPreview,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    professionalDevelopmentPreview: {
      id: "professionalDevelopmentPreview",
      component: EceProfessionalDevelopmentPreview,
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

export default renewFiveYearExpiredLessThenFiveReviewForm;
