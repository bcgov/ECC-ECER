import type { Wizard } from "@/types/wizard";

import renewFiveYearExpiredLessThenFiveReviewForm from "./application-wizard-renew-five-year-expired-less-than-five-years-review-form";
import characterReferencesForm from "./character-references-form";
import fiveYearRenewalExplanationForm from "./five-year-renewal-explanation-letter-form";
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
    fiveYearRenewalExplanation: {
      stage: "ExplanationLetter",
      title: "Renewal information",
      form: fiveYearRenewalExplanationForm,
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
      form: renewFiveYearExpiredLessThenFiveReviewForm,
      key: "item.6",
    },
  },
};

export default applicationWizard;
