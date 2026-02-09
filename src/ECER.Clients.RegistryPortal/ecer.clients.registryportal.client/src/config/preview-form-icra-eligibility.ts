import EceContactInformationPreview from "@/components/inputs/EceContactInformationPreview.vue";
import EceInternationalCertificationPreview from "@/components/inputs/EceInternationalCertificationPreview.vue";
import EceEmploymentExperiencePreview from "@/components/inputs/EceEmploymentExperiencePreview.vue";
import type { Form } from "@/types/form";

const previewForm: Form = {
  id: "previewForm",
  title: "Review",
  inputs: {
    contactInformationPreview: {
      id: "contactInformationPreview",
      component: EceContactInformationPreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    internationalCertificationPreview: {
      id: "internationalCertificationPreview",
      component: EceInternationalCertificationPreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    employmentExperiencePreview: {
      id: "employmentExperiencePreview",
      component: EceEmploymentExperiencePreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default previewForm;
