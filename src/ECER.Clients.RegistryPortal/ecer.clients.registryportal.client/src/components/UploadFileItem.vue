<template>
  <v-list-item>
    <v-row class="d-flex align-center">
      <!-- File Name and Size -->
      <v-col cols="4">
        <div>{{ file.name }} ({{ (file.size / 1024).toFixed(2) }} KB)</div>
      </v-col>

      <!-- Progress Bar or Upload Completed -->
      <v-col cols="4">
        <div v-if="isUploadComplete">Upload completed</div>
        <v-progress-linear v-else :buffer-value="uploadProgress" height="20" color="primary"></v-progress-linear>
      </v-col>

      <!-- Delete Button -->
      <v-col cols="4" class="d-flex justify-end">
        <v-icon @click="deleteFile">mdi-delete</v-icon>
      </v-col>
    </v-row>
  </v-list-item>
  <v-divider></v-divider>
</template>

<script lang="ts">
import { defineComponent } from "vue";

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
