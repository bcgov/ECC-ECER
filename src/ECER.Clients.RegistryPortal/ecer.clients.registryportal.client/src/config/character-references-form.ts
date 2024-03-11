import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const referencesForm: Form = {
  id: "characterReferenceForm",
  title: "Character References",
  inputs: {
    firstName: {
      id: "characterReferenceFirstName",
      component: EceTextField,
      props: {
        label: "First Name",
        maxLength: 100,
        rules: [Rules.required()],
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    lastName: {
      id: "characterReferenceLastName",
      component: EceTextField,
      props: {
        label: "Last Name",
        maxLength: 100,
        rules: [Rules.required()],
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    phoneNumber: {
      id: "characterReferencePhoneNumber",
      component: EceTextField,
      props: {
        label: "Phone Number (optional)",
        isNumeric: true,
        maxLength: 20,
        rules: [Rules.phoneNumber()],
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    email: {
      id: "characterReferenceEmail",
      component: EceTextField,
      props: {
        label: "Email",
        maxLength: 200,
        rules: [Rules.required(), Rules.email()],
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
  },
};

export default referencesForm;
