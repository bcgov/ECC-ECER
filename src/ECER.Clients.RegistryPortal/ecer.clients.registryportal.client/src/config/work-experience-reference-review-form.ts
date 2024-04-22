import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";

const workExperienceReviewForm: Form = {
  id: "reviewForm",
  title: "Review form",
  inputs: {
    review: {
      id: "review",
      component: EceTextField,
      props: { label: "review form this does not work should be end for stepper" },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default workExperienceReviewForm;
