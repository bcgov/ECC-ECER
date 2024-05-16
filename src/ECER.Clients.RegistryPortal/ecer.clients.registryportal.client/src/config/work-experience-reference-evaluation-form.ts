import EceWorkExperienceReferenceEvaluation from "@/components/reference/inputs/EceWorkExperienceReferenceEvaluation.vue";
import type { Form } from "@/types/form";

const workExperienceEvaluationForm: Form = {
  id: "WorkExperienceEvaluationForm",
  title: "Work Experience Evaluation form",
  inputs: {
    workExperienceEvaluation: {
      id: "workExperienceEvaluation",
      component: EceWorkExperienceReferenceEvaluation,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default workExperienceEvaluationForm;
