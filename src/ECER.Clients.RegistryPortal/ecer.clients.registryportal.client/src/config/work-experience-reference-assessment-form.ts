import EceWorkExperienceReferenceAssessment from "@/components/reference/inputs/EceWorkExperienceReferenceAssessment.vue";
import type { Form } from "@/types/form";

const workExperienceAssessmentForm: Form = {
  id: "workExperienceAssessmentForm",
  title: "Work Experience Assessment form",
  inputs: {
    workExperienceAssessment: {
      id: "workExperienceAssessment",
      component: EceWorkExperienceReferenceAssessment,
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
