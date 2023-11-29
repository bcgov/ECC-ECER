import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";

const getApplications = async (): Promise<
  Components.Schemas.Application[] | null | undefined
> => {
  const client = await getClient();
  return (await client.GetApplications()).data.items;
};

const postApplication = async (): Promise<string | null | undefined> => {
  const client = await getClient();
  return (await client.PostNewApplication({}, {})).data.applicationId;
};

export { getApplications, postApplication };
