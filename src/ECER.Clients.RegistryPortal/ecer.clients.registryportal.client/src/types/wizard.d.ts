import type { Form } from "@/types/form";
import type { Components } from "@/types/openapi";

interface Step {
  stage: Components.Schemas.PortalStage | ReferenceStage;
  title: string;
  subtitle?: string;
  form: Form;
  [key: string]: any;
}

interface Step {
  stage: ReferenceStage;
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

type ReferenceStage = "Declaration" | "Decline" | "ContactInformation" | "Assessment" | "Review" | "ReferenceEvaluation";
