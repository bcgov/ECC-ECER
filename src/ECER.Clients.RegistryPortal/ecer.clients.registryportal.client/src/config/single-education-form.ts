import EceSingleEducationForm from "@/components/inputs/EceSingleEducationForm.vue";
import type { Form } from "@/types/form";

const singleEducationForm: Form = {
  id: "educationForm",
  title: "Education",
  inputs: {
    educationList: {
      id: "educationList",
      component: EceSingleEducationForm,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default singleEducationForm;
