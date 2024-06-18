import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import EceRecaptcha from "@/components/inputs/EceRecaptcha.vue";
import EceReferenceContactPreview from "@/components/reference/inputs/EceReferenceContactPreview.vue";
import EceWorkExperienceReferenceAssessmentPreview from "@/components/reference/inputs/EceWorkExperienceReferenceAssessmentPreview.vue";
import EceWorkExperienceReferenceEvaluationPreview from "@/components/reference/inputs/EceWorkExperienceReferenceEvaluationPreview.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const workExperienceReviewForm: Form = {
  id: "reviewForm",
  title: "Review form",
  inputs: {
    eceReferenceContactPreview: {
      id: "eceReferenceContactPreview",
      component: EceReferenceContactPreview,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    eceWorkExperienceReferenceEvaluationPreview: {
      id: "eceWorkExperienceReferenceEvaluationPreview",
      component: EceWorkExperienceReferenceEvaluationPreview,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    eceWorkExperienceReferenceAssessmentPreview: {
      id: "eceWorkExperienceReferenceAssessmentPreview",
      component: EceWorkExperienceReferenceAssessmentPreview,
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
      },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default workExperienceReviewForm;
