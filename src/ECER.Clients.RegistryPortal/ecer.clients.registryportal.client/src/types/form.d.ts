import type EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import type EceRadio from "@/components/inputs/EceRadio.vue";
import type EceTextField from "@/components/inputs/EceTextField.vue";

interface Props {
  type?: string;
  isNumeric?: boolean;
  rules: readonly ValidationRule$1[];
  label: string;
}

interface Input {
  id: string;
  component: EceCheckbox | EceRadio | EceTextField;
  props: Props;
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
