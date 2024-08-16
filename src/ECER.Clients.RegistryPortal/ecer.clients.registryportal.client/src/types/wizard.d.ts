import type { Form } from "@/types/form";

interface Step {
  stage: ApplicationStage | ReferenceStage | RenewStage;
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

type ApplicationStage =
  | "CertificationType"
  | "Declaration"
  | "ContactInformation"
  | "Education"
  | "ExplanationLetter"
  | "ProfessionalDevelopment"
  | "CharacterReferences"
  | "WorkReferences"
  | "Review";
type ReferenceStage = "Declaration" | "Decline" | "ContactInformation" | "Assessment" | "Review" | "ReferenceEvaluation";
type RenewStage = "ContactInformation" | "ExplanationLetter" | "SingleEducation" | "CharacterReferences" | "WorkReferences" | "Review";
