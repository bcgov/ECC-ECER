import { getClient } from "@/api/client";
import type { RegisterPspUserRequest, PspUserProfile } from "@/types/openapi";
import ApiResultHandler from "@/utils/apiResultHandler";
const apiResultHandler = new ApiResultHandler();

const getPspUserProfile = async (): Promise<PspUserProfile | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({ request: client.psp_user_profile_get(), key: "psp_user_profile_get", suppressErrorToast: true });
  return response?.data ?? null;
};

const registerPspUser = async (registerPspUserRequest: RegisterPspUserRequest): Promise<PspUserProfile | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({ request: client.psp_user_register_post({}, registerPspUserRequest), key: "psp_user_register_post" });
  return response?.data ?? null;
};

export { getPspUserProfile, registerPspUser };
