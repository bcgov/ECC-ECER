<template>
  <v-list-item>
    <v-row class="d-flex align-center">
      <!-- File Name and Size -->
      <v-col cols="4">
        <div class="d-flex justify-start">
          <p class="text-truncate">{{ fileItem.file.name }}</p>
          <p class="text-no-wrap">&nbsp;({{ Functions.humanFileSize(fileItem.file.size) }})</p>
        </div>
      </v-col>

      <!-- Progress Bar or Upload Completed -->
      <v-col cols="4">
        <div v-if="isUploadComplete">Upload completed</div>
        <v-progress-linear v-else :model-value="uploadProgress" height="20" color="primary"></v-progress-linear>
      </v-col>

      <!-- Delete Button -->
      <v-col cols="4" class="d-flex justify-end">
        <v-tooltip text="Delete" location="top">
          <template #activator="{ props }">
            <v-btn v-if="isUploadComplete" v-bind="props" icon="mdi-trash-can-outline" variant="plain" @click="deleteFile" />
          </template>
        </v-tooltip>
      </v-col>
    </v-row>
  </v-list-item>
  <v-divider></v-divider>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import * as Functions from "@/utils/functions";

export interface FileItem {
  file: File;
  fileId: string;
  progress: number;
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
