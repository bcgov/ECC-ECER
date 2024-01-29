import type EceAddress from "@/components/inputs/EceAddress.vue";
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
  component: EceCheckbox | EceRadio | EceTextField | EceAddress;
  props: Props;
  type: Type;
}

interface Form {
  id: string;
  title: string;
  inputs: Input[];
}
