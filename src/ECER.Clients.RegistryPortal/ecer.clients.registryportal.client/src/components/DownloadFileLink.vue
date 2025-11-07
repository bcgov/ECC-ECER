<template>
  <v-progress-circular v-if="loading" size="25" color="primary" indeterminate></v-progress-circular>
  <a v-else href="#" @click.prevent="downloadFile"><slot></slot></a>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

export default defineComponent({
  name: "DownloadFileLink",
  props: {
    getFileFunction: {
      type: Function,
      required: true,
    },
    name: {
      type: String as PropType<string | null | undefined>,
      required: true,
    },
  },
  data() {
    return {
      documentUrl: "",
      loading: false,
    };
  },
  unmounted() {
    globalThis.URL.revokeObjectURL(this.documentUrl);
  },
  methods: {
    async downloadFile() {
      this.loading = true;

      try {
        const response = await this.getFileFunction();
        this.documentUrl = globalThis.URL.createObjectURL(response.data);

        const anchor = document.createElement("a");
        anchor.href = this.documentUrl;
        anchor.target = "_blank";
        if (this.isSafari()) {
          anchor.download = this.name || "default-filename";
        }
        document.body.appendChild(anchor);
        anchor.click();
        document.body.removeChild(anchor);
      } catch (e) {
        console.log(e);
      } finally {
        this.loading = false;
      }
    },
    isSafari() {
      const ua = navigator.userAgent.toLowerCase();
      return ua.includes("safari") && !ua.includes("chrome") && !ua.includes("android");
    },
  },
});
</script>
