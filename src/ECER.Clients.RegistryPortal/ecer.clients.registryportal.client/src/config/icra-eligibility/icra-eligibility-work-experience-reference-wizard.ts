import IcraEligibilityDeclarationForm from "./icra-eligibility-work-experience-reference-declaration-form";
import icraEligibilityWorkExperienceContactInformationForm from "./icra-eligibility-work-experience-contact-information-form";
import icraEligiblityWorkExperienceReferenceEvaluationForm from "./icra-eligibility-work-experience-reference-evaluation-form";
import workExperienceDeclineForm from "@/config/work-experience-reference-decline-form";
import IcraEligibilityPreviewForm from "./preview-form-icra-eligibility";

import type { Wizard } from "@/types/wizard";

const workExperienceReferenceWizardConfig: Wizard = {
  id: "workReferenceWizard",
  steps: {
    declaration: {
      title: "",
      stage: "Declaration",
      form: IcraEligibilityDeclarationForm,
      key: "item.1",
    },
    contactInformation: {
      title: "",
      stage: "ContactInformation",
      form: icraEligibilityWorkExperienceContactInformationForm,
      key: "item.2",
    },
    workExperienceEvaluation: {
      title: "",
      stage: "ReferenceEvaluation",
      form: icraEligiblityWorkExperienceReferenceEvaluationForm,
      key: "item.3",
    },
    review: {
      title: "Review",
      stage: "Review",
      form: IcraEligibilityPreviewForm,
      key: "item.4",
    },
    decline: {
      title: "",
      stage: "Decline",
      form: workExperienceDeclineForm,
      key: "item.5",
    },
  },
};

export default workExperienceReferenceWizardConfig;
