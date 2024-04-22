import characterReferenceDeclarationForm from "@/config/character-reference-declaration-form-public";
import type { Wizard } from "@/types/wizard";

const characterReferenceWizardConfig: Wizard = {
  id: "characterReferenceWizard",
  steps: {
    declaration: {
      title: "",
      stage: "Declaration",
      form: characterReferenceDeclarationForm,
      key: "item.1",
    },
  },
};

export default characterReferenceWizardConfig;
