<template>
  <v-input :model-value="modelValue" :rules="props.rules" hide-details="auto">
    <div class="g-recaptcha" :data-sitekey="siteKey" data-callback="recaptchaSuccessCallback" data-expired-callback="recaptchaExpiredCallback"></div>
  </v-input>
</template>
<script lang="ts">
import { useScriptTag } from "@vueuse/core";
import { defineComponent } from "vue";

import type { EceRecaptchaProps } from "@/types/input";

export default defineComponent({
  name: "EceRecaptcha",
  props: {
    modelValue: {
      type: String,
      required: true,
    },
    props: {
      type: Object as () => EceRecaptchaProps,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_recaptchaToken: string) => true,
  },
  async setup() {
    const siteKey = "6LfrTccpAAAAABegrjoiYef-YK45zY2_05X-xq13";
    useScriptTag("https://www.recaptcha.net/recaptcha/api.js");

    return { siteKey };
  },
  mounted() {
    window.recaptchaSuccessCallback = this.recaptchaSuccessCallback;
    window.recaptchaExpiredCallback = this.recaptchaExpiredCallback;
  },
  methods: {
    recaptchaSuccessCallback(data: string) {
      this.$emit("update:model-value", data);
    },
    recaptchaExpiredCallback() {
      this.$emit("update:model-value", "");
    },
  },
});
</script>
