import EceEducation from "@/components/inputs/EceEducation.vue";
import type { Form } from "@/types/form";

const educationForm: Form = {
  id: "education-form",
  title: "Education",
  inputs: {
    educationList: {
      id: "educationList",
      component: EceEducation,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default educationForm;
