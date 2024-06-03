<template>
  <Banner type="error" title="404 Error - Page not found" />
  <v-container>
    <div class="d-flex flex-column ga-10 mt-10">
      <h3>The page you’re looking for might have been removed, moved, or is temporarily unavailable.</h3>
      <div class="d-flex flex-column ga-3">
        <h3>We suggest</h3>
        <ul class="ml-10">
          <li>Check you’ve entered the correct URL</li>
          <li>
            Go to our
            <router-link to="/">Home page</router-link>
            and browse through our topics for the information you want or using the search function at the top of
            <router-link to="/">Home page</router-link>
          </li>
          <li>Clear your browsers cache of temporary files and cookies</li>
          <li>Update your bookmarks</li>
        </ul>
      </div>
      <div class="d-flex flex-column ga-3">
        <h3>Or, contact the ECE Registry</h3>
        <ul class="ml-10">
          <li>
            Email:
            <a href="mailto:ECERegistry@gov.bc.ca">ECERegistry@gov.bc.ca</a>
          </li>
          <li>
            Victoria:
            <a href="tel:2503876121">250 387-6121</a>
          </li>
          <li>
            Toll-free within Canada and USA:
            <a href="tel:18006637867">1 800 663-7867</a>
          </li>
          <li>Outside of Canada/USA: ?</li>
        </ul>
      </div>
    </div>
    <v-btn class="mt-10" color="primary" @click="$router.push('/')">Go to home page</v-btn>
  </v-container>
</template>

<script lang="ts">
import Banner from "@/components/Banner.vue";
export default {
  name: "PageNotFound",
  components: { Banner },
  setup() {
    console.log("PageNotFound");
  },
};
</script>
