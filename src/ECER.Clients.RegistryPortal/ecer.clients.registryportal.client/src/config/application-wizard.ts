import type { Wizard } from "@/types/wizard";

import certificationTypeForm from "./certification-type-form";
import declarationForm from "./declaration-form";
import educationForm from "./education-form";
import profileInformationForm from "./profile-information-form";
import referencesForm from "./references.form";

const applicationWizard: Wizard = {
  id: "form-1",
  steps: {
    certificationType: {
      id: "certification-type",
      title: "Certification Selection",
      form: certificationTypeForm,
      key: "item.1",
    },
    declaration: {
      id: "declaration",
      title: "Declaration & Consent",
      subtitle: "Read the following statements and use the checkbox to indicate you understand and agree",
      form: declarationForm,
      key: "item.2",
    },
    profile: {
      id: "profile",
      title: "Contact Information",
      form: profileInformationForm,
      key: "item.3",
    },
    education: {
      id: "education",
      title: "Education",
      form: educationForm,
      key: "item.4",
    },
    characterReference: {
      id: "character-reference",
      title: "Character Reference",
      form: referencesForm,
      key: "item.5",
    },
    WorkReference: {
      id: "work-reference",
      title: "Work Experience Reference",
      form: referencesForm,
      key: "item.6",
    },
    review: {
      id: "review ",
      title: "Preview & Submit",
      form: referencesForm,
      key: "item.7",
    },
  },
};

export default applicationWizard;
