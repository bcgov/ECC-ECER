<template>
  <v-list-item density="compact">
    <div class="d-flex align-center ga-3 w-100 py-1">
      <!-- Filename + (optional) inline error -->
      <div class="d-flex flex-column flex-grow-1 file-row__name-col">
        <span class="text-truncate">
          {{ fileItem.fileName }}
        </span>
        <ul v-if="hasErrors" class="small text-error ma-0 pl-5">
          <li v-for="(error, index) in errors" :key="index">{{ error }}</li>
        </ul>
      </div>

      <!-- Progress bar — only while in-flight -->
      <div v-if="showProgressBar" class="flex-grow-1">
        <v-progress-linear
          v-if="fileItem.isDeleting"
          indeterminate
          height="20"
          color="error"
        />
        <v-progress-linear
          v-else-if="fileItem.isLinking"
          indeterminate
          height="20"
          color="primary"
        />
        <v-progress-linear
          v-else
          :model-value="uploadProgress"
          height="20"
          color="primary"
        />
      </div>

      <!-- File size, right-aligned, muted -->
      <span class="text-no-wrap text-medium-emphasis flex-shrink-0">
        ({{ fileItem.fileSize }})
      </span>

      <!-- Trash -->
      <v-tooltip text="Delete" location="top">
        <template #activator="{ props }">
          <v-btn
            v-if="
              !fileItem.isDeleting &&
              !fileItem.isLinking &&
              (hasErrors || isUploadComplete) &&
              canDelete
            "
            v-bind="props"
            icon="mdi-trash-can-outline"
            variant="plain"
            density="comfortable"
            @click="deleteFile"
          />
        </template>
      </v-tooltip>
    </div>
  </v-list-item>
  <v-divider class="border-opacity-100" color="ash-grey" />
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

export interface FileItem {
  file: File;
  fileId: string;
  shareDocumentUrlId?: string;
  progress: number;
  fileErrors: string[];
  fileSize: string;
  fileName: string;
  storageFolder: "temporary" | "permanent";
  isDeleting?: boolean;
  isLinking?: boolean;
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
    canDelete: {
      type: Boolean,
      required: false,
      default: true,
    },
    errors: { type: Array as PropType<string[]>, required: true },
  },
  emits: ["delete-file"],
  computed: {
    hasErrors(): boolean {
      return this.errors.length > 0;
    },
    isUploadComplete(): boolean {
      return this.uploadProgress > 100; // 101 means api call was successful
    },
    showProgressBar(): boolean {
      if (this.fileItem.isDeleting || this.fileItem.isLinking) return true;
      return !this.hasErrors && !this.isUploadComplete;
    },
  },
  methods: {
    deleteFile() {
      if (this.canDelete) {
        this.$emit("delete-file", this.$props.fileItem);
      }
    },
  },
});
</script>

<style scoped>
/* Vuetify has no utility for min-width: 0; it's required for text-truncate
   to work inside a flex container (flex children default to min-width: auto
   which blocks shrinking below content size). */
.file-row__name-col {
  min-width: 0;
}
</style>
