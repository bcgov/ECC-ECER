import { getClient } from "@/api/client";
import type { Components, Paths } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getApplications = async (): Promise<ApiResponse<Components.Schemas.Application[] | null | undefined>> => {
  const client = await getClient();

  return apiResultHandler.execute<Components.Schemas.Application[] | null | undefined>(client.application_get({ id: "" }));
};

const createOrUpdateDraftApplication = async (
  application: Components.Schemas.DraftApplication,
): Promise<ApiResponse<Components.Schemas.DraftApplicationResponse | null | undefined>> => {
  const client = await getClient();
  const body: Paths.DraftapplicationPut.RequestBody = {
    draftApplication: application,
  };
  const pathParameters: Paths.DraftapplicationPut.PathParameters = {
    id: application.id || "",
  };

  return apiResultHandler.execute<Components.Schemas.DraftApplicationResponse | null | undefined>(client.draftapplication_put(pathParameters, body));
};

const submitDraftApplication = async (applicationId: string): Promise<ApiResponse<Components.Schemas.SubmitApplicationResponse | null | undefined>> => {
  const client = await getClient();
  const body: Components.Schemas.ApplicationSubmissionRequest = {
    id: applicationId,
  };

  return apiResultHandler.execute<Components.Schemas.SubmitApplicationResponse | null | undefined>(client.application_post(null, body), "application_post");
};

const cancelDraftApplication = async (applicationId: string): Promise<ApiResponse<Components.Schemas.SubmitApplicationResponse | null | undefined>> => {
  const client = await getClient();
  const pathParameters = {
    id: applicationId,
  };

  return await apiResultHandler.execute<Components.Schemas.SubmitApplicationResponse | null | undefined>(client.draftapplication_delete(pathParameters));
};

const getApplicationStatus = async (applicationId: string): Promise<ApiResponse<Components.Schemas.SubmittedApplicationStatus | null | undefined>> => {
  const client = await getClient();
  const pathParameters = {
    id: applicationId,
  };

  return await apiResultHandler.execute<Components.Schemas.SubmittedApplicationStatus | null | undefined>(client.application_status_get(pathParameters));
};

export { cancelDraftApplication, createOrUpdateDraftApplication, getApplications, getApplicationStatus, submitDraftApplication };
