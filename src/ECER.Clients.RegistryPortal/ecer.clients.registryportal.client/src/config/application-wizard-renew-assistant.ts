import type { Wizard } from "@/types/wizard";

import renewAssistantReviewForm from "./application-wizard-renew-assistant-review-form";
import characterReferencesForm from "./character-references-form";
import educationForm from "./education-form";
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
      title: "Character reference",
      form: characterReferencesForm,
      key: "item.3",
    },
    workReference: {
      stage: "WorkReferences",
      title: "Work experience reference",
      form: referencesForm,
      key: "item.4",
    },
    review: {
      stage: "Review",
      title: "Review and submit",
      form: renewAssistantReviewForm,
      key: "item.5",
    },
  },
};

export default applicationWizard;
