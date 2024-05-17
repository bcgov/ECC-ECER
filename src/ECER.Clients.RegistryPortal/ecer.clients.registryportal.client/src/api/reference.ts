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

const postCharacterReference = async (characterReferenceSubmission: Components.Schemas.CharacterReferenceSubmissionRequest): Promise<ApiResponse<any>> => {
  const client = await getClient();
  return apiResultHandler.execute(client.character_reference_post(null, characterReferenceSubmission), "character_reference_post");
};

const postWorkExperienceReference = async (
  workExperienceReferenceSubmission: Components.Schemas.WorkExperienceReferenceSubmissionRequest,
): Promise<ApiResponse<any>> => {
  const client = await getClient();
  return apiResultHandler.execute(client.workExperience_reference_post(null, workExperienceReferenceSubmission), "workExperience_reference_post");
};

export { getReference, optOutReference, postCharacterReference, postWorkExperienceReference };
