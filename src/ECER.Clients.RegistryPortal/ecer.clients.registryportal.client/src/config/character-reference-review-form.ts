import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import EceRecaptcha from "@/components/inputs/EceRecaptcha.vue";
import EceCharacterReferenceEvaluationPreview from "@/components/reference/inputs/EceCharacterReferenceEvaluationPreview.vue";
import EceReferenceContactPreview from "@/components/reference/inputs/EceReferenceContactPreview.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const characterReferenceReviewForm: Form = {
  id: "reviewForm",
  title: "Review form",
  inputs: {
    eceReferenceContactPreview: {
      id: "eceReferenceContactPreview",
      component: EceReferenceContactPreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    eceCharacterReferenceEvaluationPreview: {
      id: "eceCharacterReferenceEvaluationPreview",
      component: EceCharacterReferenceEvaluationPreview,
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
        rules: [Rules.hasCheckbox()],
      },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    recaptchaToken: {
      id: "recaptchaToken",
      component: EceRecaptcha,
      props: {
        rules: [Rules.required("Check to confirm you are not a robot")],
        recaptchaElementId: "recaptchaSubmit",
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
