import EceProfessionalDevelopment from "@/components/inputs/EceProfessionalDevelopment.vue";
import type { Form } from "@/types/form";

const professionalDevelopment: Form = {
  id: "professionalDevelopmentForm",
  title: "Professional Development",
  inputs: {
    professionalDevelopments: {
      id: "professionalDevelopments",
      component: EceProfessionalDevelopment,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default professionalDevelopment;
