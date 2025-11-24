import { getClient } from "@/api/client";
import type { PspUserListItem } from "@/types/openapi";
import ApiResultHandler from "@/utils/apiResultHandler";
const apiResultHandler = new ApiResultHandler();

const getUsers = async (): Promise<PspUserListItem[] | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({ request: client.psp_user_manage_get(), key: "psp_user_manage_get", suppressErrorToast: true });
  return response?.data ?? null;
};

export { getUsers };
