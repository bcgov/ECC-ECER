import OpenAPIClientAxios from "openapi-client-axios";
import { useUserStore } from "@/store/user";
import type { Client } from "@/types/openapi";

export const getClient = async () => {
  const api = new OpenAPIClientAxios({
    definition: `${import.meta.env.VITE_BASE_URL}/swagger/v1/swagger.json`,
  });

  const axiosClient = await api.init<Client>();

  // Add a request interceptor to append the access token to the request
  axiosClient.interceptors.request.use((config) => {
    const userStore = useUserStore();
    const token = userStore.userInfo?.access_token;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  });

  return axiosClient;
};
