import EceRadio from "@/components/inputs/EceRadio.vue";
import Recaptcha from "@/components/inputs/Recaptcha.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";

const characterReferenceDeclineForm: Form = {
  id: "declineForm",
  title: "Character Reference",
  inputs: {
    referenceDecline: {
      id: "decline",
      component: EceRadio,
      props: {
        options: [
          { key: "Iamunabletoatthistime", label: "I am unable to at this time" },
          { key: "Idonothavetheinformationrequired", label: "I do not have the information requested" },
          { key: "Idonotknowthisperson", label: "I do not know this person" },
          {
            key: "Idonotmeettherequirementstoprovideareference",
            label: "I do not meet the requirements to provide a reference",
          },
          { key: "Other", label: "Other" },
        ],
        title: "Tell us why you're unable to provide a reference",
        rules: [Rules.required("Select an option to continue")],
      },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    recaptchaKey: {
      id: "recaptchaKey",
      component: Recaptcha,
      props: {
        rules: [Rules.required("Select an option to continue")],
      },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default characterReferenceDeclineForm;
