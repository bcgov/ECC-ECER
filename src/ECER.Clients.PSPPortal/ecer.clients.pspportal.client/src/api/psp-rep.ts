import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";
import ApiResultHandler from "@/utils/apiResultHandler";
const apiResultHandler = new ApiResultHandler();

const getPspUserProfile = async (): Promise<Components.Schemas.PspUserProfile | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({ request: client.psp_user_profile_get(), key: "psp_user_profile_get", suppressErrorToast: true });
  return response?.data ?? null;
};

const postPspUserProfile = async (pspUserProfile: Components.Schemas.PspUserProfile): Promise<Components.Schemas.PspUserProfile | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({ request: client.psp_user_profile_post({}, pspUserProfile), key: "psp_user_profile_post" });
  return response?.data ?? null;
};

export { getPspUserProfile, postPspUserProfile };
