import { getClient } from "@/api/client";
import type { Application } from "@/store/application";
import type { Components, Paths } from "@/types/openapi";

const getApplications = async (): Promise<Components.Schemas.Application[] | null | undefined> => {
  const client = await getClient();
  return (await client.application_get()).data;
};

const createOrUpdateDraftApplication = async (application: Application): Promise<string | null | undefined> => {
  const client = await getClient();

  const body: Paths.DraftapplicationPut.RequestBody = {
    draftApplication: application,
  };
  const pathParameters: Paths.DraftapplicationPut.PathParameters = {
    id: application.Id || "",
  };

  return (await client.draftapplication_put(pathParameters, body)).data.applicationId;
};

export { createOrUpdateDraftApplication, getApplications };
