import type { Wizard } from "@/types/wizard";

import characterReferencesForm from "./character-references-form";
import oneYearRenewalExplanationForm from "./one-year-renewal-explanation-letter-form";
import profileInformationForm from "./profile-information-form";
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
    oneYearRenewalExplanationLetter: {
      stage: "ExplanationLetter",
      title: "Renewal information",
      form: oneYearRenewalExplanationForm,
      key: "item.2",
    },
    characterReferences: {
      stage: "CharacterReferences",
      title: "Character references",
      form: characterReferencesForm,
      key: "item.3",
    },
    review: {
      stage: "Review",
      title: "Review and submit",
      form: reviewAndSubmitForm,
      key: "item.4",
    },
  },
};

export default applicationWizard;
