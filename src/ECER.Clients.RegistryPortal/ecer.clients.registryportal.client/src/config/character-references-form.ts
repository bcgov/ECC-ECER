import EceCharacterReference from "@/components/inputs/EceCharacterReference.vue";
import type { Form } from "@/types/form";

const referencesForm: Form = {
  id: "characterReferencesForm",
  title: "Character Reference",
  inputs: {
    characterReferences: {
      id: "characterReferences",
      component: EceCharacterReference,
      props: {},
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
  },
};

export default referencesForm;
