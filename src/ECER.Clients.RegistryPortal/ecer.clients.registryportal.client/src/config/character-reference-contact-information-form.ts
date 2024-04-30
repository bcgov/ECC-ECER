import EceCharacterReferenceContact from "@/components/reference/inputs/EceCharacterReferenceContact.vue";
import type { Form } from "@/types/form";

const characterReferenceContactForm: Form = {
  id: "responseForm",
  title: "contact information form",
  inputs: {
    responseAccept: {
      id: "response",
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

export default characterReferenceContactForm;
