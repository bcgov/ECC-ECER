import type { Input } from "@/types/input";

interface Form {
  id: string;
  title: string;
  components: {
    [id: string]: Component;
  };
}

// Extend Input to include needsModelValue field
// If Input is defined elsewhere, you may need to use module augmentation
// For now, assuming we can add this to the Input type definition
interface Component {
  id: string;
  component: any;
  props?: Record<string, any>;
  cols: {
    md?: number;
    lg?: number;
    xl?: number;
  };
  isInput?: boolean;
}

// Wrap any type to type a dropdown list for v-select and v-autocomplete
interface DropdownWrapper<T> {
  title: string;
  value: T;
  code?: string;
}

// Wrap any type to a Radio button list for v-radio
interface RadioButtonWrapper<T> {
  label: string;
  value: T;
}

// Wrap any type to a checkbox list for v-checkbox
interface CheckBoxWrapper<T> {
  label: string;
  value: T;
}
