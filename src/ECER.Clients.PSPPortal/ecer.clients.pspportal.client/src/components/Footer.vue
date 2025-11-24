<template>
  <AcknowledgementBanner v-if="acceptedPaths"
    title="The B.C. Public Service acknowledges the territories of First Nations around B.C. and is grateful to carry out our work on these lands. We acknowledge the rights, interests, priorities, and concerns of all Indigenous Peoples - First Nations, Métis, and Inuit - respecting and acknowledging their distinct cultures, histories, rights, laws, and governments."
    color="black" class="mb-3" />
  <v-footer>
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
              <a v-for="link in firstColumnLinks" :key="link.name" :href="link.path" class="small text-black"
                :target="link?.target">{{ link.name }}</a>
            </div>
            <div class="d-flex flex-column w-50">
              <a v-for="link in secondColumnLinks" :key="link.name" :href="link.path" class="small text-black"
                :target="link?.target">{{ link.name }}</a>
            </div>
          </div>
        </div>
      </div>

      <v-divider class="border-opacity-100 w-100 my-6"></v-divider>
      <div class="w-100 justify-space-between d-flex">
        <p class="small text-black align-self-center">© 2025 Government of British Columbia</p>
        <v-btn @click="handleShowVersionModal" class="align-self-center" flat icon="mdi-information-outline"
          size="x-small"></v-btn>
      </div>
    </v-container>
  </v-footer>

  <ConfirmationDialog :title="'Version Information'" :show="showVersionDialog" @cancel="showVersionDialog = false"
    @accept="showVersionDialog = false" :has-cancel-button="false" :accept-button-text="'Close'">
    <template #confirmation-text>
      <div class="d-flex flex-column ga-3 my-6">
        <p>
          Version:
          <b>{{ version }}</b>
        </p>
        <p>
          Timestamp:
          <b>{{ formattedTimestamp }}</b>
        </p>
        <p>
          Commit:
          <b>{{ commit }}</b>
        </p>
      </div>
    </template>
  </ConfirmationDialog>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";
import { getVersionInfo } from "@/api/version";

import AcknowledgementBanner from "@/components/AcknowledgementBanner.vue";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import { useRoute } from "vue-router";
import { formatDate } from "@/utils/format";

type FooterLink = {
  name: string;
  path: string;
  target?: FooterTarget;
};

type FooterTarget = "_blank";

export default defineComponent({
  name: "EceFooter",
  components: { AcknowledgementBanner, ConfirmationDialog },
  data: () => ({
    showVersionDialog: false,
    version: "",
    timestamp: "",
    commit: "",
  }),
  setup: () => {
    const { smAndUp } = useDisplay();
    const route = useRoute();

    const links: FooterLink[] = [
      { name: "Home", path: "/" },
      { name: "Contact Us", path: "/messages" },
      { name: "Terms of Use", path: "/terms-of-use" },
      { name: "Disclaimer", path: "https://www2.gov.bc.ca/gov/content?id=79F93E018712422FBC8E674A67A70535", target: "_blank" },
      { name: "Privacy", path: "https://www2.gov.bc.ca/gov/content?id=9E890E16955E4FF4BF3B0E07B4722932", target: "_blank" },
      { name: "Accessibility", path: "https://www2.gov.bc.ca/gov/content?id=E08E79740F9C41B9B0C484685CC5E412", target: "_blank" },
      { name: "Copyright", path: "https://www2.gov.bc.ca/gov/content?id=1AAACC9C65754E4D89A118B875E0FBDA", target: "_blank" },
    ];
    return { links, smAndUp, route };
  },
  computed: {
    firstColumnLinks() {
      return this.links.slice(0, 2);
    },
    secondColumnLinks() {
      return this.links.slice(2);
    },
    acceptedPaths(): boolean {
      const routeName = this.route.name?.toString() || "";
      return ["login", "dashboard", "invalid-reference", "reference-submitted", "verify", "lookup-certification", "lookup-certification-record"].includes(
        routeName,
      );
    },
    formattedTimestamp(): string {
      return formatDate(this.timestamp || "", "FFF");
    },
  },
  methods: {
    async handleShowVersionModal() {
      // Get version information from server
      const versionInfo = await getVersionInfo();

      if (versionInfo) {
        this.version = versionInfo.version ?? "";
        this.timestamp = versionInfo.timestamp ?? "";
        this.commit = versionInfo.commit ?? "";
      }

      this.showVersionDialog = true;
    },
  },
});
</script>
