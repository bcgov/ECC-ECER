import IcraApplicationWorkReferenceDeclarationForm from "./icra-application-work-experience-reference-declaration-form";
import icraWorkExperienceContactInformationForm from "./icra-eligibility-work-experience-contact-information-form";
import workExperienceAssessmentForm from "../work-experience-reference-assessment-form";
import workExperienceDeclineForm from "@/config/work-experience-reference-decline-form";
import icraWorkExperienceReferencePreviewForm from "./preview-form-icra-work-experience-reference";

import type { Wizard } from "@/types/wizard";

const workExperienceReferenceWizardConfig: Wizard = {
  id: "workReferenceWizard",
  steps: {
    declaration: {
      title: "",
      stage: "Declaration",
      form: IcraApplicationWorkReferenceDeclarationForm,
      key: "item.1",
    },
    contactInformation: {
      title: "",
      stage: "ContactInformation",
      form: icraWorkExperienceContactInformationForm,
      key: "item.2",
    },
    assessment: {
      stage: "Assessment",
      title: "",
      form: workExperienceAssessmentForm,
      key: "item.3",
    },
    review: {
      title: "Review",
      stage: "Review",
      form: icraWorkExperienceReferencePreviewForm,
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
