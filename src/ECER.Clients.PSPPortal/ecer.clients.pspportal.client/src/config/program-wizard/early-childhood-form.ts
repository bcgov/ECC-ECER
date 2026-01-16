import type { Form } from "@/types/form";
import type { Components } from "@/types/openapi";
import { ProgramType } from "@/utils/constant";
import EceProgramAreaInput from "@/components/inputs/EceProgramAreaInput.vue";

const earlyChildhoodForm: Form = {
  id: "earlyChildhoodForm",
  title: "Basic Early Childhood Educator",
  components: {
    earlyChildhood: {
      id: "isEarlyChildhoodEducatorOffered",
      component: EceProgramAreaInput,
      props: {
        programType: ProgramType.BASIC,
      },
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
      getValue: (data) =>
        (
          data.draftApplication as Components.Schemas.Program
        )?.programTypes?.includes(ProgramType.BASIC),
    },
  },
};

export default earlyChildhoodForm;
