<template>
  <main>
    <Suspense>
      <v-app>
        <NavigationBar />
        <v-main>
          <InactiveSessionTimeout />
          <Snackbar />
          <router-view></router-view>
        </v-main>
        <!-- <EceFooter /> -->
      </v-app>
    </Suspense>
  </main>
</template>

<script lang="ts">
import { defineComponent } from "vue";

// import EceFooter from "./components/Footer.vue";
import InactiveSessionTimeout from "./components/InactiveSessionTimeout.vue";
import NavigationBar from "./components/NavigationBar.vue";
import Snackbar from "./components/Snackbar.vue";

export default defineComponent({
  name: "App",
  components: {
    NavigationBar,
    // EceFooter,
    Snackbar,
    InactiveSessionTimeout,
  },
});
</script>

<style lang="scss">
@import "@/styles/main.scss";
</style>
