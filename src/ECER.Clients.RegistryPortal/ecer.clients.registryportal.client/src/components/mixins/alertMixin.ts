import { mapActions } from "pinia";

import { useAlertStore } from "../../store/alert";
import { AlertNotificationType } from "../../utils/constant";

export default {
  data() {
    return {
      alertType: null,
    };
  },
  methods: {
    ...mapActions(useAlertStore, ["addAlertNotification"]),
    setSuccessAlert(message: String) {
      this.addAlertNotification({ text: message, alertType: AlertNotificationType.SUCCESS });
    },
    setFailureAlert(message: String) {
      this.addAlertNotification({ text: message, alertType: AlertNotificationType.ERROR });
    },
    setWarningAlert(message: String) {
      this.addAlertNotification({ text: message, alertType: AlertNotificationType.WARN });
    },
  },
};
