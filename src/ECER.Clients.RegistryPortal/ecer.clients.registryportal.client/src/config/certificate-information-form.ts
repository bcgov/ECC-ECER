import EceCertificateInformation from "@/components/inputs/EceCertificateInformation.vue";
import type { Form } from "@/types/form";

const certificateInformationForm: Form = {
  id: "certificateInformationForm",
  title: "Certificate information",
  inputs: {
    certificateInformation: {
      id: "certificateInformation",
      component: EceCertificateInformation,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default certificateInformationForm;
