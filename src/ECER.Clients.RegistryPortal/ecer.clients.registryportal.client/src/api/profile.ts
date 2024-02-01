import axios from "axios";

import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";

const getProfile = async (): Promise<Components.Schemas.UserProfile | null> => {
  try {
    const client = await getClient();
    const response = await client.profile_get();

    return response?.data ?? null;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 404) {
        console.log("Profile not found:", error);
      } else {
        console.log("Error fetching profile:", error);
      }
    }
    return null;
  }
};

const putProfile = async (user: Components.Schemas.UserProfile): Promise<boolean> => {
  try {
    const client = await getClient();
    const response = await client.profile_put({}, user);
    return response.status === 200;
  } catch (error) {
    console.log("Error creating profile:", error);
    return false;
  }
};

export { getProfile, putProfile };
