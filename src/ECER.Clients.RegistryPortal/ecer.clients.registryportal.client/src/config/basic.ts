import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const basic: Form = {
  id: "form-1",
  steps: [
    {
      id: "step-1",
      title: "Contact Information",
      inputs: [
        { id: "legalLastName", component: EceTextField, props: { label: "Legal Last Name", rules: [Rules.required()] } },
        { id: "legalFirstName", component: EceTextField, props: { label: "Legal First Name", rules: [Rules.required()] } },
        { id: "legalMiddleName", component: EceTextField, props: { label: "Legal Middle Name", rules: [Rules.required()] } },
        { id: "preferredName", component: EceTextField, props: { label: "Preferred Name (optional)", rules: [] } },
        { id: "previousName", component: EceTextField, props: { label: "Previous Name (if applicable)", rules: [] } },
        { id: "dateOfBirth", component: EceTextField, props: { label: "Date of Birth", type: "date", rules: [Rules.required()] } },
        { id: "residentialMailingAddress", component: EceTextField, props: { label: "Residential Mailing Address", rules: [Rules.required()] } },
        { id: "cityTown", component: EceTextField, props: { label: "City/Town", rules: [Rules.required()] } },
        { id: "province", component: EceTextField, props: { label: "Province", rules: [Rules.required()] } },
        { id: "postalCode", component: EceTextField, props: { label: "Postal Code", rules: [Rules.required()] } },
        { id: "country", component: EceTextField, props: { label: "Country", rules: [Rules.required()] } },
        { id: "primaryContactNumber", component: EceTextField, props: { label: "PrimaryContactNumber", rules: [Rules.phoneNumber(), Rules.required()] } },
        { id: "alternateContactNumber", component: EceTextField, props: { label: "AlternateContactNumber", rules: [Rules.required()] } },
        { id: "email", component: EceTextField, props: { label: "Email", rules: [Rules.email(), Rules.required()] } },
      ],
      key: "item.1",
    },
    {
      id: "step-2",
      title: "Education",
      inputs: [],
      key: "item.2",
    },
    {
      id: "step-3",
      title: "References",
      inputs: [],
      key: "item.3",
    },
    {
      id: "step-4",
      title: "Review",
      inputs: [],
      key: "item.4",
    },
    {
      id: "step-5",
      title: "Declaration",
      inputs: [],
      key: "item.5",
    },
  ],
};

export default basic;
