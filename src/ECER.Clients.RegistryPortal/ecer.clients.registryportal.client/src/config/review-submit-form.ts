import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";

const reviewAndSubmit: Form = {
  id: "reviewAndSubmitForm",
  title: "Review and Submit",
  inputs: {
    reviewAndSubmit: {
      id: "reviewAndSubmit",
      component: EceTextField,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default reviewAndSubmit;