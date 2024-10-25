import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const previousNameForm: Form = {
  id: "previousNameForm",
  title: "Previous Name",
  inputs: {
    firstName: {
      id: "firstName",
      component: EceTextField,
      props: {
        label: "First name",
        rules: [Rules.noSpecialCharactersContactName()],
        maxLength: 100,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    middleName: {
      id: "middleName",
      component: EceTextField,
      props: {
        label: "Middle names (optional)",
        rules: [Rules.noSpecialCharactersContactName()],
        maxLength: 100,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    lastName: {
      id: "lastName",
      component: EceTextField,
      props: {
        label: "Last name",
        rules: [Rules.required("Enter your legal name"), Rules.noSpecialCharactersContactName()],
        maxLength: 100,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
  },
};

export default previousNameForm;
