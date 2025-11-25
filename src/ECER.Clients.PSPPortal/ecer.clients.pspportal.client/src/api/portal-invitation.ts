import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getPortalInvitation = async (token: string): Promise<ApiResponse<Components.Schemas.PortalInvitationQueryResult | null | undefined>> => {
  const client = await getClient();

  const pathParameters = {
    token: token,
  };

  return apiResultHandler.execute<Components.Schemas.PortalInvitationQueryResult | null | undefined>({
    request: client.portal_invitation_get(pathParameters),
    suppressErrorToast: true,
  });
};

export { getPortalInvitation };
