import axios from "axios";

import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";

const getMessagesStatus = async (): Promise<Components.Schemas.CommunicationsStatus | null> => {
  try {
    const client = await getClient();
    const response = await client.message_status_get();
    return response?.data.status ?? null;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 404) {
        console.log("User not found:", error);
      } else {
        console.log("Error fetching user communications status:", error);
      }
    }
    return null;
  }
};

const getMessages = async (): Promise<Components.Schemas.Communication[] | null | undefined> => {
  const client = await getClient();
  return (await client.message_get()).data;
};

export { getMessages, getMessagesStatus };
