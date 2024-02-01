import axios from "axios";

import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";

const getUserInfo = async (): Promise<Components.Schemas.UserInfo | null> => {
  try {
    const client = await getClient();
    const response = await client.userinfo_get();

    return response?.data ?? null;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 404) {
        console.log("User info not found:", error);
      } else {
        console.log("Error fetching user info:", error);
      }
    }
    return null;
  }
};

const postUserInfo = async (user: Components.Schemas.UserInfo): Promise<boolean> => {
  try {
    const client = await getClient();
    const response = await client.userinfo_post({}, user);
    return response.status === 200;
  } catch (error) {
    console.log("Error creating user info:", error);
    return false;
  }
};

export { getUserInfo, postUserInfo };
