import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getReference = async (token: string): Promise<ApiResponse<Components.Schemas.ReferenceQueryResult | null | undefined>> => {
  const client = await getClient();

  const pathParameters = {
    token: token,
  };

  return apiResultHandler.execute<Components.Schemas.ReferenceQueryResult | null | undefined>(client.references_get(pathParameters));
};

export { getReference };
