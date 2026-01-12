import type { Form } from "@/types/form";
import type { Components } from "@/types/openapi";
import EceProgramAreaInput from "@/components/inputs/EceProgramAreaInput.vue";
import { ProgramType } from "@/utils/constant";

const infantAndToddlerForm: Form = {
  id: "infantAndToddlerForm",
  title: "Infant and Toddler Educator",
  components: {
    infantAndToddler: {
      id: "isInfantAndToddlerOffered",
      component: EceProgramAreaInput,
      props: {
        programType: ProgramType.ITE,
      },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
      getValue: (data) => (data.draftApplication as Components.Schemas.Program)?.programTypes?.includes(ProgramType.ITE),
    },
  },
};

export default infantAndToddlerForm;
