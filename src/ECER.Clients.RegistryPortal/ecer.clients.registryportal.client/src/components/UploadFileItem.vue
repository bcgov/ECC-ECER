<template>
  <v-list-item>
    <v-row class="d-flex align-center">
      <!-- File Name and Size -->
      <v-col cols="4">
        <div>{{ file.name }} ({{ Functions.humanFileSize(file.size) }})</div>
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
            <v-btn v-bind="props" icon="mdi-trash-can-outline" variant="plain" @click="deleteFile" />
          </template>
        </v-tooltip>
      </v-col>
    </v-row>
  </v-list-item>
  <v-divider></v-divider>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import * as Functions from "@/utils/functions";

export default defineComponent({
  name: "UploadFileItem",
  props: {
    file: {
      type: File,
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
      return this.uploadProgress >= 100;
    },
  },
  methods: {
    deleteFile() {
      // Emit the delete-file event with the file as payload
      this.$emit("delete-file", this.$props.file);
    },
  },
});
</script>
