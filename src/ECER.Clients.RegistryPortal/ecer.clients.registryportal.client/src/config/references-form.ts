import EceWorkExperienceReferences from "@/components/inputs/EceWorkExperienceReferences.vue";
import type { Form } from "@/types/form";

import { useApplicationStore } from "@/store/application";
const applicationStore = useApplicationStore();

const referencesForm: Form = {
  id: "referencesForm",
  title: "Work experience references",
  inputs: {
    referenceList: {
      id: "referenceList",
      component: EceWorkExperienceReferences,
      props: {
        isRenewal: applicationStore.isDraftApplicationRenewal,
      },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default referencesForm;
