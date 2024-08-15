import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";

const professionalDevelopment: Form = {
  id: "professionalDevelopmentForm",
  title: "Professional Development",
  inputs: {
    professionalDevelopment: {
      id: "professionalDevelopment",
      component: EceTextField,
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