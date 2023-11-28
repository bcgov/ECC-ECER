import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";

const getApplications = async (): Promise<
  Components.Schemas.Application[] | null | undefined
> => {
  const client = await getClient();
  return (await client.GetApplications()).data.items;
};

const postApplication = async (): Promise<any> => {
  const client = await getClient();
  await client.PostNewApplication({}, {});
};

export { getApplications, postApplication };
