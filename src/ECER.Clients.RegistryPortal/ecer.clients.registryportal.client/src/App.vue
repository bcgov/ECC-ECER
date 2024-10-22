<template>
  <main>
    <Suspense>
      <v-app>
        <NavigationBar v-if="showNavigationBarAndFooter" />
        <v-main class="fill-height">
          <InactiveSessionTimeout />
          <Snackbar />
          <router-view></router-view>
        </v-main>
        <EceFooter v-if="showNavigationBarAndFooter" />
      </v-app>
      <!-- loading state via #fallback slot -->
      <template #fallback><Loading /></template>
    </Suspense>
  </main>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import EceFooter from "@/components/Footer.vue";
import InactiveSessionTimeout from "@/components/InactiveSessionTimeout.vue";
import NavigationBar from "@/components/NavigationBar.vue";
import Loading from "@/components/Loading.vue";
import Snackbar from "@/components/Snackbar.vue";
import { useRoute } from "vue-router";

export default defineComponent({
  name: "App",
  components: {
    NavigationBar,
    EceFooter,
    Snackbar,
    InactiveSessionTimeout,
    Loading,
  },
  setup() {
    const route = useRoute();

    return { route };
  },
  computed: {
    showNavigationBarAndFooter() {
      return !this.route.path.includes("reply");
    },
  },
});
</script>

<style lang="scss">
@import "@/styles/main.scss";
</style>
