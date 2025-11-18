import { getClient } from "@/api/client";
import type { RegisterPspUserRequest, PspRegistrationErrorResponse, PspUserProfile } from "@/types/openapi";
import ApiResultHandler from "@/utils/apiResultHandler";
const apiResultHandler = new ApiResultHandler();

const getPspUserProfile = async (): Promise<PspUserProfile | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({ request: client.psp_user_profile_get(), key: "psp_user_profile_get", suppressErrorToast: true });
  return response?.data ?? null;
};

const updatePspUserProfile = async (pspUserProfile: PspUserProfile): Promise<{} | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({
    request: client.psp_user_profile_put({}, pspUserProfile),
    key: "psp_user_profile_put",
    suppressErrorToast: true,
  });
  return response?.data ?? null;
};

const registerPspUser = async (registerPspUserRequest: RegisterPspUserRequest): Promise<{} | PspRegistrationErrorResponse> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({
    request: client.psp_user_register_post({}, registerPspUserRequest),
    key: "psp_user_register_post",
    suppressErrorToast: true,
  });
  return (response.error as PspRegistrationErrorResponse) ?? response.data ?? {};
};

export { getPspUserProfile, registerPspUser, updatePspUserProfile };
