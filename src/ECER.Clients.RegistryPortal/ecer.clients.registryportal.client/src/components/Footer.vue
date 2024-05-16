<template>
  <v-footer border app absolute :style="{ 'min-height': '50px' }">
    <v-container>
      <div class="d-flex w-100 justify-space-between">
        <router-link to="/">
          <img src="../assets/bc-gov-logo.png" width="155" class="logo ms-6" alt="B.C. Government Logo" />
        </router-link>
        <div class="w-lg-25 w-50">
          <p><b>More information</b></p>
          <div class="d-flex">
            <div class="d-flex flex-column w-50">
              <a v-for="link in firstColumnLinks" :key="link.name" :href="link.path" class="small text-black">{{ link.name }}</a>
            </div>
            <div class="d-flex flex-column w-50">
              <a v-for="link in secondColumnLinks" :key="link.name" :href="link.path" class="small text-black">{{ link.name }}</a>
            </div>
          </div>
        </div>
      </div>
      <v-divider class="border-opacity-100 w-100 my-6"></v-divider>

      <p class="small text-black">© 2024 Government of British Columbia</p>
    </v-container>
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
      { name: "About the ECE Registry", path: "/about" },
      { name: "Contact Us", path: "/contact-us" },
      { name: "Disclaimer", path: "/disclaimer" },
      { name: "Privacy", path: "/privacy" },
      { name: "Accessibility", path: "/accessibility" },
      { name: "Copyright", path: "/copyright" },
    ];
    return { links };
  },
  computed: {
    firstColumnLinks() {
      return this.links.slice(0, Math.floor(this.links.length / 2));
    },
    secondColumnLinks() {
      return this.links.slice(Math.floor(this.links.length / 2));
    },
  },
});
</script>
