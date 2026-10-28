import { getClient } from "@/api/client";
import type { Components, Paths } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getInvestigationReconsiderationsQuery = async (
  id?: string,
  statuses?: Components.Schemas.InvestigationReconsiderationStatusCode[],
): Promise<ApiResponse<Components.Schemas.InvestigationReconsideration[]>> => {
  const queryParameters: Paths.InvestigationReconsiderationsGet.QueryParameters =
    {
      ById: id,
      "ByStatusCodes[]": statuses,
    };
  const client = await getClient();

  return apiResultHandler.execute<
    Components.Schemas.InvestigationReconsideration[]
  >({
    request: client.investigation_reconsiderations_get(queryParameters),
    key: "investigation_reconsiderations_get",
  });
};

const submitInvestigationReconsideration = async (
  investigationReconsideration: Components.Schemas.InvestigationReconsideration,
): Promise<ApiResponse<string>> => {
  const client = await getClient();

  return apiResultHandler.execute<string>({
    request: client.investigation_reconsiderations_submit_put(
      {
        id: investigationReconsideration.id ?? "",
      },
      investigationReconsideration,
    ),
    key: "investigation_reconsiderations_submit_put",
  });
};

const mapInvestigationReconsiderationTypeToTypeOfDecision = (
  reconsideration: Components.Schemas.InvestigationReconsideration,
) => {
  const reconsiderationType = reconsideration.type;
  switch (reconsiderationType) {
    case "ImmediateAction":
      switch (reconsideration.immediateActionType) {
        case "YesSuspended":
          return "Suspend certificate";
        case "YesTC":
          return "Attach or vary terms and conditions";
        default:
          console.warn(
            `mapInvestigationReconsiderationTypeToTypeOfDecision: unhandled immediate action type: ${reconsideration.immediateActionType}`,
          );
          return reconsideration.immediateActionType;
      }
    case "InvestigationOutcome":
      switch (reconsideration.investigationOutcome) {
        case "ActionApplicationDenied":
          return "Intent to deny application or reject renewal application";
        case "ActionCancellation":
          return "Intent to cancel certificate";
        case "ActionSuspension":
          return "Intent to suspend certificate";
        case "ActionTermsandConditions":
          return "Intent to attach or vary terms and conditions";
        default:
          console.warn(
            `mapInvestigationReconsiderationTypeToTypeOfDecision: unhandled investigation outcome type: ${reconsideration.investigationOutcome}`,
          );
          return reconsideration.investigationOutcome;
      }
    default:
      console.warn(
        `mapInvestigationReconsiderationTypeToTypeOfDecision: unhandled reconsideration type: ${reconsiderationType}`,
      );
      return reconsiderationType;
  }
};

export {
  getInvestigationReconsiderationsQuery,
  submitInvestigationReconsideration,
  mapInvestigationReconsiderationTypeToTypeOfDecision,
};
