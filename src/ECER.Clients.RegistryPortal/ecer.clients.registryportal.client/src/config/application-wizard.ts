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
      stage: "CertificationType",
      title: "Certification Selection",
      form: certificationTypeForm,
      key: "item.1",
    },
    declaration: {
      stage: "Declaration",
      title: "Declaration & Consent",
      subtitle: "Read the following statements and use the checkbox to indicate you understand and agree",
      form: declarationForm,
      key: "item.2",
    },
    profile: {
      stage: "ContactInformation",
      title: "Contact Information",
      form: profileInformationForm,
      key: "item.3",
    },
    education: {
      stage: "Education",
      title: "Education",
      form: educationForm,
      key: "item.4",
    },
    characterReference: {
      stage: "CharacterReferences",
      title: "Character Reference",
      form: referencesForm,
      key: "item.5",
    },
    workReference: {
      stage: "WorkReferences",
      title: "Work Experience Reference",
      form: referencesForm,
      key: "item.6",
    },
    review: {
      stage: "Review",
      title: "Preview & Submit",
      form: referencesForm,
      key: "item.7",
    },
  },
};

export default applicationWizard;
