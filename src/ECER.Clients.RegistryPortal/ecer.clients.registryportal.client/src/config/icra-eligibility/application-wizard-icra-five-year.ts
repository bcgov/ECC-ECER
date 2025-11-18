import type { Wizard } from "@/types/wizard";

import characterReferencesForm from "../character-references-form";
import educationForm from "../education-form";
import previewFormIcraFiveYear from "./preview-form-icra-five-year";

const applicationWizard: Wizard = {
  id: "form-1",
  steps: {
    education: {
      stage: "Education",
      title: "Education",
      form: educationForm,
      key: "item.1",
    },
    characterReferences: {
      stage: "CharacterReferences",
      title: "Character reference",
      form: characterReferencesForm,
      key: "item.2",
    },
    review: {
      stage: "Review",
      title: "Review and submit",
      form: previewFormIcraFiveYear,
      key: "item.3",
    },
  },
};

export default applicationWizard;
