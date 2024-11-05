import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";
import ApiResultHandler from "@/utils/apiResultHandler";
const apiResultHandler = new ApiResultHandler();

const getUserInfo = async (): Promise<Components.Schemas.UserInfo | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({ request: client.userinfo_get(), key: "userinfo_get", suppressErrorToast: true });
  return response?.data ?? null;
};

const postUserInfo = async (user: Components.Schemas.UserInfo): Promise<boolean> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({ request: client.userinfo_post({}, user), key: "userinfo_post" });
  return response != null;
};

export { getUserInfo, postUserInfo };
