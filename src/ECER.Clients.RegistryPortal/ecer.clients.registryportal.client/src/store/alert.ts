import { defineStore } from "pinia";

export interface AlertState {
  alertNotificationText: String;
  alertNotificationQueue: Array<AlertNotification>;
  alertNotification: Boolean;
}

export interface AlertNotification {
  text: String;
  alertType: String;
}

export const useAlertStore = defineStore("alert", {
  state: (): AlertState => ({
    alertNotificationText: "",
    alertNotificationQueue: [],
    alertNotification: false,
  }),
  getters: {},
  actions: {
    async setAlertNotificationText(alertNotificationText: String) {
      this.alertNotificationText = alertNotificationText;
    },
    async setAlertNotification(alertNotification: Boolean) {
      this.alertNotification = alertNotification;
    },
    async addAlertNotification(notification: AlertNotification) {
      this.alertNotificationQueue.push(notification);
      if (!this.alertNotification) {
        this.alertNotification = true;
      }
    },
  },
});
