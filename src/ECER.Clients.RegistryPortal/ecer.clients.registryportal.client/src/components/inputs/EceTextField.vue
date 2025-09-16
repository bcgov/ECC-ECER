<template>
  <label>
    <div>
      {{ label }}
      <v-tooltip v-if="tooltipText !== ''" :text="tooltipText" location="top">
        <template #activator="{ props }">
          <v-icon v-bind="props" :icon="tooltipIcon" variant="plain" />
        </template>
      </v-tooltip>
    </div>
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
    tooltipText: {
      type: String,
      required: false,
      default: "",
    },
    tooltipIcon: {
      type: String,
      required: false,
      default: "mdi-information-slab-circle",
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
