import type { Wizard } from "@/types/wizard";

import renewOneYearExpiredReviewForm from "./application-wizard-renew-one-year-expired-review-form";
import characterReferencesForm from "./character-references-form";
import oneYearRenewalExplanation from "./one-year-renewal-explanation-letter-form";
import professionalDevelopmentForm from "./professional-development-form";
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
    oneYearRenewalExplanation: {
      stage: "ExplanationLetter",
      title: "Renewal information",
      form: oneYearRenewalExplanation,
      key: "item.2",
    },
    professionalDevelopments: {
      stage: "ProfessionalDevelopment",
      title: "Professional development",
      form: professionalDevelopmentForm,
      key: "item.3",
    },
    characterReferences: {
      stage: "CharacterReferences",
      title: "Character reference",
      form: characterReferencesForm,
      key: "item.4",
    },
    workReference: {
      stage: "WorkReferences",
      title: "Work experience reference",
      form: referencesForm,
      key: "item.5",
    },
    review: {
      stage: "Review",
      title: "Review and submit",
      form: renewOneYearExpiredReviewForm,
      key: "item.6",
    },
  },
};

export default applicationWizard;
