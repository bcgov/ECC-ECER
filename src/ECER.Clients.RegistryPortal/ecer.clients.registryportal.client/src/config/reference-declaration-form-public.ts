import EceDeclaration from "@/components/reference/inputs/EceDeclaration.vue";
import type { Form } from "@/types/form";

const referencePublicForm: Form = {
  id: "declarationForm",
  title: "Declaration Reference",
  inputs: {
    declarationForm: {
      id: "declaration",
      component: EceDeclaration,
      props: {},
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default referencePublicForm;
