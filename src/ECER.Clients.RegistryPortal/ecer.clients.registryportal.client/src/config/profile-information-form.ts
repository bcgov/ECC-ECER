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
      props: { label: "Legal Middle Name", rules: [Rules.required()], maxLength: 50 },
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
    residentialMailingAddress: {
      id: "residentialMailingAddress",
      component: EceTextField,
      props: { label: "Residential Mailing Address", rules: [Rules.required()] },
    },
    cityTown: {
      id: "cityTown",
      component: EceTextField,
      props: { label: "City/Town", rules: [Rules.required()] },
    },
    province: {
      id: "province",
      component: EceTextField,
      props: { label: "Province", rules: [Rules.required()] },
    },
    postalCode: {
      id: "postalCode",
      component: EceTextField,
      props: { label: "Postal Code", rules: [Rules.required()] },
    },
    country: {
      id: "country",
      component: EceTextField,
      props: { label: "Country", rules: [Rules.required()] },
    },
    primaryContactNumber: {
      id: "primaryContactNumber",
      component: EceTextField,
      props: { label: "Primary Contact Number", isNumeric: true, rules: [Rules.phoneNumber(), Rules.required()] },
    },
    alternateContactNumber: {
      id: "alternateContactNumber",
      component: EceTextField,
      props: { label: "Alternate Contact Number", isNumeric: true, rules: [Rules.required()] },
    },
    email: {
      id: "email",
      component: EceTextField,
      props: { label: "Email", rules: [Rules.email(), Rules.required()] },
    },
  },
};

export default profileInformationForm;
