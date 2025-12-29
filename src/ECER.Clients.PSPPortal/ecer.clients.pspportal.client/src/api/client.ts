import OpenAPIClientAxios from "openapi-client-axios";
import { useOidcStore } from "@/store/oidc";
import type { Client } from "@/types/openapi";

// Cache for initialized client
let cachedClient: Client | null = null;

export const getClient = async (appendToken: boolean = true) => {
  // Initialize the client if not already cached
  if (!cachedClient) {
    const api = new OpenAPIClientAxios({
      definition: "/swagger/v1/swagger.json",
      axiosConfigDefaults: {
        paramsSerializer: {
          indexes: null // This sends arrays in a format .net is expecting
        }
      }
    });
    cachedClient = await api.init<Client>();
  }

  if (appendToken) {
    const oidcStore = useOidcStore();

    // Add a request interceptor to append the access token before every request
    cachedClient.interceptors.request.use(async (config) => {
      const user = await oidcStore.getUser();
      const access_token = user?.access_token ?? "";

      if (access_token) {
        config.headers.Authorization = `Bearer ${access_token}`;
      }

      return config;
    });

    // Add a response interceptor to handle 401 responses
    cachedClient.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response && error.response.status === 401) {
          oidcStore.logout();
        } else {
          return Promise.reject(error);
        }
      },
    );
  }

  return cachedClient;
};
