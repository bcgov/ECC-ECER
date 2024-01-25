import { getClient } from "@/api/client";
import type { Components, Paths } from "@/types/openapi";

const getApplications = async (): Promise<Components.Schemas.Application[] | null | undefined> => {
  const client = await getClient();
  return (await client.application_get()).data.items;
};

const createDraftApplication = async (certificationTypes: Components.Schemas.CertificationType[]): Promise<string | null | undefined> => {
  const client = await getClient();

  const body: Paths.DraftapplicationPut.RequestBody = {
    id: "",
    certificationTypes: certificationTypes,
  };

  const pathParameters: Paths.DraftapplicationPut.PathParameters = {
    id: "",
  };

  return (await client.draftapplication_put(pathParameters, body)).data.applicationId;
};

export { createDraftApplication, getApplications };
