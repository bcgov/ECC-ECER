import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const icraEligibilityWorkExperienceReferencesUpsertForm: Form = {
  id: "icraEligibilityWorkExperienceReferenceUpsertForm",
  title: "Work Experience Reference",
  inputs: {
    lastName: {
      id: "lastName",
      component: EceTextField,
      props: {
        label: "Last name",
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
        label: "First name",
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
          Rules.required(),
          Rules.email(
            "Enter your reference's email in the format 'name@email.com'",
          ),
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
        label: "Phone number (optional)",
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

export default icraEligibilityWorkExperienceReferencesUpsertForm;
