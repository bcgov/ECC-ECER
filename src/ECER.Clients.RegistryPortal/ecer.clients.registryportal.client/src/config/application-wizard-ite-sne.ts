import type { Wizard } from "@/types/wizard";
import characterReferencesForm from "./character-references-form";
import educationForm from "./education-form";
import previewForm from "./preview-form-ite-sne";
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
    education: {
      stage: "Education",
      title: "Education",
      form: educationForm,
      key: "item.2",
    },
    characterReferences: {
      stage: "CharacterReferences",
      title: "Character reference",
      form: characterReferencesForm,
      key: "item.3",
    },
    review: {
      stage: "Review",
      title: "Review and submit",
      form: previewForm,
      key: "item.4",
    },
  },
};

export default applicationWizard;
