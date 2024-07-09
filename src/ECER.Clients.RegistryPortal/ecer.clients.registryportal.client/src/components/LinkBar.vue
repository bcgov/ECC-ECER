<template>
  <v-container>
    <v-card :rounded="'0'" flat color="background-light" height="74px" width="100%">
      <div class="d-flex flex-row align-center fill-height ga-10 mx-4">
        <div v-for="(link, index) in links" :key="index">
          <router-link :to="link.to">
            {{ link.title }}
          </router-link>
        </div>
      </div>
    </v-card>
  </v-container>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { RouteLocationRaw } from "vue-router";

export interface LinkBarItem {
  title: string;
  to: RouteLocationRaw;
}

export default defineComponent({
  name: "LinkBar",
  props: {
    links: {
      type: Array as PropType<LinkBarItem[]>,
      required: true,
    },
  },
});
</script>
