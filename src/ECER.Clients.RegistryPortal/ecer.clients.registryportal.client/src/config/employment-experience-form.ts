import EceIcraWorkExperienceEligibility from "@/components/inputs/EceIcraWorkExperienceEligibility.vue";
import type { Form } from "@/types/form";

const employmentExperienceForm: Form = {
  id: "employmentExperienceForm",
  title: "Employment experience",
  inputs: {
    employmentExperienceList: {
      id: "employmentExperienceList",
      component: EceIcraWorkExperienceEligibility,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default employmentExperienceForm;
