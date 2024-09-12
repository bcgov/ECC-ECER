import EceWorkExperienceReference400HoursEvaluation from "@/components/reference/inputs/EceWorkExperienceReference400HoursEvaluation.vue";
import type { Form } from "@/types/form";

const workExperience400HoursEvaluationForm: Form = {
  id: "workExperience400HoursEvaluationForm",
  title: "Work Experience Evaluation form",
  inputs: {
    workExperience400HoursEvaluation: {
      id: "workExperience400HoursEvaluation",
      component: EceWorkExperienceReference400HoursEvaluation,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default workExperience400HoursEvaluationForm;
