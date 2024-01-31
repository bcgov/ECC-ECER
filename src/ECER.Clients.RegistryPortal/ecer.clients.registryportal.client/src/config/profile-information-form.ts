import EceAddresses from "@/components/inputs/EceAddresses.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const profileInformationForm: Form = {
  id: "profile-information-form",
  title: "Contact Information",
  inputs: {
    legalLastName: {
      id: "legalLastName",
      component: EceTextField,
      props: { label: "Legal Last Name", rules: [Rules.required()], maxLength: 50, disabled: true },
    },
    legalFirstName: {
      id: "legalFirstName",
      component: EceTextField,
      props: { label: "Legal First Name", rules: [Rules.required()], maxLength: 50, disabled: true },
    },
    legalMiddleName: {
      id: "legalMiddleName",
      component: EceTextField,
      props: { label: "Legal Middle Name", rules: [], maxLength: 50 },
    },
    preferredName: {
      id: "preferredName",
      component: EceTextField,
      props: { label: "Preferred Name (optional)", rules: [], maxLength: 50 },
    },
    previousName: {
      id: "previousName",
      component: EceTextField,
      props: { label: "Previous Name (if applicable)", rules: [], maxLength: 50 },
    },
    dateOfBirth: {
      id: "dateOfBirth",
      component: EceTextField,
      props: { label: "Date of Birth", type: "date", rules: [Rules.required()], disabled: true },
    },
    addresses: {
      id: "addresses",
      component: EceAddresses,
      props: {
        residential: { addressLabel: "Residential" },
        mailing: { addressLabel: "Mailing" },
      },
    },
    primaryContactNumber: {
      id: "primaryContactNumber",
      component: EceTextField,
      props: { label: "Primary Contact Number", isNumeric: true, rules: [Rules.phoneNumber(), Rules.required()] },
    },
    alternateContactNumber: {
      id: "alternateContactNumber",
      component: EceTextField,
      props: { label: "Alternate Contact Number", isNumeric: true, rules: [] },
    },
    email: {
      id: "email",
      component: EceTextField,
      props: { label: "Email", rules: [Rules.email(), Rules.required()] },
    },
  },
};

export default profileInformationForm;
