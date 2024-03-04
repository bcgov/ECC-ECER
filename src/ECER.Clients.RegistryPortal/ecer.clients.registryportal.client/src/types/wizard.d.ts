import type { Form } from "@/types/form";
import type { Components } from "@/types/openapi";

interface Step {
  stage: Components.Schemas.PortalStage;
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
