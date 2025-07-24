import workExperience400HoursReviewForm from "@/config/work-experience-reference-400-hours-review-form";
import workExperienceDeclarationForm from "@/config/work-experience-reference-declaration-form";
import workExperienceDeclineForm from "@/config/work-experience-reference-decline-form";
import type { Wizard } from "@/types/wizard";

import referenceContactInformationForm from "./reference-contact-information-form";
import workExperience400HoursEvaluationForm from "./work-experience-reference-400-hours-evaluation-form";

const workExperienceReference400HoursWizardConfig: Wizard = {
  id: "workReference400HoursWizard",
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
    workExperience400HoursEvaluation: {
      title: "",
      stage: "ReferenceEvaluation",
      form: workExperience400HoursEvaluationForm,
      key: "item.3",
    },
    review: {
      title: "Review",
      stage: "Review",
      form: workExperience400HoursReviewForm,
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

export default workExperienceReference400HoursWizardConfig;
