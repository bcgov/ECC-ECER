import EceCharacterReferenceContact from "@/components/reference/inputs/EceCharacterReferenceContact.vue";
import type { Form } from "@/types/form";

const referenceContactInformationForm: Form = {
  id: "referenceContactInformationForm",
  title: "contact information form",
  inputs: {
    referenceContactInformation: {
      id: "referenceContactInformation",
      // Can we use the same component for both forms? Maybe we config with props for CHAR vs WE reference?
      component: EceCharacterReferenceContact,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default referenceContactInformationForm;
