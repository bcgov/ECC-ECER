import EceReconsiderationPreview from "@/components/inputs/EceReconsiderationPreview.vue";
import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import * as Rules from "@/utils/formRules";

import type { Form } from "@/types/form";

const reviewReconsiderationForm: Form = {
  id: "reviewForm",
  title: "Review",
  inputs: {
    reconsiderationPreview: {
      id: "reconsiderationPreview",
      component: EceReconsiderationPreview,
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
        rules: [
          Rules.hasCheckbox(
            "You must agree with the above statement to submit your dispute",
          ),
        ],
      },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default reviewReconsiderationForm;
