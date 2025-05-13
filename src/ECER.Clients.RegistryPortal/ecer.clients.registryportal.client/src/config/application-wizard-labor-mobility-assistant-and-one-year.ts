import type { Wizard } from "@/types/wizard";

import characterReferencesForm from "./character-references-form";
import previewFormAssistantAndOneYearLaborMobility from "./preview-form-assistant-and-one-year-labor-mobility";
import profileInformationForm from "./profile-information-form";
import certificateInformationForm from "./certificate-information-form";

const applicationWizard: Wizard = {
  id: "form-1",
  steps: {
    certificateInformation: {
      stage: "CertificateInformation",
      title: "Certificate information",
      form: certificateInformationForm,
      key: "item.1",
    },
    profile: {
      stage: "ContactInformation",
      title: "Contact information",
      form: profileInformationForm,
      key: "item.2",
    },
    characterReferences: {
      stage: "CharacterReferences",
      title: "Character reference",
      form: characterReferencesForm,
      key: "item.3",
    },
    review: {
      stage: "Review",
      title: "Review and submit",
      form: previewFormAssistantAndOneYearLaborMobility,
      key: "item.4",
    },
  },
};

export default applicationWizard;
