<template>
  <v-progress-circular v-if="loading" size="25" color="primary" indeterminate></v-progress-circular>
  <a v-else href="#" @click.prevent="downloadFile"><slot></slot></a>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "DownloadFileLink",
  props: {
    getFileFunction: {
      type: Function,
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
    window.URL.revokeObjectURL(this.documentUrl);
  },
  methods: {
    async downloadFile() {
      this.loading = true;

      try {
        const response = await this.getFileFunction();
        this.documentUrl = window.URL.createObjectURL(response.data);

        const anchor = document.createElement("a");
        anchor.href = this.documentUrl;
        anchor.target = "_blank";
        document.body.appendChild(anchor);
        anchor.click();
        document.body.removeChild(anchor);
      } catch (e) {
        console.log(e);
      } finally {
        this.loading = false;
      }
    },
  },
});
</script>
