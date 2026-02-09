import EceIcraWorkExperienceContact from "@/components/reference/inputs/icra-eligibility/EceIcraWorkExperienceContact.vue";
import type { Form } from "@/types/form";

const icraWorkExperienceContactInformationForm: Form = {
  id: "icraWorkExperienceContactInformationForm",
  title: "contact information form",
  inputs: {
    icraEligibilityWorkExperienceContactInformation: {
      id: "icraEligibilityWorkExperienceContactInformation",
      component: EceIcraWorkExperienceContact,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default icraWorkExperienceContactInformationForm;
