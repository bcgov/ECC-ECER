import EceIcraApplicationWorkReferenceDeclaration from "@/components/reference/inputs/icra-eligibility/EceIcraApplicationWorkExperienceDeclaration.vue";
import type { Form } from "@/types/form";

const IcraApplicationWorkReferenceDeclarationForm: Form = {
  id: "declarationForm",
  title: "Declaration Reference",
  inputs: {
    willProvideReference: {
      id: "willProvideReference",
      component: EceIcraApplicationWorkReferenceDeclaration,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default IcraApplicationWorkReferenceDeclarationForm;
