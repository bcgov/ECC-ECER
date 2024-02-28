import { getClient } from "@/api/client";
import type { Components, Paths } from "@/types/openapi";
import ApiResultHandler from "@/utils/apiResultHandler";

const getApplications = async (): Promise<Components.Schemas.Application[] | null | undefined> => {
  const client = await getClient();
  return (await client.application_get()).data;
};

const apiResultHandler = new ApiResultHandler();

const createOrUpdateDraftApplication = async (application: Components.Schemas.DraftApplication): Promise<string | null | undefined> => {
  try {
    const client = await getClient();
    const body: Paths.DraftapplicationPut.RequestBody = {
      draftApplication: application,
    };
    const pathParameters: Paths.DraftapplicationPut.PathParameters = {
      id: application.id || "",
    };
    const response = await client.draftapplication_put(pathParameters, body);
    return apiResultHandler.handleSuccess(response);
  } catch (error) {
    await apiResultHandler.handleError(error);
  }
};

export { createOrUpdateDraftApplication, getApplications };
