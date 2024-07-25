<template>
  <v-responsive :aspect-ratio="16 / 9">
    <v-card height="100%" :prepend-icon="icon" :title="title" class="custom-card-styling">
      <v-card-text>
        <slot></slot>
      </v-card-text>
      <div class="d-flex flex-column align-start">
        <v-btn v-for="(link, index) in links" :key="index" variant="text">
          <router-link :to="link.to">
            {{ link.text }}
          </router-link>
        </v-btn>
      </div>
    </v-card>
  </v-responsive>
</template>

<script lang="ts">
import type { PropType } from "vue";
import { defineComponent } from "vue";
import type { RouteLocationRaw } from "vue-router";

export interface Link {
  text: string;
  to: RouteLocationRaw;
}

export default defineComponent({
  name: "ActionCard",
  props: {
    title: {
      type: String,
      required: true,
    },
    icon: {
      type: String,
      required: true,
    },
    links: {
      type: Array as PropType<Link[]>,
      required: false,
      default: () => [],
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
