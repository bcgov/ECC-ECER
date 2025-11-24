import EceEmploymentExperience from "@/components/inputs/EceEmploymentExperience.vue";
import type { Form } from "@/types/form";

const employmentExperienceForm: Form = {
  id: "employmentExperienceForm",
  title: "Employment experience",
  inputs: {
    employmentExperience: {
      id: "employmentExperience",
      component: EceEmploymentExperience,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default employmentExperienceForm;
