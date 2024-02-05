import type { Wizard } from "@/types/wizard";

import declarationForm from "./declaration-form";
import educationForm from "./education-form";
import profileInformationForm from "./profile-information-form";
import referencesForm from "./references.form";

const applicationWizard: Wizard = {
  id: "form-1",
  steps: {
    declaration: {
      id: "declaration-step",
      title: "Declaration & Consent",
      subtitle: "Read the following statements and use the checkbox to indicate you understand and agree",
      form: declarationForm,
      key: "item.1",
    },
    profile: {
      id: "step-2",
      title: "Contact Information",
      form: profileInformationForm,
      key: "item.2",
    },
    education: {
      id: "step-3",
      title: "Education",
      form: educationForm,
      key: "item.3",
    },
    references: {
      id: "step-4",
      title: "References",
      form: referencesForm,
      key: "item.4",
    },
    review: {
      id: "step-5",
      title: "Review",
      form: profileInformationForm,
      key: "item.5",
    },
  },
};

export default applicationWizard;
