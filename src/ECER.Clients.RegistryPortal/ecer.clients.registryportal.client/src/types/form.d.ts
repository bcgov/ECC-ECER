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
