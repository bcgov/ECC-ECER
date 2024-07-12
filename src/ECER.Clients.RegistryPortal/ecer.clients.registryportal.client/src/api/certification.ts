import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getCertifications = async (): Promise<ApiResponse<Components.Schemas.Certification[] | null>> => {
  const client = await getClient();
  return apiResultHandler.execute<Components.Schemas.Certification[]>({ request: client.certification_get({ id: "" }) });
};

export { getCertifications };
