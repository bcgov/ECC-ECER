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
    },
  },
};

export default certificationTypeForm;
