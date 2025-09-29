<template>
  <div @mouseover="pause = true" @mouseleave="pause = false">
    <v-snackbar
      id="mainSnackBar"
      v-model="showSnackBar"
      :timeout="timeout"
      elevation="24"
      location="top"
      centered
      :color="colour"
      transition="slide-y-transition"
      class="snackbar"
    >
      <v-alert :color="colour" :icon="icon" density="compact">{{ alertNotificationText }}</v-alert>
      <template #actions>
        <v-btn text="true" :color="colour == AlertNotificationType.WARN ? 'black' : 'white'" v-bind="$attrs" @click="showSnackBar = false">
          {{ alertNotificationQueue.length > 0 ? "Next (" + alertNotificationQueue.length + ")" : "Close" }}
        </v-btn>
      </template>
    </v-snackbar>
  </div>
</template>

<script lang="ts">
import { mapActions, mapState } from "pinia";

import { useAlertStore } from "@/store/alert";
import { AlertNotificationType } from "@/utils/constant";

export default {
  name: "Snackbar",
  setup() {
    return {
      AlertNotificationType,
    };
  },
  data() {
    return {
      colour: "",
      icon: "",
      polling: 1000,
      timeout: 5000,
      pause: false,
    };
  },
  computed: {
    ...mapState(useAlertStore, ["alertNotificationText", "alertNotificationQueue", "alertNotification"]),
    hasNotificationsPending() {
      return this.alertNotificationQueue.length > 0;
    },
    showSnackBar: {
      get(): any {
        return this.alertNotification;
      },
      set(val: Boolean) {
        this.setAlertNotification(val);
      },
    },
  },
  watch: {
    showSnackBar() {
      if (!this.showSnackBar && this.hasNotificationsPending) {
        this.$nextTick(() => (this.showSnackBar = true));
      } else if (this.showSnackBar && this.hasNotificationsPending) {
        this.setupSnackBar();
      } else {
        this.teardownSnackBar();
      }
    },
  },
  methods: {
    ...mapActions(useAlertStore, ["setAlertNotificationText", "setAlertNotification"]),
    setAlertType(alertType: String) {
      if (!alertType) {
        alertType = "";
      }
      switch (alertType.toLowerCase()) {
        case AlertNotificationType.ERROR:
          this.colour = AlertNotificationType.ERROR;
          this.icon = "mdi-alert-circle-outline";
          break;
        case AlertNotificationType.WARN:
          this.colour = AlertNotificationType.WARN;
          this.icon = "mdi-alert-outline";
          break;
        case AlertNotificationType.SUCCESS:
          this.colour = AlertNotificationType.SUCCESS;
          this.icon = "mdi-check-circle-outline";
          break;
        case AlertNotificationType.INFO:
        default:
          this.colour = AlertNotificationType.INFO;
      }
    },
    setupSnackBar() {
      let alertObject = this.alertNotificationQueue.shift();
      this.setAlertNotificationText(alertObject!.text);
      this.setAlertType(alertObject!.alertType);
      document.addEventListener("keydown", this.close);
      if (alertObject!.alertType === AlertNotificationType.ERROR) {
        this.timeout = 8000;
      } else {
        this.timeout = 5000;
      }
      this.timeoutCounter();
    },
    teardownSnackBar() {
      document.removeEventListener("keydown", this.close);
      clearInterval(this.polling);
    },
    close(e: KeyboardEvent) {
      if ((e.key === "Escape" || e.key === "Esc") && this.showSnackBar) {
        this.showSnackBar = false;
      }
    },
    timeoutCounter() {
      this.polling = globalThis.setInterval(() => {
        if (this.pause) {
          this.timeout += 1;
        }
      }, 1000);
    },
  },
};
</script>

<style>
.snackbar {
  padding: 0 !important;
}
</style>
