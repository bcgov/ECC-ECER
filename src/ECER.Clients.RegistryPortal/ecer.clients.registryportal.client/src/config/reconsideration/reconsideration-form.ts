import EceReconsideration from "@/components/reconsideration/inputs/EceReconsideration.vue";
import type { Form } from "@/types/form";
import type { Components } from "@/types/openapi";

const reconsiderationForm: Form = {
  id: "reconsiderationForm",
  title: "Dispute details",
  inputs: {
    reconsideration: {
      id: "reconsideration",
      component: EceReconsideration,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
      getValue: (dataSource): Components.Schemas.Reconsideration => {
        return {
          explanationAndEvidence: undefined,
          files: [],
          reconsiderationEndDate:
            dataSource.reconsideration?.reconsiderationEndDate,
        };
      },
    },
  },
};

export default reconsiderationForm;
