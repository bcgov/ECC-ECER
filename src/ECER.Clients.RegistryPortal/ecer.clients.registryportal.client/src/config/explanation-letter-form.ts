import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";

const explanationLetter: Form = {
  id: "explanationLetterForm",
  title: "Explanation Letter",
  inputs: {
    explanationLetter: {
      id: "explanationLetter",
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

export default explanationLetter;
