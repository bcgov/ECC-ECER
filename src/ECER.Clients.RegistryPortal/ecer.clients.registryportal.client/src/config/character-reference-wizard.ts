import characterReferenceDeclarationForm from "@/config/character-reference-declaration-form";
import characterReferenceDeclineForm from "@/config/character-reference-decline-form";
import type { Wizard } from "@/types/wizard";

import characterReferenceReferenceEvaluationForm from "./character-reference-reference-evaluation-form";
import characterReferenceReviewForm from "./character-reference-review-form";
import referenceContactInformationForm from "./reference-contact-information-form";

const characterReferenceWizardConfig: Wizard = {
  id: "characterReferenceWizard",
  steps: {
    declaration: {
      title: "",
      stage: "Declaration",
      form: characterReferenceDeclarationForm,
      key: "item.1",
    },
    contactInformation: {
      title: "",
      stage: "ContactInformation",
      form: referenceContactInformationForm,
      key: "item.2",
    },
    ReferenceEvaluation: {
      title: "",
      stage: "ReferenceEvaluation",
      form: characterReferenceReferenceEvaluationForm,
      key: "item.3",
    },
    review: {
      title: "Review",
      stage: "Review",
      form: characterReferenceReviewForm,
      key: "item.4",
    },
    decline: {
      title: "",
      stage: "Decline",
      form: characterReferenceDeclineForm,
      key: "item.5",
    },
  },
};

export default characterReferenceWizardConfig;
