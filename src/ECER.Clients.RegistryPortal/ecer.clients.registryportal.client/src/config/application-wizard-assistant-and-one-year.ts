import type { Wizard } from "@/types/wizard";

import characterReferencesForm from "./character-references-form";
import educationForm from "./education-form";
import previewFormAssistantAndOneYear from "./preview-form-assistant-and-one-year";
import profileInformationForm from "./profile-information-form";

const applicationWizard: Wizard = {
  id: "form-1",
  steps: {
    profile: {
      stage: "ContactInformation",
      title: "Contact Information",
      form: profileInformationForm,
      key: "item.1",
    },
    education: {
      stage: "Education",
      title: "Education",
      form: educationForm,
      key: "item.2",
    },
    characterReferences: {
      stage: "CharacterReferences",
      title: "Character Reference",
      form: characterReferencesForm,
      key: "item.3",
    },
    review: {
      stage: "Review",
      title: "Preview & Submit",
      form: previewFormAssistantAndOneYear,
      key: "item.4",
    },
  },
};

export default applicationWizard;
