import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const inviteUserForm: Form = {
  id: "inviteUserForm",
  title: "",
  components: {
    firstName: {
      id: "firstName",
      component: EceTextField,
      props: {
        label: "First name",
        rules: [Rules.required()],
        maxLength: 50,
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
        rules: [Rules.required()],
        maxLength: 50,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    email: {
      id: "email",
      component: EceTextField,
      props: {
        label: "Email",
        rules: [Rules.email(), Rules.required()],
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    jobTitle: {
      id: "jobTitle",
      component: EceTextField,
      props: {
        label: "Job title (optional)",
        rules: [],
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
  },
};

export default inviteUserForm;
