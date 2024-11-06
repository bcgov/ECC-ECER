import EceCharacterReferenceForm from "@/components/reference/inputs/EceCharacterReferenceDeclaration.vue";
import type { Form } from "@/types/form";

const characterReferenceForm: Form = {
  id: "declarationForm",
  title: "Declaration Reference",
  inputs: {
    willProvideReference: {
      id: "willProvideReference",
      component: EceCharacterReferenceForm,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default characterReferenceForm;
