import type { Wizard } from "@/types/wizard";

// import internationalCertificationForm from "./international-certification-form";
// import employmentExperienceForm from "./employment-experience-form";

import previewFormIcraEligibility from "./preview-form-icra-eligibility.ts";
import profileInformationForm from "./profile-information-form";
import characterReferencesForm from "./character-references-form.ts";

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
      stage: "InternationalCertification",
      title: "International certification",
      form: characterReferencesForm,
      key: "item.2",
    },
    characterReferences: {
      stage: "EmploymentExperience",
      title: "Employment experience",
      form: characterReferencesForm,
      key: "item.3",
    },
    review: {
      stage: "Review",
      title: "Review and submit",
      form: previewFormIcraEligibility,
      key: "item.5",
    },
  },
};

export default applicationWizard;
