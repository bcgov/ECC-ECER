import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";

const getUserInfo = async (): Promise<Components.Schemas.UserProfile | null> => {
  const client = await getClient();
  return (await client.GetUserInfo()).data?.userInfo ?? null;
};

export { getUserInfo };
