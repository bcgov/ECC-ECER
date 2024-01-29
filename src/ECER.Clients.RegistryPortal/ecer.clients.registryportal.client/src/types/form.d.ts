import type EceAddress from "@/components/inputs/EceAddress.vue";
import type EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import type EceRadio from "@/components/inputs/EceRadio.vue";
import type EceTextField from "@/components/inputs/EceTextField.vue";

interface Props {
  type?: string;
  isNumeric?: boolean;
  disabled?: boolean;
  maxLength?: number;
  rules: readonly ValidationRule$1[];
  label: string;
}

interface Input {
  id: string;
  component: EceCheckbox | EceRadio | EceTextField | EceAddress;
  props: Props;
}

interface Form {
  id: string;
  title: string;
  inputs: {
    [id: string]: Input;
  };
}
