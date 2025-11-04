import EceIcraEligibilityWorkReferenceDeclaration from "@/components/reference/inputs/icra-eligibility/EceIcraEligibilityWorkExperienceDeclaration.vue";
import type { Form } from "@/types/form";

const IcraEligibilityWorkReferenceDeclarationForm: Form = {
  id: "declarationForm",
  title: "Declaration Reference",
  inputs: {
    willProvideReference: {
      id: "willProvideReference",
      component: EceIcraEligibilityWorkReferenceDeclaration,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default IcraEligibilityWorkReferenceDeclarationForm;
