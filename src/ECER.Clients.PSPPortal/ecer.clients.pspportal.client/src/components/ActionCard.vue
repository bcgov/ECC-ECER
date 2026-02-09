<template>
  <v-responsive :aspect-ratio="16 / 9">
    <v-card height="100%" class="custom-card-styling">
      <v-card-title class="text-wrap">
        <v-icon v-if="icon" :icon="icon"></v-icon>
        {{ title }}
      </v-card-title>
      <v-card-text>
        <slot name="content"></slot>
      </v-card-text>
      <div class="d-flex flex-column align-start">
        <slot name="action"></slot>
      </div>
    </v-card>
  </v-responsive>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ActionCard",
  props: {
    title: {
      type: String,
      required: true,
    },
    icon: {
      type: String,
      required: false,
      default: "",
    },
  },
});
</script>

<style scoped lang="scss">
.custom-card-styling {
  border: 1px solid #adb5bd;
  border-top: 5.5px solid black;
}
</style>
