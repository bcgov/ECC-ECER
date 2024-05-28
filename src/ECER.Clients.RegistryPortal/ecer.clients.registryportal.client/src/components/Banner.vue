<template>
  <v-card :rounded="'0'" flat :color="backgroundColour" :style="{ 'min-height': '200px' }" class="d-flex flex-column justify-center">
    <v-container>
      <div class="d-flex">
        <svg xmlns="http://www.w3.org/2000/svg" width="75" height="75" viewBox="0 0 75 75" fill="none">
          <path
            d="M41.25 41.25H33.75V18.75H41.25M41.25 56.25H33.75V48.75H41.25M37.5 0C32.5754 0 27.6991 0.969966 23.1494 2.85452C18.5997 4.73907 14.4657 7.5013 10.9835 10.9835C3.95088 18.0161 0 27.5544 0 37.5C0 47.4456 3.95088 56.9839 10.9835 64.0165C14.4657 67.4987 18.5997 70.2609 23.1494 72.1455C27.6991 74.03 32.5754 75 37.5 75C47.4456 75 56.9839 71.0491 64.0165 64.0165C71.0491 56.9839 75 47.4456 75 37.5C75 32.5754 74.03 27.6991 72.1455 23.1494C70.2609 18.5997 67.4987 14.4657 64.0165 10.9835C60.5343 7.5013 56.4003 4.73907 51.8506 2.85452C47.3009 0.969966 42.4246 0 37.5 0Z"
            fill="#CE3E39"
          />
        </svg>
        <h2 class="align-self-center ml-6">{{ title }}</h2>
      </div>
    </v-container>
  </v-card>
</template>

<script lang="ts">
import type { PropType } from "vue";
import { defineComponent } from "vue";

export default defineComponent({
  name: "Banner",
  props: {
    type: {
      type: String as PropType<"error" | "warning" | "info" | "success">,
      required: true,
    },
    title: {
      type: String,
      required: true,
    },
  },
  computed: {
    backgroundColour() {
      switch (this.type) {
        case "error":
          return "alert-error";
        case "warning":
          return "alert-warning";
        case "info":
          return "alert-info";
        case "success":
          return "alert-success";
        default:
          return "alert-info";
      }
    },
  },
});
</script>
