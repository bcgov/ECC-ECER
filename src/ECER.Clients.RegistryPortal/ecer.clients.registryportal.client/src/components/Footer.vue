<template>
  <AcknowledgementBanner
    v-if="acceptedPaths"
    title="The B.C. Public Service acknowledges the territories of First Nations around B.C. and is grateful to carry out our work on these lands. We acknowledge the rights, interests, priorities, and concerns of all Indigenous Peoples - First Nations, Métis, and Inuit - respecting and acknowledging their distinct cultures, histories, rights, laws, and governments."
    color="black"
    class="mb-3"
  />
  <v-footer :style="{ 'min-height': '50px' }">
    <v-container>
      <div class="w-100 justify-space-between d-sm-flex-column d-md-flex">
        <div>
          <router-link to="/">
            <img src="../assets/bc-gov-logo.png" width="155" class="logo" alt="B.C. Government Logo" />
          </router-link>
        </div>
        <div :class="[smAndUp ? 'w-50' : 'w-100']">
          <p><b>More information</b></p>
          <div class="d-flex">
            <div class="d-flex flex-column w-50">
              <a v-for="link in firstColumnLinks" :key="link.name" :href="link.path" class="small text-black" :target="link?.target">{{ link.name }}</a>
            </div>
            <div class="d-flex flex-column w-50">
              <a v-for="link in secondColumnLinks" :key="link.name" :href="link.path" class="small text-black" :target="link?.target">{{ link.name }}</a>
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
import { useDisplay } from "vuetify";

import AcknowledgementBanner from "@/components/AcknowledgementBanner.vue";

type FooterLink = {
  name: string;
  path: string;
  target?: FooterTarget;
};

type FooterTarget = "_blank";

export default defineComponent({
  name: "EceFooter",
  components: { AcknowledgementBanner },
  setup: () => {
    const { smAndUp } = useDisplay();

    const links: FooterLink[] = [
      { name: "Home", path: "/" },
      { name: "Contact Us", path: "https://www2.gov.bc.ca/gov/content?id=9376DE7539D44C64B3E667DB53320E71", target: "_blank" },
      { name: "Disclaimer", path: "https://www2.gov.bc.ca/gov/content?id=79F93E018712422FBC8E674A67A70535", target: "_blank" },
      { name: "Privacy", path: "https://www2.gov.bc.ca/gov/content?id=9E890E16955E4FF4BF3B0E07B4722932", target: "_blank" },
      { name: "Accessibility", path: "https://www2.gov.bc.ca/gov/content?id=E08E79740F9C41B9B0C484685CC5E412", target: "_blank" },
      { name: "Copyright", path: "https://www2.gov.bc.ca/gov/content?id=1AAACC9C65754E4D89A118B875E0FBDA", target: "_blank" },
    ];
    return { links, smAndUp };
  },
  computed: {
    firstColumnLinks() {
      return this.links.slice(0, 2);
    },
    secondColumnLinks() {
      return this.links.slice(2);
    },
    acceptedPaths(): boolean {
      const routeName = this.$route.name?.toString() || "";
      return ["login", "dashboard", "invalid-reference", "reference-submitted", "verify"].includes(routeName);
    },
  },
});
</script>
