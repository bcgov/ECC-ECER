import EceIcraApplicationCompetencyAssessment from "@/components/reference/inputs/icra-eligibility/EceIcraApplicationCompetencyAssessment.vue";
import type { Form } from "@/types/form";

const icraApplicationCompetencyAssessmentForm: Form = {
  id: "icraCompetencyAssessmentForm",
  title: "Work Experience Evaluation form",
  inputs: {
    competencyAssessment: {
      id: "competencyAssessment",
      component: EceIcraApplicationCompetencyAssessment,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default icraApplicationCompetencyAssessmentForm;
