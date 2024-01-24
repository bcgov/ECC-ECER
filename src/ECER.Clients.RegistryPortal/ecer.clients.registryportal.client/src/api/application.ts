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

export { getApplications, postApplication };
