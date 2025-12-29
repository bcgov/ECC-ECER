import type { Form } from "@/types/form";

interface Step {
  stage: ProgramStage;
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

type ProgramStage =
  | "ProgramOverview"
  | "EarlyChildhood"
  | "InfantAndToddler"
  | "SpecialNeeds"
  | "Review";
