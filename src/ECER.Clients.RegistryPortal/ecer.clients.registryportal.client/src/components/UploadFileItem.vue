<template>
  <v-list-item>
    <v-row class="d-flex align-center">
      <!-- File Name and Size -->
      <v-col cols="4">
        <div class="d-flex justify-start">
          <p class="text-truncate">{{ fileItem.fileName }}</p>
          <p class="text-no-wrap">&nbsp;({{ Functions.humanFileSize(fileItem.fileSize) }})</p>
        </div>
      </v-col>
      <v-col cols="2">
        <v-icon v-if="fileItem.fileErrors.length > 0" color="#CE3E39" icon="mdi-alert-circle"></v-icon>
      </v-col>

      <!-- Progress Bar or Upload Completed -->
      <v-col cols="4">
        <div v-if="!(fileItem.fileErrors.length > 0)">
          <div v-if="isUploadComplete">Upload complete</div>
          <v-progress-linear v-else :model-value="uploadProgress" height="20" color="primary"></v-progress-linear>
        </div>
      </v-col>

      <!-- Delete Button -->
      <v-col cols="2" class="d-flex justify-end">
        <v-tooltip text="Delete" location="top">
          <template #activator="{ props }">
            <v-btn v-if="fileItem.fileErrors.length > 0 || isUploadComplete" v-bind="props" icon="mdi-trash-can-outline" variant="plain" @click="deleteFile" />
          </template>
        </v-tooltip>
      </v-col>
    </v-row>
    <div v-if="errors.length > 0" style="color: #ea4335">
      <ul>
        <li v-for="(error, index) in errors" :key="index">{{ error }}</li>
      </ul>
    </div>
  </v-list-item>
  <v-divider class="border-opacity-100" color="ash-grey"></v-divider>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import * as Functions from "@/utils/functions";

export interface FileItem {
  file: File;
  fileId: string;
  progress: number;
  fileErrors: string[];
  fileSize: number;
  fileName: string;
  storageFolder: "temporary" | "permanent";
}

export default defineComponent({
  name: "UploadFileItem",
  props: {
    fileItem: {
      type: Object as PropType<FileItem>,
      required: true,
    },
    uploadProgress: {
      type: Number,
      required: true,
    },
    errors: { type: Array, required: true },
  },
  emits: ["delete-file"], // Declare the delete-file event here
  data() {
    return {
      Functions,
    };
  },
  computed: {
    isUploadComplete() {
      return this.uploadProgress > 100; // 101 means api call was successful
    },
  },
  methods: {
    deleteFile() {
      // Emit the delete-file event with the file as payload
      this.$emit("delete-file", this.$props.fileItem);
    },
  },
});
</script>
