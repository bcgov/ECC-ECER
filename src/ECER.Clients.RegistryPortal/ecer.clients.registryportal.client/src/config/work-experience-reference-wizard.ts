import workExperienceDeclarationForm from "@/config/work-experience-reference-declaration-form";
import workExperienceDeclineForm from "@/config/work-experience-reference-decline-form";
import workExperienceReviewForm from "@/config/work-experience-reference-review-form";
import type { Wizard } from "@/types/wizard";

import referenceContactInformationForm from "./reference-contact-information-form";
import workExperienceAssessmentForm from "./work-experience-reference-assessment-form";
import workExperienceEvaluationForm from "./work-experience-reference-evaluation-form";

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
    workExperienceEvaluation: {
      title: "",
      stage: "ReferenceEvaluation",
      form: workExperienceEvaluationForm,
      key: "item.3",
    },
    assessment: {
      title: "",
      stage: "Assessment",
      form: workExperienceAssessmentForm,
      key: "item.4",
    },
    review: {
      title: "Preview",
      stage: "Review",
      form: workExperienceReviewForm,
      key: "item.5",
    },
    decline: {
      title: "",
      stage: "Decline",
      form: workExperienceDeclineForm,
      key: "item.6",
    },
  },
};

export default workExperienceReferenceWizardConfig;
