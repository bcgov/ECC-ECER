import type { Wizard } from "@/types/wizard";

import characterReferencesForm from "./character-references-form";
import previewFormLaborMobilityAssistantAndOneYear from "./preview-form-labor-mobility-assistant-and-one-year";
import profileInformationForm from "./profile-information-form";

const applicationWizard: Wizard = {
  id: "form-1",
  steps: {
    profile: {
      stage: "ContactInformation",
      title: "Contact information",
      form: profileInformationForm,
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
      form: previewFormLaborMobilityAssistantAndOneYear,
      key: "item.3",
    },
  },
};

export default applicationWizard;
