import EceCharacterReferenceEvaluation from "@/components/reference/inputs/EceCharacterReferenceEvaluation.vue";
import type { Form } from "@/types/form";

const characterReferenceReferenceEvaluationForm: Form = {
  id: "characterEvaluationForm",
  title: "character evaluation form",
  inputs: {
    characterReferenceEvaluation: {
      id: "characterReferenceEvaluation",
      component: EceCharacterReferenceEvaluation,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default characterReferenceReferenceEvaluationForm;
