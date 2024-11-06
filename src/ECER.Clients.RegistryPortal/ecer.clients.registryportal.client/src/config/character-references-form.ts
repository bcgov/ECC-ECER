import EceCharacterReference from "@/components/inputs/EceCharacterReference.vue";
import type { Form } from "@/types/form";

const referencesForm: Form = {
  id: "characterReferencesForm",
  title: "Character Reference",
  inputs: {
    characterReferences: {
      id: "characterReferences",
      component: EceCharacterReference,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default referencesForm;
