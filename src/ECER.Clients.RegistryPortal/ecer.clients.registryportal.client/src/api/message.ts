import type { AxiosRequestConfig } from "axios";

import { getClient } from "@/api/client";
import type { Components, Paths } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getMessagesStatus = async (): Promise<ApiResponse<Components.Schemas.CommunicationsStatusResults | null>> => {
  const client = await getClient();
  return apiResultHandler.execute<Components.Schemas.CommunicationsStatusResults>(client.message_status_get());
};

const getMessages = async (params: { page: number; pageSize: number }): Promise<ApiResponse<Components.Schemas.GetMessagesResponse | null>> => {
  const client = await getClient();

  const config: AxiosRequestConfig = {
    params: params,
  };

  return apiResultHandler.execute<Components.Schemas.GetMessagesResponse | null>(client.message_get(null, null, config));
};

const getChildMessages = async (params: Paths.MessageGet.PathParameters): Promise<ApiResponse<any>> => {
  const client = await getClient();
  return apiResultHandler.execute(client.message_get(params));
};

const markMessageAsRead = async (messageId: string): Promise<ApiResponse<Components.Schemas.CommunicationResponse>> => {
  const client = await getClient();
  return apiResultHandler.execute<Components.Schemas.CommunicationResponse>(
    client.communication_put(
      {
        id: messageId,
      },
      {
        communicationId: messageId,
      },
    ),
  );
};

export { getChildMessages, getMessages, getMessagesStatus, markMessageAsRead };
