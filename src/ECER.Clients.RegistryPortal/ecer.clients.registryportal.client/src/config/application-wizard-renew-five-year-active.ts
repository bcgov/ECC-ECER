import type { Wizard } from "@/types/wizard";

import characterReferencesForm from "./character-references-form";
import professionalDevelopmentForm from "./professional-development-form";
import profileInformationForm from "./profile-information-form";
import referencesForm from "./references-form";
import reviewAndSubmitForm from "./review-submit-form";

const applicationWizard: Wizard = {
  id: "form-1",
  steps: {
    profile: {
      stage: "ContactInformation",
      title: "Contact information",
      form: profileInformationForm,
      key: "item.1",
    },
    professionalDevelopments: {
      stage: "ProfessionalDevelopment",
      title: "Professional Development",
      form: professionalDevelopmentForm,
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
      form: reviewAndSubmitForm,
      key: "item.5",
    },
  },
};

export default applicationWizard;
