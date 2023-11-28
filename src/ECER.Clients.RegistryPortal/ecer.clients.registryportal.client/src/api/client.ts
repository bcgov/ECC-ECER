import OpenAPIClientAxios from "openapi-client-axios";

import type { Client } from "@/types/openapi";

export const getClient = async () => {
  const api = new OpenAPIClientAxios({
    definition: "http://localhost:5121/swagger/v1/swagger.json",
  });

  return await api.init<Client>();
};
