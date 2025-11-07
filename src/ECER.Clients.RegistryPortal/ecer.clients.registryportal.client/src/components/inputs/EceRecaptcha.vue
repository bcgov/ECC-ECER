<template>
  <v-input :model-value="modelValue" :rules="rules" hide-details="auto">
    <div
      :id="recaptchaElementId"
      class="g-recaptcha"
      :data-sitekey="siteKey"
      data-callback="recaptchaSuccessCallback"
      data-expired-callback="recaptchaExpiredCallback"
    ></div>
  </v-input>
</template>
<script lang="ts">
import { useScriptTag } from "@vueuse/core";
import { defineComponent } from "vue";

import { getRecaptchaSiteKey } from "@/api/configuration";
import type { EceRecaptchaProps } from "@/types/input";

export default defineComponent({
  name: "EceRecaptcha",
  props: {
    modelValue: {
      type: String,
      required: true,
    },
    recaptchaElementId: {
      type: String,
      required: true,
    },
    rules: {
      type: Array as () => EceRecaptchaProps["rules"],
      required: true,
    },
  },
  emits: {
    "update:model-value": (_recaptchaToken: string) => true,
  },
  async setup() {
    const siteKey = await getRecaptchaSiteKey();
    useScriptTag("https://www.recaptcha.net/recaptcha/api.js?onload=recaptchaOnloadCallback&render=explicit", () => {}, { async: true, defer: true });

    return { siteKey };
  },
  mounted() {
    globalThis.recaptchaSuccessCallback = this.recaptchaSuccessCallback;
    globalThis.recaptchaExpiredCallback = this.recaptchaExpiredCallback;
    globalThis.recaptchaOnloadCallback = this.recaptchaOnloadCallback;

    if (globalThis.grecaptcha) {
      //checks to see if we've already loaded the grecaptcha script.
      globalThis.grecaptcha.render(this.recaptchaElementId);
    }
  },
  methods: {
    recaptchaSuccessCallback(data: string) {
      this.$emit("update:model-value", data);
    },
    recaptchaExpiredCallback() {
      this.$emit("update:model-value", "");
    },
    recaptchaOnloadCallback() {
      globalThis.grecaptcha.render(this.recaptchaElementId);
    },
  },
});
</script>
