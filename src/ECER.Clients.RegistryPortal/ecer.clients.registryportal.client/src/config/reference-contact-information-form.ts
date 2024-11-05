import EceReferenceContact from "@/components/reference/inputs/EceReferenceContact.vue";
import type { Form } from "@/types/form";

const referenceContactInformationForm: Form = {
  id: "referenceContactInformationForm",
  title: "contact information form",
  inputs: {
    referenceContactInformation: {
      id: "referenceContactInformation",
      component: EceReferenceContact,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default referenceContactInformationForm;
