import EceWorkExperienceDeclaration from "@/components/reference/inputs/EceWorkExperienceReferenceDeclaration.vue";
import type { Form } from "@/types/form";

const workExpereinceDeclarationForm: Form = {
  id: "declarationForm",
  title: "Declaration Reference",
  inputs: {
    declarationForm: {
      id: "declaration",
      component: EceWorkExperienceDeclaration,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default workExpereinceDeclarationForm;
