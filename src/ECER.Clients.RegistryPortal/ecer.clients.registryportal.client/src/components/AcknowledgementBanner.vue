<template>
  <div>
    <v-divider :thickness="3" color="warning" class="border-opacity-100"></v-divider>
    <v-card :rounded="'0'" flat :color="backgroundColour" :style="{ 'min-height': '100px' }" class="d-flex justify-center border-xl">
      <v-container>
        <p class="align-self-center">{{ title }}</p>
      </v-container>
    </v-card>
    <v-divider :thickness="3" color="warning" class="border-opacity-100"></v-divider>
  </div>
</template>

<script lang="ts">
import type { PropType } from "vue";
import { defineComponent } from "vue";

export default defineComponent({
  name: "AcknowledgementBanner",
  props: {
    color: {
      type: String as PropType<"black">,
      required: true,
    },
    title: {
      type: String,
      required: true,
    },
  },
  computed: {
    backgroundColour() {
      switch (this.color) {
        case "black":
          return "black";
        default:
          return "";
      }
    },
  },
});
</script>
