import EceWorkExperienceReferenceDetails from "@/components/reference/inputs/EceWorkExperienceReferenceDetails.vue";
import type { Form } from "@/types/form";

const workExperienceAssessmentForm: Form = {
  id: "workExAssessmentForm",
  title: "Work Experience Assessment form",
  inputs: {
    workExAssessment: {
      id: "workExAssessment",
      component: EceWorkExperienceReferenceDetails,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default workExperienceAssessmentForm;
