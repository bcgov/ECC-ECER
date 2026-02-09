import EceInternationalCertification from "@/components/inputs/EceInternationalCertification.vue";
import type { Form } from "@/types/form";

const internationalCertificationForm: Form = {
  id: "internationalCertificationForm",
  title: "International certification",
  inputs: {
    internationalCertification: {
      id: "internationalCertification",
      component: EceInternationalCertification,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default internationalCertificationForm;
