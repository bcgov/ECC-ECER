import EceIcraWorkExperienceContactPreview from "@/components/reference/inputs/icra-eligibility/EceIcraWorkExperienceContactPreview.vue";
import EceIcraEligibilityWorkExperienceReferenceEvaluationPreview from "@/components/reference/inputs/icra-eligibility/EceIcraEligibilityWorkExperienceReferenceEvaluationPreview.vue";

import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import EceRecaptcha from "@/components/inputs/EceRecaptcha.vue";
import * as Rules from "@/utils/formRules";

import type { Form } from "@/types/form";

const previewForm: Form = {
  id: "previewForm",
  title: "Review",
  inputs: {
    contactInformationPreview: {
      id: "contactInformationPreview",
      component: EceIcraWorkExperienceContactPreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    workExperienceEvaluationPreview: {
      id: "workExperienceEvaluationPreview",
      component: EceIcraEligibilityWorkExperienceReferenceEvaluationPreview,
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
        rules: [Rules.hasCheckbox("You must agree with the above statement to submit your reference")],
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

export default previewForm;
