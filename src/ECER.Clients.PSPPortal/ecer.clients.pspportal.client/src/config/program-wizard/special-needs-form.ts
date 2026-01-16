import type { Form } from "@/types/form";
import type { Components } from "@/types/openapi";
import { ProgramType } from "@/utils/constant";
import EceProgramAreaInput from "@/components/inputs/EceProgramAreaInput.vue";

const specialNeedsForm: Form = {
  id: "specialNeedsForm",
  title: "Confirm and submit",
  components: {
    specialNeeds: {
      id: "isSpecialNeedsOffered",
      component: EceProgramAreaInput,
      props: {
        programType: ProgramType.SNE,
      },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
      getValue: (data) =>
        (
          data.draftApplication as Components.Schemas.Program
        )?.programTypes?.includes(ProgramType.SNE),
    },
  },
};

export default specialNeedsForm;
