import type { Form } from "@/types/form";

interface Step {
  stage: ApplicationStage | ReferenceStage;
  title: string;
  subtitle?: string;
  form: Form;
  [key: string]: any;
}

interface Wizard {
  id: string;
  steps: {
    [id: string]: Step;
  };
}

type ApplicationStage = "CertificationType" | "Declaration" | "ContactInformation" | "Education" | "CharacterReferences" | "WorkReferences" | "Review";
type RenewalAssitantStage = "Declaration" | "ContactInformation" | "Review";
type ReferenceStage = "Declaration" | "Decline" | "ContactInformation" | "Assessment" | "Review" | "ReferenceEvaluation";
