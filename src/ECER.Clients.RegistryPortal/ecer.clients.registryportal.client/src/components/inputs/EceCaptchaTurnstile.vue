<template>
  <v-input :model-value="modelValue" :rules="rules" hide-details="auto">
    <div :id="captchaElementId" class="cloudflare-captcha-turnstile"></div>
  </v-input>
</template>
<script lang="ts">
import { useScriptTag } from "@vueuse/core";
import { defineComponent, ref } from "vue";
import { getCaptchaSiteKey } from "@/api/configuration";
import { useCaptchaTurnstileStore } from "@/store/captchaTurnstile";

import type { EceCaptchaTurnstileProps } from "@/types/input";

export interface CaptchaTurnstile {
  reset: () => void;
}

export default defineComponent({
  name: "EceCaptchaTurnstile",
  props: {
    modelValue: {
      type: String,
      required: true,
    },
    captchaElementId: {
      type: String,
      required: true,
    },
    rules: {
      type: Array as () => EceCaptchaTurnstileProps["rules"],
      required: true,
    },
  },
  emits: {
    "update:model-value": (_captchaToken: string) => true,
  },
  mounted() {
    if (window.turnstile) {
      this.widgetId = window.turnstile.render(`#${this.captchaElementId}`, {
        sitekey: this.siteKey,
        theme: "light",
        size: "flexible",
        callback: (data: string) => {
          this.$emit("update:model-value", data);
        },
        "expired-callback": () => {
          this.$emit("update:model-value", "");
        },
        "response-field": false,
      });
      this.captchaTurnstileStore.addWidgetId(this.widgetId);
    }
  },
  async setup(props, { emit }) {
    const captchaTurnstileStore = useCaptchaTurnstileStore();
    const siteKey = await getCaptchaSiteKey();
    const widgetId = ref<string | null>(null);
    useScriptTag(
      "https://challenges.cloudflare.com/turnstile/v0/api.js?render=explicit",
      () => {
        widgetId.value = window.turnstile.render(`#${props.captchaElementId}`, {
          sitekey: siteKey,
          theme: "light",
          size: "flexible",
          callback: (data: string) => {
            emit("update:model-value", data);
          },
          "expired-callback": () => {
            emit("update:model-value", "");
          },
          "response-field": false,
        });
        captchaTurnstileStore.addWidgetId(widgetId.value);
      },
      { async: true, defer: true },
    );

    return { siteKey, widgetId, captchaTurnstileStore };
  },
  methods: {
    reset() {
      window.turnstile.reset(this.widgetId);
    },
  },
});
</script>
