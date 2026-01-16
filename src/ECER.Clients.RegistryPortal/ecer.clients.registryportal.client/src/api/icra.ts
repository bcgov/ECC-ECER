import { getClient } from "@/api/client";
import type { Components, Paths } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getIcraEligibilities = async (): Promise<ApiResponse<Components.Schemas.ICRAEligibility[] | null | undefined>> => {
  const client = await getClient();

  return apiResultHandler.execute<Components.Schemas.ICRAEligibility[] | null | undefined>({
    request: client.icra_get({ id: "" }),
    key: "icra_get",
  });
};

const createOrUpdateDraftIcraEligibility = async (
  icraEligibility: Components.Schemas.ICRAEligibility,
): Promise<ApiResponse<Components.Schemas.DraftICRAEligibilityResponse | null | undefined>> => {
  const client = await getClient();
  const body: Paths.IcraPut.RequestBody = {
    eligibility: icraEligibility,
  };
  const pathParameters: Paths.IcraPut.PathParameters = {
    id: icraEligibility.id || "",
  };

  return apiResultHandler.execute<Components.Schemas.DraftICRAEligibilityResponse | null | undefined>({
    request: client.icra_put(pathParameters, body),
    key: "icra_put",
  });
};

const getIcraEligibilityStatus = async (eligibilityId: string): Promise<ApiResponse<Components.Schemas.ICRAEligibilityStatus | null | undefined>> => {
  const client = await getClient();
  const pathParameters = {
    id: eligibilityId,
  };

  return await apiResultHandler.execute<Components.Schemas.ICRAEligibilityStatus | null | undefined>({
    request: client.icra_status_get(pathParameters),
    key: "icra_status_get",
  });
};

const submitIcraEligibilityApplication = async (
  eligibilityId: string,
): Promise<ApiResponse<Components.Schemas.SubmitICRAEligibilityResponse | null | undefined>> => {
  const client = await getClient();
  const body: Components.Schemas.ICRAEligibilitySubmissionRequest = {
    id: eligibilityId,
  };

  return apiResultHandler.execute<Components.Schemas.SubmitICRAEligibilityResponse | null | undefined>({
    request: client.icra_post(null, body),
    key: "icra_post",
  });
};

const addIcraEligibilityWorkExperienceReference = async (
  icraEligibilityId: string,
  workExperienceReference: Components.Schemas.EmploymentReference,
): Promise<ApiResponse<Components.Schemas.EmploymentReference | null | undefined>> => {
  const pathParameters = {
    icraEligibilityId: icraEligibilityId,
  };
  const client = await getClient();

  return apiResultHandler.execute<Components.Schemas.EmploymentReference | null | undefined>({
    request: client.icra_work_reference_add_post(pathParameters, workExperienceReference),
    key: "icra_work_reference_add_post",
  });
};

const replaceIcraEligibilityWorkExperienceReference = async (
  icraEligibilityId: string,
  referenceId: string,
  workExperienceReference: Components.Schemas.EmploymentReference,
): Promise<ApiResponse<Components.Schemas.EmploymentReference | null | undefined>> => {
  const pathParameters = {
    icraEligibilityId: icraEligibilityId,
    referenceId: referenceId,
  };
  const client = await getClient();

  return apiResultHandler.execute<Components.Schemas.EmploymentReference | null | undefined>({
    request: client.icra_work_reference_replace_post(pathParameters, workExperienceReference),
    key: "icra_work_reference_replace_post",
  });
};

const getIcraWorkExperienceReferenceById = async (
  workExperienceReferenceId: string,
): Promise<ApiResponse<Components.Schemas.EmploymentReference | null | undefined>> => {
  const client = await getClient();
  const pathParameters = {
    workExperienceReferenceId,
  };

  return apiResultHandler.execute<Components.Schemas.EmploymentReference | null | undefined>({
    request: client.icra_work_reference_by_id_get(pathParameters),
    key: "icra_work_reference_by_id_get",
  });
};

const resendIcraEligibilityWorkExperienceReferenceInvite = async (
  icraEligibilityId: string,
  referenceId: string,
): Promise<ApiResponse<Components.Schemas.ResendIcraReferenceInviteResponse | null | undefined>> => {
  const pathParameters = {
    icraEligibilityId,
    referenceId,
  };
  const client = await getClient();

  return apiResultHandler.execute<Components.Schemas.ResendIcraReferenceInviteResponse | null | undefined>({
    request: client.icra_work_reference_resend_invite_post(pathParameters),
    key: "icra_work_reference_resend_invite_post",
  });
};

export {
  addIcraEligibilityWorkExperienceReference,
  createOrUpdateDraftIcraEligibility,
  getIcraEligibilities,
  getIcraEligibilityStatus,
  getIcraWorkExperienceReferenceById,
  replaceIcraEligibilityWorkExperienceReference,
  submitIcraEligibilityApplication,
  resendIcraEligibilityWorkExperienceReferenceInvite,
};
