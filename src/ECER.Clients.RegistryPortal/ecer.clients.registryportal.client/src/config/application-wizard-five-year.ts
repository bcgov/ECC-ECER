import type { Wizard } from "@/types/wizard";

import characterReferencesForm from "./character-references-form";
import educationForm from "./education-form";
import previewFormFiveYear from "./preview-form-five-year";
import profileInformationForm from "./profile-information-form";
import referencesForm from "./references-form";

const applicationWizard: Wizard = {
  id: "form-1",
  steps: {
    profile: {
      stage: "ContactInformation",
      title: "Contact information",
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
      title: "Character references",
      form: characterReferencesForm,
      key: "item.3",
    },
    workReference: {
      stage: "WorkReferences",
      title: "Work experience references",
      form: referencesForm,
      key: "item.4",
    },
    review: {
      stage: "Review",
      title: "Review and submit",
      form: previewFormFiveYear,
      key: "item.5",
    },
  },
};

export default applicationWizard;
