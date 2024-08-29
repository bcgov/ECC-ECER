import EceOneYearRenewalInformation from "@/components/inputs/EceOneYearRenewalInformation.vue";
import type { Form } from "@/types/form";

const oneYearRenewalExplanation: Form = {
  id: "explanationLetterForm",
  title: "Explanation Letter",
  inputs: {
    oneYearRenewalExplanation: {
      id: "oneYearRenewalExplanation",
      component: EceOneYearRenewalInformation,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    explanationLetter: {
      id: "explanationLetter",
      component: null,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default oneYearRenewalExplanation;
