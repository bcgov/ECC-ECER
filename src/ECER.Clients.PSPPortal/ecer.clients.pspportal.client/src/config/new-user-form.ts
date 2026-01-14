import EceTextField from "@/components/inputs/EceTextField.vue";
import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const newUserForm: Form = {
  id: "newUserForm",
  title: "My contact details",
  components: {
    nameHeader: {
      id: "nameHeader",
      component: ECEHeader,
      props: {
        title: "Name",
      },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
      isInput: false,
    },
    lastName: {
      id: "lastName",
      component: EceTextField,
      props: {
        label: "Last name",
        rules: [],
        maxLength: 50,
        readonly: true,
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
        rules: [],
        maxLength: 50,
        readonly: true,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    preferredFirstName: {
      id: "preferredFirstName",
      component: EceTextField,
      props: {
        label: "Preferred first name (optional)",
        rules: [],
        maxLength: 50,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    contactInformationHeader: {
      id: "contactInformationHeader",
      component: ECEHeader,
      props: {
        title: "Contact Information",
      },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
      isInput: false,
    },
    jobTitle: {
      id: "jobTitle",
      component: EceTextField,
      props: {
        label: "Job title",
        rules: [Rules.required("Enter a job title")],
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
        label: "Primary contact number",
        rules: [Rules.phoneNumber("Enter a valid phone number"), Rules.required("Enter a phone number")],
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    phoneNumberExtension: {
      id: "phoneNumberExtension",
      component: EceTextField,
      props: {
        label: "Phone extension (optional)",
        rules: [],
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
    hasAcceptedTermsOfUse: {
      id: "hasAcceptedTermsOfUse",
      component: EceCheckbox,
      props: {
        label: "I have read and accept the <a href='/terms-of-use' target='_blank'>Terms of Use</a>",
        rules: [Rules.hasCheckbox("You must read and accept the Terms of Use")],
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
  },
};

export default newUserForm;
