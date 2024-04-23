import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";

const characterReferenceDeclineForm: Form = {
  id: "declineForm",
  title: "Character Reference",
  inputs: {
    referenceDecline: {
      id: "decline",
      component: EceTextField,
      props: { label: "user declined this does not work" },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default characterReferenceDeclineForm;
