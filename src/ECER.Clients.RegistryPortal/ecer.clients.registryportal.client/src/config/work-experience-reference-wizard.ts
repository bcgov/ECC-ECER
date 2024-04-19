import assessmentForm from "@/config/work-experience-reference-assessment-form-public";
import contactInformationForm from "@/config/work-experience-contact-information-form-public";
import declarationForm from "@/config/work-experience-reference-declaration-form-public";
import declineForm from "@/config/work-experience-reference-decline-form-public";
import reviewForm from "@/config/work-experience-reference-review-form-public";
import type { Wizard } from "@/types/wizard";

const workExperienceReferenceWizardConfig: Wizard = {
  id: "workReferenceWizard",
  steps: {
    declaration: {
      title: "",
      stage: "Declaration",
      form: declarationForm,
      key: "item.1",
    },
    contactInformation: {
      title: "Contact information",
      stage: "ContactInformation",
      form: contactInformationForm,
      key: "item.2",
    },
    assessment: {
      title: "Assessment",
      stage: "Assessment",
      form: assessmentForm,
      key: "item.3",
    },
    review: {
      title: "Review",
      stage: "Review",
      form: reviewForm,
      key: "item.5",
    },
    decline: {
      title: "",
      stage: "Decline",
      form: declineForm,
      key: "item.5",
    },
  },
};

export default workExperienceReferenceWizardConfig;
