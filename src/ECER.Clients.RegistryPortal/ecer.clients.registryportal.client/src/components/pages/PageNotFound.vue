<template>
  <Banner type="error" title="404 Error - Page not found" />
  <v-container>
    <div class="d-flex flex-column ga-10 mt-10">
      <h3>The page you’re looking for might have been removed, moved, or is temporarily unavailable.</h3>
      <div class="d-flex flex-column ga-3">
        <h3>We suggest</h3>
        <ul class="ml-10">
          <li>Check you’ve entered the correct URL</li>
          <li>Clear your browsers cache of temporary files and cookies</li>
          <li>Update your bookmarks</li>
          <li>
            If needed,
            <a href="https://www2.gov.bc.ca/gov/content?id=9376DE7539D44C64B3E667DB53320E71">contact the ECE Registry</a>
          </li>
        </ul>
      </div>
    </div>
    <v-btn class="mt-16" color="primary" @click="router.push('/')">Go to home page</v-btn>
  </v-container>
</template>

<script lang="ts">
import Banner from "@/components/Banner.vue";
import { useRouter } from "vue-router";
export default {
  name: "PageNotFound",
  components: { Banner },
  setup() {
    const router = useRouter();
    return { router };
  },
};
</script>
