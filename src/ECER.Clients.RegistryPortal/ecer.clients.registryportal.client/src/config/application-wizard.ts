import type { Wizard } from "@/types/wizard";

import educationForm from "./education-form";
import profileInformationForm from "./profile-information-form";
import referencesForm from "./references.form";

const applicationWizard: Wizard = {
  id: "form-1",
  steps: [
    {
      id: "step-1",
      title: "Contact Information",
      form: profileInformationForm,
      key: "item.1",
    },
    {
      id: "step-2",
      title: "Education",
      form: educationForm,
      key: "item.2",
    },
    {
      id: "step-3",
      title: "References",
      form: referencesForm,
      key: "item.3",
    },
    {
      id: "step-4",
      title: "Review",
      form: profileInformationForm,
      key: "item.4",
    },
    {
      id: "step-5",
      title: "Declaration",
      form: profileInformationForm,
      key: "item.5",
    },
  ],
};

export default applicationWizard;
