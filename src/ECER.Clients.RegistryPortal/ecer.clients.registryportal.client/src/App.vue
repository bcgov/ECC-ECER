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
    </Suspense>
  </main>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import EceFooter from "./components/Footer.vue";
import InactiveSessionTimeout from "./components/InactiveSessionTimeout.vue";
import NavigationBar from "./components/NavigationBar.vue";
import Snackbar from "./components/Snackbar.vue";
import { useRoute } from "vue-router";

export default defineComponent({
  name: "App",
  components: {
    NavigationBar,
    EceFooter,
    Snackbar,
    InactiveSessionTimeout,
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
