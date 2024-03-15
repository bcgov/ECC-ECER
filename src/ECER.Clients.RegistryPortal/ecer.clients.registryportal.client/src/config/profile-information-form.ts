import EceAddresses from "@/components/inputs/EceAddresses.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const profileInformationForm: Form = {
  id: "profileInformationForm",
  title: "Contact Information",
  inputs: {
    legalLastName: {
      id: "legalLastName",
      component: EceTextField,
      props: {
        label: "Legal Last Name",
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
    legalFirstName: {
      id: "legalFirstName",
      component: EceTextField,
      props: {
        label: "Legal First Name",
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
    legalMiddleName: {
      id: "legalMiddleName",
      component: EceTextField,
      props: {
        label: "Legal Middle Name",
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
    preferredName: {
      id: "preferredName",
      component: EceTextField,
      props: {
        label: "Preferred Name (optional)",
        rules: [],
        maxLength: 50,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    dateOfBirth: {
      id: "dateOfBirth",
      component: EceTextField,
      props: {
        label: "Date of Birth",
        type: "date",
        rules: [Rules.required()],
        readonly: true,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    addresses: {
      id: "addresses",
      component: EceAddresses,
      props: {
        residential: { addressLabel: "Residential" },
        mailing: { addressLabel: "Mailing" },
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    primaryContactNumber: {
      id: "primaryContactNumber",
      component: EceTextField,
      props: {
        label: "Primary Contact Number",
        isNumeric: true,
        rules: [Rules.phoneNumber("Enter your primary 10-digit phone number"), Rules.required()],
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    alternateContactNumber: {
      id: "alternateContactNumber",
      component: EceTextField,
      props: {
        label: "Alternate Contact Number",
        isNumeric: true,
        rules: [Rules.phoneNumber("Enter your alternate 10-digit phone number")],
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
  },
};

export default profileInformationForm;
