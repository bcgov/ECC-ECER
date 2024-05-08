import type { Input } from "@/types/input";

interface Form {
  id: string;
  title: string;
  inputs: {
    [id: string]: Input;
  };
}

// Wrap any type to type a dropdown list for v-select and v-autocomplete
interface DropdownWrapper<T> {
  title: string;
  value: T;
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
