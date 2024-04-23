import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";

const characterReferenceContactForm: Form = {
  id: "responseForm",
  title: "contact information form",
  inputs: {
    responseAccept: {
      id: "response",
      component: EceTextField,
      props: { label: "contact information form reference accepted this does not work" },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default characterReferenceContactForm;
