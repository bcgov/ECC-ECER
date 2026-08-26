import { getClient } from "@/api/client";
import type { Components, Paths } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getReconsiderationsQuery = async (
  id?: string,
  status?: Components.Schemas.ReconsiderationStatusCode[],
): Promise<ApiResponse<Components.Schemas.Reconsideration[]>> => {
  const queryParameters: Paths.ReconsiderationsGet.QueryParameters = {
    ById: id,
    "ByStatusCodes[]": status,
  };
  const client = await getClient();

  return apiResultHandler.execute<Components.Schemas.Reconsideration[]>({
    request: client.reconsiderations_get(queryParameters),
    key: "reconsiderations_get",
  });
};

export { getReconsiderationsQuery };
