import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";

const referencePublicForm: Form = {
  id: "referenceForm",
  title: "Experience Reference",
  inputs: {
    referenceDecline: {
      id: "decline",
      component: EceTextField,
      props: { label: "user declined this does not work" },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default referencePublicForm;
