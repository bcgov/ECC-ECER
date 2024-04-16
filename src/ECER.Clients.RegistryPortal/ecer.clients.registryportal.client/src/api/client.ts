import OpenAPIClientAxios from "openapi-client-axios";

import { useUserStore } from "@/store/user";
import type { Client } from "@/types/openapi";

export const getClient = async (appendToken: boolean = true) => {
  const api = new OpenAPIClientAxios({
    definition: "/swagger/v1/swagger.json",
  });

  const axiosClient = await api.init<Client>();

  if (appendToken) {
    const userStore = useUserStore();
    const access_token = userStore.accessToken;

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
          userStore.logout();
        } else {
          return Promise.reject(error);
        }
      },
    );
  }

  return axiosClient;
};
