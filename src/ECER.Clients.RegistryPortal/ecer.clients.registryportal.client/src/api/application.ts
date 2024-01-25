import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";

const getApplications = async (): Promise<Components.Schemas.Application[] | null | undefined> => {
  const client = await getClient();
  return (await client.application_get()).data.items;
};

const postApplication = async (applicationId: string): Promise<string | null | undefined> => {
  const client = await getClient();
  return (await client.application_post(applicationId, {})).data.applicationId;
};

const createDraftApplication = async (certificationTypes: Components.Schemas.CertificationType[]): Promise<string | null | undefined> => {
  const client = await getClient();

  const draftApplication: Components.Schemas.DraftApplication = {
    certificationTypes: certificationTypes,
  };

  return (await client.draftapplication_put("", draftApplication)).data.applicationId;
};

export { createDraftApplication, getApplications, postApplication };
