import type { Form } from "@/types/form";

interface Step {
  // Shimmed types to be removed once the types are updated
  stage:
    | ApplicationStage
    | ReferenceStage
    | RenewStage
    | IcraEligibilityStage
    | ReconsiderationStage;
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
  | "CertificateInformation"
  | "CertificationType"
  | "Declaration"
  | "ContactInformation"
  | "Education"
  | "ExplanationLetter"
  | "ProfessionalDevelopment"
  | "CharacterReferences"
  | "WorkReferences"
  | "Review";
type IcraEligibilityStage =
  | "ContactInformation"
  | "InternationalCertification"
  | "EmploymentExperience"
  | "Review";
type ReferenceStage =
  | "Declaration"
  | "Decline"
  | "ContactInformation"
  | "Assessment"
  | "Review"
  | "ReferenceEvaluation";
type RenewStage =
  | "ContactInformation"
  | "ExplanationLetter"
  | "Education"
  | "CharacterReferences"
  | "WorkReferences"
  | "Review";
type ReconsiderationStage = "Reconsideration" | "Review";
