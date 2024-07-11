import type { Wizard } from "@/types/wizard";

import characterReferencesForm from "./character-references-form";
import educationForm from "./education-form";
import previewForm from "./preview-form";
import profileInformationForm from "./profile-information-form";
import referencesForm from "./references-form";

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
    workReference: {
      stage: "WorkReferences",
      title: "Work Experience References",
      form: referencesForm,
      key: "item.4",
    },
    review: {
      stage: "Review",
      title: "Preview & Submit",
      form: previewForm,
      key: "item.5",
    },
  },
};

export default applicationWizard;
