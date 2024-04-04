import assessmentForm from "@/config/reference-assessment-form-public copy";
import contactInformationForm from "@/config/reference-contact-information-form-public";
import declarationForm from "@/config/reference-declaration-form-public";
import declineForm from "@/config/reference-decline-form-public";
import reviewForm from "@/config/reference-review-form-public";
import type { Wizard } from "@/types/wizard";

const referenceWizardConfig: Wizard = {
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

export default referenceWizardConfig;
