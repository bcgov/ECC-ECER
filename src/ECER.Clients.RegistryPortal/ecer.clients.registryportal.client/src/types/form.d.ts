import type { Input } from "@/types/input";

interface Form {
  id: string;
  title: string;
  inputs: {
    [id: string]: Input;
  };
}
