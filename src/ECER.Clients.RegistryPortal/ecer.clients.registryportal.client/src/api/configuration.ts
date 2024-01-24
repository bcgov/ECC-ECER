import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";

const getConfiguration = async (): Promise<Components.Schemas.ApplicationConfiguration | null | undefined> => {
  const client = await getClient(false);
  return (await client.configuration_get()).data;
};

export { getConfiguration };
