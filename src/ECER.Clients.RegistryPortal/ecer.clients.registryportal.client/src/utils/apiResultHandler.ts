import type { AxiosResponse } from "axios";

import { useAlertStore } from "@/store/alert";
import type { LoadingOperation } from "@/store/loading";
import { useLoadingStore } from "@/store/loading";

// Define the structure of the response object
export interface ApiResponse<T> {
  data?: T;
  error?: any;
}

class ApiResultHandler {
  // Inject the alert store

  // Generic method to execute an API request and handle the response
  public async execute<T>(request: Promise<AxiosResponse<T>>, key?: LoadingOperation): Promise<ApiResponse<T>> {
    try {
      if (key) this.setLoadingState(key, true);

      const response = await request;
      return { data: response.data };
    } catch (error: any) {
      this.handleError(error);
      return { error: error.response ? error.response.data : "An unknown error occurred" };
    } finally {
      if (key) this.setLoadingState(key, true);
    }
  }

  // Method to handle API errors
  private handleError(error: any) {
    if (error.response) {
      const status = error.response.status;
      if (status === 400) {
        // Extract error message from server's response
        const errorMessage = error.response.data.message || "Bad request. Please check your input.";
        this.showErrorMessage(errorMessage);
      } else if (status === 404) {
        // Handle 404 Not Found
        this.showErrorMessage("Requested resource not found.");
      } else if (status === 500) {
        // Handle 500 Internal Server Error
        this.showErrorMessage("Server error. Please try again later.");
      }
    } else if (error.request) {
      // The request was made but no response was received
      this.showErrorMessage("No response from server. Please check your network.");
    } else {
      // Something happened in setting up the request that triggered an Error
      this.showErrorMessage("Error in request: " + error.message);
    }
  }

  // Method to show error messages
  private showErrorMessage(message: any) {
    const alertStore = useAlertStore();
    alertStore.setFailureAlert(message);
  }

  private setLoadingState(key: string, loading: boolean) {
    const loadingStore = useLoadingStore();
    loadingStore.setLoading(key, loading);
  }
}

export default ApiResultHandler;
