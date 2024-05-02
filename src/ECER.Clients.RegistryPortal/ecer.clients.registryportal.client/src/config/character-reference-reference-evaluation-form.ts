import EceCharacterReferenceEvaluation from "@/components/reference/inputs/EceCharacterReferenceEvaluation.vue";
import type { Form } from "@/types/form";

const characterReferenceReferenceEvaluationForm: Form = {
  id: "characterEvaluationForm",
  title: "character evaluation form",
  inputs: {
    responseAccept: {
      id: "characterEvaluation",
      component: EceCharacterReferenceEvaluation,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default characterReferenceReferenceEvaluationForm;
