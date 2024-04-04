import DeclarationStepContent from "@/components/DeclarationStepContent.vue";
import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const declarationForm: Form = {
  id: "declarationForm",
  title: "Declaration & Consent",
  inputs: {
    declarationStepContent: {
      id: "declarationStepContent",
      component: DeclarationStepContent,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    consentCheckbox: {
      id: "consentCheckbox",
      component: EceCheckbox,
      props: {
        rules: [Rules.hasCheckbox("Please agree to continue")],
        label: "I understand and agree with the statements above",
        disabled: false,
        checkableOnce: true,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    applicantLegalName: {
      id: "applicantLegalName",
      component: EceTextField,
      props: {
        label: "Applicant Legal Name",
        readonly: true,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
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
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
  },
};

export default declarationForm;
