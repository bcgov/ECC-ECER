import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getProfile = async (): Promise<Components.Schemas.UserProfile | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({ request: client.profile_get(), key: "profile_get" });
  return response?.data ?? null;
};

const putProfile = async (user: Components.Schemas.UserProfile): Promise<ApiResponse<any>> => {
  const client = await getClient();
  return apiResultHandler.execute({ request: client.profile_put({}, user), key: "profile_put" });
};

export { getProfile, putProfile };
