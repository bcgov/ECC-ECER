import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getReference = async (token: string): Promise<ApiResponse<Components.Schemas.PortalInvitationQueryResult | null | undefined>> => {
  const client = await getClient();

  const pathParameters = {
    token: token,
  };

  return apiResultHandler.execute<Components.Schemas.PortalInvitationQueryResult | null | undefined>(client.references_get(pathParameters));
};

const optOutReference = async (token: string, optOutReason: Components.Schemas.UnabletoProvideReferenceReasons): Promise<ApiResponse<any>> => {
  const client = await getClient();
  const body: Components.Schemas.OptOutReferenceRequest = {
    token: token,
    unabletoProvideReferenceReasons: optOutReason,
  };
  return apiResultHandler.execute(client.reference_optout(null, body), "reference_optout");
};

export { getReference, optOutReference };
