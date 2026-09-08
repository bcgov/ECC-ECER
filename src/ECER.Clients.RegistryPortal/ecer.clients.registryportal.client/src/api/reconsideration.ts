import { getClient } from "@/api/client";
import type { Components, Paths } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getReconsiderationsQuery = async (
  id?: string,
  statuses?: Components.Schemas.ReconsiderationStatusCode[],
): Promise<ApiResponse<Components.Schemas.Reconsideration[]>> => {
  const queryParameters: Paths.ReconsiderationsGet.QueryParameters = {
    ById: id,
    "ByStatusCodes[]": statuses,
  };
  const client = await getClient();

  return apiResultHandler.execute<Components.Schemas.Reconsideration[]>({
    request: client.reconsiderations_get(queryParameters),
    key: "reconsiderations_get",
  });
};

const submitReconsideration = async (
  reconsideration: Components.Schemas.Reconsideration,
): Promise<ApiResponse<string>> => {
  const client = await getClient();

  return apiResultHandler.execute<string>({
    request: client.reconsiderations_submit_put(
      {
        id: reconsideration.id ?? "",
      },
      reconsideration,
    ),
    key: "reconsiderations_submit_put",
  });
};

export { getReconsiderationsQuery, submitReconsideration };
