import axios from "axios";

import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

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

const putProfile = async (user: Components.Schemas.UserProfile): Promise<ApiResponse<any>> => {
  const client = await getClient();
  return apiResultHandler.execute({ request: client.profile_put({}, user), key: "profile_put" });
};

export { getProfile, putProfile };
