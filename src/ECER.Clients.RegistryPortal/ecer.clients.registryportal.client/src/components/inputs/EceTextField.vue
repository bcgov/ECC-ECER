<template>
  <label>
    {{ label }}
    <v-text-field
      ref="textField"
      v-bind="$attrs"
      :aria-label="label"
      label=""
      @input="$emit('input', $event.target.value)"
      @keypress="
        if (isNumeric) {
          isNumber($event);
        }
      "
      :class="['pt-2', $attrs.class]"
    ></v-text-field>
  </label>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { isNumber } from "@/utils/formInput";
import type { VTextField } from "vuetify/components";

export type ECETextField = {
  resetValidation(): void;
};

export default defineComponent({
  name: "EceTextField",
  props: {
    label: {
      type: String,
      default: "",
    },
    isNumeric: {
      type: Boolean,
      default: false,
    },
  },
  emits: ["input"],
  methods: {
    isNumber,
    resetValidation() {
      (this.$refs.textField as VTextField).resetValidation();
    },
  },
});
</script>
