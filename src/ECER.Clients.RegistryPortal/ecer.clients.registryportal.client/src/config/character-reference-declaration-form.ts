import EceCharacterReferenceForm from "@/components/reference/inputs/EceCharacterReferenceDeclaration.vue";
import type { Form } from "@/types/form";

const characterReferenceForm: Form = {
  id: "declarationForm",
  title: "Declaration Reference",
  inputs: {
    declarationForm: {
      id: "declaration",
      component: EceCharacterReferenceForm,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default characterReferenceForm;
