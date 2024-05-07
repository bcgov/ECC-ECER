import workExperienceAssessmentForm from "@/config/work-experience-reference-assessment-form";
import workExperienceDeclarationForm from "@/config/work-experience-reference-declaration-form";
import workExperienceDeclineForm from "@/config/work-experience-reference-decline-form";
import workExperienceReviewForm from "@/config/work-experience-reference-review-form";
import type { Wizard } from "@/types/wizard";

import referenceContactInformationForm from "./reference-contact-information-form";

const workExperienceReferenceWizardConfig: Wizard = {
  id: "workReferenceWizard",
  steps: {
    declaration: {
      title: "",
      stage: "Declaration",
      form: workExperienceDeclarationForm,
      key: "item.1",
    },
    contactInformation: {
      title: "",
      stage: "ContactInformation",
      form: referenceContactInformationForm,
      key: "item.2",
    },
    assessment: {
      title: "",
      stage: "Assessment",
      form: workExperienceAssessmentForm,
      key: "item.3",
    },
    review: {
      title: "Review",
      stage: "Review",
      form: workExperienceReviewForm,
      key: "item.5",
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
