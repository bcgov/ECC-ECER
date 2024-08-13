import type { Wizard } from "@/types/wizard";

import characterReferencesForm from "./character-references-form";
import explanationLetterForm from "./explanation-letter-form";
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
    explanationLetter: {
      stage: "ExplanationLetter",
      title: "Explanation Letter",
      form: explanationLetterForm,
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
