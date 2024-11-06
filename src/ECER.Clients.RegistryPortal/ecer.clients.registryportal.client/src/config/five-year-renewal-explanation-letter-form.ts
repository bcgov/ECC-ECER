import EceFiveYearRenewalInformation from "@/components/inputs/EceFiveYearRenewalInformation.vue";
import type { Form } from "@/types/form";

const FiveYearRenewalExplanationForm: Form = {
  id: "explanationLetterForm",
  title: "Explanation Letter",
  inputs: {
    fiveYearRenewalExplanation: {
      id: "fiveYearRenewalExplanationChoice",
      component: EceFiveYearRenewalInformation,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
    renewalExplanationOther: {
      id: "renewalExplanationOther",
      component: null,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default FiveYearRenewalExplanationForm;
