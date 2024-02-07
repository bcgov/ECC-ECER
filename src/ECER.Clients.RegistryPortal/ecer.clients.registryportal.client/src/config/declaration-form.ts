import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const profileInformationForm: Form = {
  id: "declaration-form",
  title: "Declaration & Consent",
  inputs: {
    consentCheckbox: {
      id: "consentCheckbox",
      component: EceCheckbox,
      props: {
        rules: [Rules.hasCheckbox("Please agree to continue")],
        label: "I understand and agree with the statements above",
      },
    },
    applicantLegalName: {
      id: "applicantLegalName",
      component: EceTextField,
      props: {
        label: "Applicant Legal Name",
        readonly: true,
      },
    },
    signedDate: {
      id: "signedDate",
      component: EceTextField,
      props: {
        label: "Signed Date",
        type: "date",
        readonly: true,
      },
    },
  },
};

export default profileInformationForm;
