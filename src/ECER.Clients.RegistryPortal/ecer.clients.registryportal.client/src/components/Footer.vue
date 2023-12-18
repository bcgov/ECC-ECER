<template>
  <v-footer app color="primary" :style="{ 'min-height': '50px' }">
    <v-row justify="start" class="ga-8 ms-2 my-1">
      <router-link v-for="link in links" :key="link.name" :style="{ 'text-decoration': 'none' }" :to="link.path"
        ><p class="white">{{ link.name }}</p>
      </router-link>
    </v-row>
  </v-footer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

type FooterLink = {
  name: string;
  path: string;
};

export default defineComponent({
  setup: () => {
    const links: FooterLink[] = [
      { name: "Home", path: "/" },
      { name: "Disclaimer", path: "/disclaimer" },
      { name: "Privacy", path: "/privacy" },
      { name: "Terms of Use", path: "/terms-of-use" },
      { name: "Accessibility", path: "/accessibility" },
      { name: "Contact Us", path: "/contact-us" },
    ];
    return { links };
  },
});
</script>
