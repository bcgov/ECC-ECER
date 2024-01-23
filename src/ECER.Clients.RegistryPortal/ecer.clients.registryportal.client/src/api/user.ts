import axios from "axios";

import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";

const getUserInfo = async (): Promise<Components.Schemas.UserProfile | null> => {
  try {
    const client = await getClient();
    const response = await client.GetUserInfo();

    // Check if the response has data and userInfo property
    return response?.data?.userInfo ?? null;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 404) {
        console.log("User not found:", error);
      } else {
        console.log("Error fetching user info:", error);
      }
    }
    return null;
  }
};

const createUser = async (user: Components.Schemas.NewUserRequest): Promise<boolean> => {
  try {
    const client = await getClient();
    const response = await client.CreateUserInfo({}, user);
    return response.status === 200;
  } catch (error) {
    console.log("Error creating user:", error);
    return false;
  }
};

export { createUser, getUserInfo };
