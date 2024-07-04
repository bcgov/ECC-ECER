import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getMessagesStatus = async (): Promise<ApiResponse<Components.Schemas.CommunicationsStatusResults | null>> => {
  const client = await getClient();
  return apiResultHandler.execute<Components.Schemas.CommunicationsStatusResults>(client.message_status_get());
};

const getMessages = async (): Promise<ApiResponse<Components.Schemas.Communication[] | null | undefined>> => {
  const client = await getClient();
  return apiResultHandler.execute<Components.Schemas.Communication[]>(client.message_get());
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

const sendMessage = async (sendMessageRequest: Components.Schemas.SendMessageRequest): Promise<ApiResponse<Components.Schemas.SendMessageResponse>> => {
  const client = await getClient();
  return apiResultHandler.execute<Components.Schemas.SendMessageResponse>(client.message_post(null, sendMessageRequest), "message_post");
};

export { getMessages, getMessagesStatus, markMessageAsRead, sendMessage };