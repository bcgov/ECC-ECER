import { getClient } from "@/api/client";
import type { Application } from "@/store/application";
import type { Components, Paths } from "@/types/openapi";

const getApplications = async (): Promise<Components.Schemas.Application[] | null | undefined> => {
  const client = await getClient();
  return (await client.application_get()).data;
};

const createDraftApplication = async (certificationTypes: Components.Schemas.CertificationType[]): Promise<string | null | undefined> => {
  const client = await getClient();

  const body: Paths.DraftapplicationPut.RequestBody = {
    draftApplication: {
      id: "",
      certificationTypes: certificationTypes,
    },
  };
  const pathParameters: Paths.DraftapplicationPut.PathParameters = {
    id: "",
  };

  return (await client.draftapplication_put(pathParameters, body)).data.applicationId;
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

export { createDraftApplication, createOrUpdateDraftApplication, getApplications };
