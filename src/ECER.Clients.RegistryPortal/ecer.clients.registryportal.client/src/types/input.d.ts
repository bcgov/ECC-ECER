import type EceAddress from "@/components/inputs/EceAddress.vue";
import type EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import type EceRadio from "@/components/inputs/EceRadio.vue";
import type EceTextField from "@/components/inputs/EceTextField.vue";

interface EceTextFieldProps {
  rules: readonly ValidationRule$1[];
  label: string;
  type?: string;
  isNumeric?: boolean;
  disabled?: boolean;
  maxLength?: number;
}

interface EceAddressProps {
  addressLabel: string;
  hasCheckbox?: boolean;
}

interface Input {
  id: string;
  component: EceCheckbox | EceRadio | EceTextField | EceAddress;
  props: EceTextFieldProps | EceAddressProps;
}
