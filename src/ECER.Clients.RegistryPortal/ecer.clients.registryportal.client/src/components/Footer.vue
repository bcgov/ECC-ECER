<template>
  <v-footer app color="primary" absolute :style="{ 'min-height': '50px' }">
    <v-row justify="start" class="ga-1 ms-2 my-1">
      <router-link v-for="link in links" :key="link.name" :to="link.path"
        ><v-btn variant="text"
          ><p class="white small">{{ link.name }}</p></v-btn
        >
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
  name: "EceFooter",
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
