import { useAlertStore } from "@/store/alert";

class ApiResultHandler {
  // Inject the alert store
  alertStore = useAlertStore();

  // Method to handle successful API responses
  handleSuccess(response: any) {
    return response.data;
  }

  // Method to handle API errors
  async handleError(error: any) {
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

    // Optionally rethrow the error if you want calling code to handle it as well
    throw error;
  }

  // Method to show error messages
  showErrorMessage(message: any) {
    this.alertStore.setFailureAlert(message);
  }
}
export default ApiResultHandler;
