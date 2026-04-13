// Used to prevent a bug with the wizard specifically with our character + work reference flows,
// where the user will see bugs if they navigate from opting out and submitting the reference.
// bug due to how our captcha is not unmounted when we navigate between the steps so we will have 2 captchas affecting our
// captcha token variable.

import { defineStore } from "pinia";

export interface CaptchaTurnstileState {
  widgetIds: string[];
}

export const useCaptchaTurnstileStore = defineStore("captchaTurnstileStore", {
  state: (): CaptchaTurnstileState => ({
    widgetIds: [],
  }),
  actions: {
    async addWidgetId(widgetId: string) {
      this.widgetIds.push(widgetId);
    },
    async resetAllCaptchaTurnstileWidgets() {
      if (window.turnstile) {
        this.widgetIds.forEach((widgetId) => {
          window.turnstile.reset(widgetId);
        });
      }
    },
  },
});
