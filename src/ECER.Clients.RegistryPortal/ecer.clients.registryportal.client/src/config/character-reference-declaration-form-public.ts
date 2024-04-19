import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";

const characterReferencePublicForm: Form = {
  id: "characterReferenceDeclarationForm",
  title: "Declaration Reference",
  inputs: {
    characterReferenceDeclarationForm: {
      id: "characterReferenceDeclarationForm",
      component: EceTextField,
      props: { label: "user has a character reference this does not work" },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default characterReferencePublicForm;
