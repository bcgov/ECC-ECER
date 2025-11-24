import EceIcraEligibilityWorkExperienceContact from "@/components/reference/inputs/icra-eligibility/EceIcraEligibilityWorkExperienceContact.vue";
import type { Form } from "@/types/form";

const icraEligibilityWorkExperienceContactInformationForm: Form = {
  id: "icraEligiblityWorkExperienceContactInformationForm",
  title: "contact information form",
  inputs: {
    icraEligibilityWorkExperienceContactInformation: {
      id: "icraEligibilityWorkExperienceContactInformation",
      component: EceIcraEligibilityWorkExperienceContact,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default icraEligibilityWorkExperienceContactInformationForm;
