import type { Wizard } from "@/types/wizard";

import characterReferencesForm from "./character-references-form";
import previewFormLaborMobilityFiveYear from "./preview-form-labor-mobility-five-year";
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
    characterReferences: {
      stage: "CharacterReferences",
      title: "Character reference",
      form: characterReferencesForm,
      key: "item.2",
    },
    workReference: {
      stage: "WorkReferences",
      title: "Work experience reference",
      form: referencesForm,
      key: "item.3",
    },
    review: {
      stage: "Review",
      title: "Review and submit",
      form: previewFormLaborMobilityFiveYear,
      key: "item.4",
    },
  },
};

export default applicationWizard;
