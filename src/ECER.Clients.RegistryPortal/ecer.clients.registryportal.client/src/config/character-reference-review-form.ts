import EceCharacterReferenceContactPreview from "@/components/reference/inputs/EceCharacterReferenceContactPreview.vue";
import EceCharacterReferenceEvaluationPreview from "@/components/reference/inputs/EceCharacterReferenceEvaluationPreview.vue";
import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import type { Form } from "@/types/form";
import { hasCheckbox } from "@/utils/formRules";

const characterReferenceReviewForm: Form = {
  id: "reviewForm",
  title: "Review form",
  inputs: {
    eceCharacterReferenceContactPreview: {
      id: "eceCharacterReferenceContactPreview",
      component: EceCharacterReferenceContactPreview,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    eceCharacterReferenceEvaluationPreview: {
      id: "eceCharacterReferenceEvaluationPreview",
      component: EceCharacterReferenceEvaluationPreview,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    confirmProvidedInformationIsRight: {
      id: "confirmProvidedInformationIsRight",
      component: EceCheckbox,
      props: {
        label:
          "To the best of my knowledge the provided information is complete and correct. I am aware the ECE Registry may contact me to verify or clarify the provided information.",
        rules: [hasCheckbox()],
      },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default characterReferenceReviewForm;
