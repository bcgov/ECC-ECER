import { getClient } from "@/api/client";
import type { Components, Paths } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getReference = async (token: string): Promise<ApiResponse<Components.Schemas.PortalInvitationQueryResult | null | undefined>> => {
  const client = await getClient();

  const pathParameters = {
    token: token,
  };

  return apiResultHandler.execute<Components.Schemas.PortalInvitationQueryResult | null | undefined>(client.references_get(pathParameters));
};

const optOutReference = async (
  token: string,
  optOutReason: Components.Schemas.UnabletoProvideReferenceReasons,
  recaptchaToken: string,
): Promise<ApiResponse<any>> => {
  const client = await getClient();
  const body: Components.Schemas.OptOutReferenceRequest = {
    token: token,
    unabletoProvideReferenceReasons: optOutReason,
    recaptchaToken: recaptchaToken,
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

const resendCharacterReference = async (params: Paths.ApplicationCharacterReferenceResendInvitePost.PathParameters): Promise<ApiResponse<any>> => {
  const client = await getClient();
  return apiResultHandler.execute(client.application_character_reference_resend_invite_post(params), "application_character_reference_resend_invite_post");
};

const resendWorkExperienceReference = async (params: Paths.ApplicationCharacterReferenceResendInvitePost.PathParameters): Promise<ApiResponse<any>> => {
  const client = await getClient();
  return apiResultHandler.execute(
    client.application_work_experience_reference_resend_invite_post(params),
    "application_work_experience_reference_resend_invite_post",
  );
};

const upsertWorkExperienceReference = async (
  params: Paths.ApplicationWorkexperiencereferenceUpdatePost.PathParameters,
  body: Paths.ApplicationWorkexperiencereferenceUpdatePost.RequestBody,
): Promise<ApiResponse<any>> => {
  const client = await getClient();
  return apiResultHandler.execute(client.application_workexperiencereference_update_post(params, body), "application_workexperiencereference_update_post");
};

const upsertCharacterReference = async (
  params: Paths.ApplicationCharacterreferenceUpdatePost.PathParameters,
  body: Paths.ApplicationCharacterreferenceUpdatePost.RequestBody,
): Promise<ApiResponse<any>> => {
  const client = await getClient();
  return apiResultHandler.execute(client.application_characterreference_update_post(params, body), "application_characterreference_update_post");
};

export {
  getReference,
  optOutReference,
  postCharacterReference,
  postWorkExperienceReference,
  resendCharacterReference,
  resendWorkExperienceReference,
  upsertCharacterReference,
  upsertWorkExperienceReference,
};
