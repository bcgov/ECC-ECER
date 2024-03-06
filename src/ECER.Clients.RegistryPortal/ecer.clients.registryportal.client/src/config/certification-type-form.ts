import EceCertificationType from "@/components/inputs/EceCertificationType.vue";
import type { Form } from "@/types/form";

import certificationTypes from "./certification-types";

const certificationTypeForm: Form = {
  id: "certificationTypeForm",
  title: "Certification Selection",
  inputs: {
    certificationSelection: {
      id: "certificationSelection",
      component: EceCertificationType,
      props: {
        options: certificationTypes,
      },
      cols: {
        md: 12,
        lg: 8,
        xl: 8,
      },
    },
  },
};

export default certificationTypeForm;
