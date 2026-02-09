import IcraEligibilityDeclarationForm from "./icra-eligibility-work-experience-reference-declaration-form";
import icraWorkExperienceContactInformationForm from "./icra-eligibility-work-experience-contact-information-form";
import icraEligibilityWorkExperienceReferenceEvaluationForm from "./icra-eligibility-work-experience-reference-evaluation-form";
import workExperienceDeclineForm from "@/config/work-experience-reference-decline-form";
import IcraEligibilityWorkExperienceReferencePreviewForm from "./preview-form-icra-eligibility-work-experience-reference";

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
      form: icraWorkExperienceContactInformationForm,
      key: "item.2",
    },
    workExperienceEvaluation: {
      title: "",
      stage: "ReferenceEvaluation",
      form: icraEligibilityWorkExperienceReferenceEvaluationForm,
      key: "item.3",
    },
    review: {
      title: "Review",
      stage: "Review",
      form: IcraEligibilityWorkExperienceReferencePreviewForm,
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
