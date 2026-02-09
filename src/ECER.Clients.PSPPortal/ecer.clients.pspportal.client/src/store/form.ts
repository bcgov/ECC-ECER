import { defineStore } from "pinia";

export interface FormData {
  [key: string]: any;
}

export interface FormState {
  formData: FormData;
}

export const useFormStore = defineStore("form", {
  state: (): FormState => ({
    formData: {} as FormData,
  }),
  actions: {
    initializeForm(formData: FormData): void {
      this.formData = formData;
    },
    setFormData(formData: FormData): void {
      this.formData = { ...this.formData, ...formData };
    },
  },
});
