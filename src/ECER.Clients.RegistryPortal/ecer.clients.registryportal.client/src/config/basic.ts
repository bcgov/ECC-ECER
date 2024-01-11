import EceDatePicker from "@/components/inputs/EceDatePicker.vue";
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
        { id: "legalLastName", label: "Legal Last Name", type: EceTextField, rules: [Rules.required()] },
        { id: "legalFirstName", label: "Legal First Name", type: EceTextField, rules: [Rules.required()] },
        { id: "legalMiddleName", label: "Legal Middle Name", type: EceTextField, rules: [Rules.required()] },
        { id: "preferredName", label: "Preferred Name (optional)", type: EceTextField, rules: [] },
        { id: "previousName", label: "Previous Name (if applicable)", type: EceTextField, rules: [] },
        { id: "dateOfBirth", label: "Date of Birth", type: EceDatePicker, rules: [Rules.required()] },
        { id: "residentialMailingAddress", label: "Residential Mailing Address", type: EceTextField, rules: [Rules.required()] },
        { id: "cityTown", label: "City/Town ", type: EceTextField, rules: [Rules.required()] },
        { id: "province", label: "Province", type: EceTextField, rules: [Rules.required()] },
        { id: "postalCode", label: "Postal Code", type: EceTextField, rules: [Rules.required()] },
        { id: "country", label: "Country", type: EceTextField, rules: [Rules.required()] },
        { id: "primaryContactNumber", label: "PrimaryContactNumber", type: EceTextField, rules: [Rules.phoneNumber(), Rules.required()] },
        { id: "alternateContactNumber", label: "AlternateContactNumber", type: EceTextField, rules: [Rules.required()] },
        { id: "email", label: "Email", type: EceTextField, rules: [Rules.email(), Rules.required()] },
      ],
      key: "item.1",
    },
    {
      id: "step-2",
      title: "Education",
      inputs: [
        { id: "input-3", label: "Email", type: EceTextField, rules: [] },
        { id: "input-4", label: "Phone", type: EceTextField, rules: [] },
      ],
      key: "item.2",
    },
    {
      id: "step-3",
      title: "Review",
      inputs: [
        { id: "input-3", label: "Email", type: EceTextField, rules: [] },
        { id: "input-4", label: "Phone", type: EceTextField, rules: [] },
      ],
      key: "item.2",
    },
  ],
};

export default basic;
