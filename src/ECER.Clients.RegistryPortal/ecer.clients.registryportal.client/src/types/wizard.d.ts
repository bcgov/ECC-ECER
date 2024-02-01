import type { Form } from "@/types/form";

interface Step {
  id: string;
  title: string;
  form: Form;
  [key: string]: any;
}

interface Wizard {
  id: string;
  steps: {
    [id: string]: Step;
  };
}
