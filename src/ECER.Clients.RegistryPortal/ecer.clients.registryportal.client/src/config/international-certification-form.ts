import EceIcraInternationalCertification from "@/components/inputs/EceIcraInternationalCertification.vue";
import type { Form } from "@/types/form";

const internationalCertificationForm: Form = {
  id: "internationalCertificationForm",
  title: "International certification",
  inputs: {
    internationalCertificationList: {
      id: "internationalCertificationList",
      component: EceIcraInternationalCertification,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default internationalCertificationForm;
