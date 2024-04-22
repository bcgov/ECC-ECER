import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";

const characterReferenceReferenceEvaluationForm: Form = {
  id: "characterEvaluationForm",
  title: "character evaluation form",
  inputs: {
    responseAccept: {
      id: "characterEvaluation",
      component: EceTextField,
      props: { label: "charactered evaluation form reference accepted this does not work" },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default characterReferenceReferenceEvaluationForm;
