import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";

const getUserInfo = async (): Promise<Components.Schemas.UserProfile | null> => {
  try {
    const client = await getClient();
    const response = await client.GetUserInfo();

    // Check if the response has data and userInfo property
    return response?.data?.userInfo ?? null;
  } catch (error) {
    // Handle the error
    console.log("Error fetching user info:", error);
    return null;
  }
};

const createUser = async (user: Components.Schemas.NewUserRequest): Promise<Components.Schemas.UserProfile | null> => {
  try {
    const client = await getClient();
    const response = await client.CreateUserInfo({}, user);

    // Check if the response has data and userInfo property
    return response?.data ?? null;
  } catch (error) {
    // Handle the error
    console.log("Error creating user:", error);
    return null;
  }
};

export { createUser, getUserInfo };
