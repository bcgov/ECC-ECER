import { defineStore } from "pinia";
import { AlertNotificationType } from "@/utils/constant";

export interface AlertState {
  alertNotificationText: string;
  alertNotificationQueue: Array<AlertNotification>;
  alertNotification: boolean;
}

export interface AlertNotification {
  text: string;
  alertType: string;
}

export const useAlertStore = defineStore("alert", {
  state: (): AlertState => ({
    alertNotificationText: "",
    alertNotificationQueue: [],
    alertNotification: false,
  }),
  getters: {},
  actions: {
    async setAlertNotificationText(alertNotificationText: string) {
      this.alertNotificationText = alertNotificationText;
    },
    async setAlertNotification(alertNotification: boolean) {
      this.alertNotification = alertNotification;
    },
    async addAlertNotification(notification: AlertNotification) {
      this.alertNotificationQueue.push(notification);
      if (!this.alertNotification) {
        this.alertNotification = true;
      }
    },
    // Use these three below methods to show a snackbar notification.
    async setSuccessAlert(message: string) {
      this.addAlertNotification({ text: message, alertType: AlertNotificationType.SUCCESS });
    },
    async setFailureAlert(message: string) {
      this.addAlertNotification({ text: message, alertType: AlertNotificationType.ERROR });
    },
    async setWarningAlert(message: string) {
      this.addAlertNotification({ text: message, alertType: AlertNotificationType.WARN });
    },
  },
});
