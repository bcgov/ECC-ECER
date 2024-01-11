import type EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import type EceRadio from "@/components/inputs/EceRadio.vue";
import type EceTextField from "@/components/inputs/EceTextField.vue";

interface Input {
  id: string;
  label: string;
  type: EceCheckbox | EceRadio | EceTextField;
  rules: readonly ValidationRule$1[];
}

interface Step {
  id: string;
  title: string;
  inputs: Input[];
  [key: string]: any;
}

interface Form {
  id: string;
  steps: Step[];
}
