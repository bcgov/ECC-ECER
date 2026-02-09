import EceIcraEligibilityWorkExperienceReferenceEvaluation from "@/components/reference/inputs/icra-eligibility/EceIcraEligibilityWorkExperienceReferenceEvaluation.vue";
import type { Form } from "@/types/form";

const workExperienceEvaluationForm: Form = {
  id: "WorkExperienceEvaluationForm",
  title: "Work Experience Evaluation form",
  inputs: {
    workExperienceEvaluation: {
      id: "workExperienceEvaluation",
      component: EceIcraEligibilityWorkExperienceReferenceEvaluation,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default workExperienceEvaluationForm;
