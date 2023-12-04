import OpenAPIClientAxios from "openapi-client-axios";
import { useUserStore } from "@/store/user";
import type { Client } from "@/types/openapi";

export const getClient = async () => {
  const api = new OpenAPIClientAxios({
    definition: `${import.meta.env.VITE_BASE_URL}/swagger/v1/swagger.json`,
  });

  const axiosClient = await api.init<Client>();
  const userStore = useUserStore();
  const access_token = await userStore.getAccessToken();

  // Add a request interceptor to append the access token to the request
  axiosClient.interceptors.request.use((config) => {
    if (access_token) {
      config.headers.Authorization = `Bearer ${access_token}`;
    } else {
      // If we've failed to get an access token from oidc-client-ts sessionStorage, clear the profile and logout
      userStore.clearProfile();
      userStore.logout();
    }
    return config;
  });

  return axiosClient;
};
