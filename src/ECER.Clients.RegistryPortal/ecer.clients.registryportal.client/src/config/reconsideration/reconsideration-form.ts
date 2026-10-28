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
      getValue: (
        dataSource,
      ):
        | Components.Schemas.Reconsideration
        | Components.Schemas.InvestigationReconsideration => {
        // since we can accept both types we need this function to help determine what we have.
        // only InvestigationReconsideration has immediateActionType and investigationOutcome fields.
        function isInvestigationReconsideration(
          value:
            | Components.Schemas.Reconsideration
            | Components.Schemas.InvestigationReconsideration
            | undefined,
        ): value is Components.Schemas.InvestigationReconsideration {
          return (
            value?.type === "ImmediateAction" ||
            value?.type === "InvestigationOutcome"
          );
        }

        const reconsideration = dataSource.reconsideration;
        const investigationReconsideration = isInvestigationReconsideration(
          reconsideration,
        )
          ? reconsideration
          : undefined;

        return {
          explanationAndEvidence: undefined,
          files: [],
          reconsiderationEndDate:
            dataSource.reconsideration?.reconsiderationEndDate,
          type: dataSource.reconsideration?.type,
          immediateActionType:
            investigationReconsideration?.immediateActionType,
          investigationOutcome:
            investigationReconsideration?.investigationOutcome,
        };
      },
    },
  },
};

export default reconsiderationForm;
