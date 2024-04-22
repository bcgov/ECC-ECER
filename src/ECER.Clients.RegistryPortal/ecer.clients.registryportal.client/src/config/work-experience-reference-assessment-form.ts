import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";

const workExperienceAssessmentForm: Form = {
  id: "assessmentForm",
  title: "Assessment form",
  inputs: {
    assessment: {
      id: "assessment",
      component: EceTextField,
      props: { label: "assessment form reference accepted this does not work" },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default workExperienceAssessmentForm;
