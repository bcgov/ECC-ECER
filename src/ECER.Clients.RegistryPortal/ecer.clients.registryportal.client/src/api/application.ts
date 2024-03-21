import { getClient } from "@/api/client";
import type { Components, Paths } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getApplications = async (): Promise<ApiResponse<Components.Schemas.Application[] | null | undefined>> => {
  const client = await getClient();

  return apiResultHandler.execute<Components.Schemas.Application[] | null | undefined>(client.application_get());
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

  return apiResultHandler.execute<Components.Schemas.SubmitApplicationResponse | null | undefined>(client.application_post(null, body));
};

export { createOrUpdateDraftApplication, getApplications, submitDraftApplication };
