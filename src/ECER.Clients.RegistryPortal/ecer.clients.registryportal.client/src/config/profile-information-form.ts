import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const profileInformationForm: Form = {
  id: "profile-information-form",
  title: "Contact Information",
  inputs: [
    {
      id: "legalLastName",
      component: EceTextField,
      props: { label: "Legal Last Name", rules: [Rules.required()] },
      type: typeof String,
    },
    {
      id: "legalFirstName",
      component: EceTextField,
      props: { label: "Legal First Name", rules: [Rules.required()] },
      type: typeof String,
    },
    {
      id: "legalMiddleName",
      component: EceTextField,
      props: { label: "Legal Middle Name", rules: [Rules.required()] },
      type: typeof String,
    },
    {
      id: "preferredName",
      component: EceTextField,
      props: { label: "Preferred Name (optional)", rules: [] },
      type: typeof String,
    },
    {
      id: "previousName",
      component: EceTextField,
      props: { label: "Previous Name (if applicable)", rules: [] },
      type: typeof String,
    },
    {
      id: "dateOfBirth",
      component: EceTextField,
      props: { label: "Date of Birth", type: "date", rules: [Rules.required()] },
      type: typeof String,
    },
    {
      id: "residentialMailingAddress",
      component: EceTextField,
      props: { label: "Residential Mailing Address", rules: [Rules.required()] },
      type: typeof String,
    },
    {
      id: "cityTown",
      component: EceTextField,
      props: { label: "City/Town", rules: [Rules.required()] },
      type: typeof String,
    },
    {
      id: "province",
      component: EceTextField,
      props: { label: "Province", rules: [Rules.required()] },
      type: typeof String,
    },
    {
      id: "postalCode",
      component: EceTextField,
      props: { label: "Postal Code", rules: [Rules.required()] },
      type: typeof String,
    },
    {
      id: "country",
      component: EceTextField,
      props: { label: "Country", rules: [Rules.required()] },
      type: typeof String,
    },
    {
      id: "primaryContactNumber",
      component: EceTextField,
      props: { label: "Primary Contact Number", isNumeric: true, rules: [Rules.phoneNumber(), Rules.required()] },
      type: typeof String,
    },
    {
      id: "alternateContactNumber",
      component: EceTextField,
      props: { label: "Alternate Contact Number", isNumeric: true, rules: [Rules.required()] },
      type: typeof String,
    },
    {
      id: "email",
      component: EceTextField,
      props: { label: "Email", rules: [Rules.email(), Rules.required()] },
      type: typeof String,
    },
  ],
};

export default profileInformationForm;
