import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const characterReferencesUpsertForm: Form = {
  id: "characterReferenceUpsertForm",
  title: "Character Reference",
  inputs: {
    lastName: {
      id: "lastName",
      component: EceTextField,
      props: {
        label: "Last Name",
        rules: [Rules.required("Enter your reference's last name")],
        maxLength: 100,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    firstName: {
      id: "firstName",
      component: EceTextField,
      props: {
        label: "First Name",
        maxLength: 100,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    emailAddress: {
      id: "emailAddress",
      component: EceTextField,
      props: {
        label: "Email",
        rules: [
          Rules.required("Enter your reference's email in the format 'name@email.com'"),
          Rules.email("Enter your reference's email in the format 'name@email.com'"),
        ],
        maxLength: 100,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    phoneNumber: {
      id: "phoneNumber",
      component: EceTextField,
      props: {
        label: "Phone Number (optional)",
        rules: [Rules.phoneNumber("Enter your reference's valid phone number")],
      },
      cols: {
        md: 4,
        lg: 3,
        xl: 2,
      },
    },
  },
};

export default characterReferencesUpsertForm;
