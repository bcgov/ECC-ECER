import OpenAPIClientAxios from "openapi-client-axios";

import { useOidcStore } from "@/store/oidc";
import type { Client } from "@/types/openapi";

export const getClient = async (appendToken: boolean = true) => {
  const api = new OpenAPIClientAxios({
    definition: "/swagger/v1/swagger.json",
  });

  const axiosClient = await api.init<Client>();

  if (appendToken) {
    const oidcStore = useOidcStore();
    const user = await oidcStore.getUser();
    const access_token = user?.access_token ?? "";

    // Add a request interceptor to append the access token to the request
    axiosClient.interceptors.request.use((config) => {
      if (access_token) {
        config.headers.Authorization = `Bearer ${access_token}`;
      }
      return config;
    });

    // Add a response interceptor to handle 401 responses
    axiosClient.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response.status === 401) {
          oidcStore.logout();
        } else {
          return Promise.reject(error);
        }
      },
    );
  }

  return axiosClient;
};
