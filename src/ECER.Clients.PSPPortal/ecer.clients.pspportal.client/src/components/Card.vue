<template>
  <v-card
    :rounded="true"
    :border="true"
    flat
    class="pa-6 border-primary border-opacity-100 w-100"
    :class="[
      {
        'card-top-border-large': hasTopBorder && topBorderSize === 'large',
        'card-top-border-small': hasTopBorder && topBorderSize === 'small',
      },
    ]"
    v-bind="$attrs"
  >
    <slot />
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "Card",
  props: {
    hasTopBorder: {
      type: Boolean,
      default: false,
    },
    topBorderSize: {
      type: String,
      default: "large",
      validator: (v: string) => ["large", "small"].includes(v),
    },
  },
});
</script>

<style scoped>
.card-top-border-large {
  border-top: 16px solid rgba(var(--v-theme-primary, #013366));
}

.card-top-border-small {
  border-top: 8px solid rgba(var(--v-theme-primary, #013366));
}
</style>
