import EceWorkExperienceReferences from "@/components/inputs/EceWorkExperienceReferences.vue";
import type { Form } from "@/types/form";

const referencesForm: Form = {
  id: "referencesForm",
  title: "Work experience reference",
  inputs: {
    referenceList: {
      id: "referenceList",
      component: EceWorkExperienceReferences,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default referencesForm;
