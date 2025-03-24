<template>
  <v-row>
    <v-col>
      <p>
        <v-icon size="large" icon="mdi-attachment" />
        Attachments
      </p>
      <p>You can upload images, Microsoft Word documents, Microsoft Excel documents, and PDFs. Max file size accepted is 10MB.</p>
      <Callout class="mt-3" type="warning">
        <div class="d-flex flex-column ga-3">
          <p v-if="selectedFiles.length >= maxNumberOfFiles">
            No more files can be added. You can only add {{ maxNumberOfFiles }} file{{ maxNumberOfFiles > 1 ? "s" : "" }}.
          </p>
          <p v-else>Selected files: {{ selectedFiles.length }}/{{ maxNumberOfFiles }}</p>
        </div>
      </Callout>
      <v-btn
        v-if="showAddFileButton && selectedFiles.length < maxNumberOfFiles"
        prepend-icon="mdi-plus"
        variant="text"
        color="primary"
        class="mt-3"
        @click="triggerFileInput"
      >
        Add file
      </v-btn>
      <v-file-input
        ref="fileInput"
        style="display: none"
        :multiple="allowMultipleFiles"
        accept=".txt,.pdf,.doc,.docx,.rtf,.xls,.xlsx,.jpg,.jpeg,.gif,.png,.bmp,.tiff,.x-tiff"
        @change="handleFileUpload"
      ></v-file-input>
      <Alert v-model="showErrorBanner" class="mt-10" type="error">
        <p class="small">{{ errorBannerMessage }}</p>
      </Alert>
      <v-list lines="two" class="flex-grow-1 message-list">
        <v-divider v-if="selectedFiles.length > 0" class="border-opacity-100" color="ash-grey"></v-divider>
        <UploadFileItem
          v-for="(file, index) in selectedFiles"
          :key="index"
          :file-item="file"
          :upload-progress="file.progress"
          :errors="file.fileErrors"
          :can-delete="canDeletePermanentFiles || file.storageFolder === 'temporary'"
          @delete-file="removeFile"
        ></UploadFileItem>
      </v-list>
      <v-input
        :model-value="userFiles"
        :hide-details="'auto'"
        :rules="[!fileErrors, !tooManyFiles, !filesInProgress, !fileTooLarge, !allFilesTooLarge, !duplicateFileName, ...rules]"
      />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import type { AxiosProgressEvent } from "axios";
import { v4 as uuidv4 } from "uuid";
import { defineComponent, type PropType } from "vue";

import { deleteFile, uploadFile } from "@/api/file";
import Alert from "@/components/Alert.vue";
import Callout from "@/components/Callout.vue";
import UploadFileItem, { type FileItem } from "@/components/UploadFileItem.vue";
import { useAlertStore } from "@/store/alert";
import * as Functions from "@/utils/functions";

export interface FileUploaderData {
  selectedFiles: FileItem[];
  showErrorBanner: boolean;
  errorBannerMessage: string;
  firstLoad: boolean;
}

const allowedFileTypes = [
  "application/pdf",
  "text/plain",
  "application/msword",
  "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
  "application/rtf",
  "application/vnd.ms-excel",
  "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
  "image/jpeg",
  "image/gif",
  "image/png",
  "image/bmp",
  "image/tiff",
  "image/x-tiff",
];

export default defineComponent({
  name: "FileUploader",
  components: { UploadFileItem, Alert, Callout },
  props: {
    userFiles: {
      type: Array as PropType<FileItem[]>,
      required: false,
      default: () => [],
    },
    deleteFileFromTempWhenRemoved: {
      type: Boolean,
      required: false,
      default: true,
    },
    canDeletePermanentFiles: {
      type: Boolean,
      required: false,
      default: true,
    },
    rules: {
      type: Array as PropType<any[]>,
      required: false,
      default: () => [],
    },
    allowMultipleFiles: {
      type: Boolean,
      required: false,
      default: true,
    },
    showAddFileButton: {
      type: Boolean,
      required: false,
      default: true,
    },
    maxNumberOfFiles: {
      type: Number,
      required: false,
      default: 5,
    },
  },
  emits: ["update:files", "delete:file"],
  async setup() {
    const alertStore = useAlertStore();
    const maxFileSizeInMB = 10;
    const maxFileSizeInBytes = maxFileSizeInMB * 1024 * 1024; // File Size Limit (10)MB to Bytes

    return {
      alertStore,
      maxFileSizeInBytes,
      maxFileSizeInMB,
    };
  },
  data(): FileUploaderData {
    return {
      selectedFiles: [],
      showErrorBanner: false,
      errorBannerMessage: "",
      firstLoad: true,
    };
  },
  computed: {
    fileErrors() {
      return this.selectedFiles.some((file) => file.fileErrors.length !== 0);
    },
    tooManyFiles() {
      return this.selectedFiles.length > this.maxNumberOfFiles;
    },
    filesInProgress() {
      return this.selectedFiles.some((file) => file.progress < 101);
    },
    fileTooLarge() {
      return this.selectedFiles.some((file) => file.file.size > this.maxFileSizeInBytes);
    },
    allFilesTooLarge() {
      const totalSize = this.selectedFiles?.reduce((acc, sf) => acc + sf.file.size, 0);
      if (totalSize > this.maxFileSizeInBytes) {
        return true;
      }
      return totalSize > this.maxFileSizeInBytes;
    },
    duplicateFileName() {
      const fileNameMap = new Set();
      for (const file of this.selectedFiles) {
        if (fileNameMap.has(file.file.name)) {
          return true; //there is a duplicate stop checking and return true
        }
        fileNameMap.add(file.file.name);
      }

      return false;
    },
  },
  watch: {
    selectedFiles: {
      handler() {
        if (!this.firstLoad) {
          this.updateEmit(); // Emit updates whenever selectedFiles changes
        } else {
          this.firstLoad = false;
        }
      },
      deep: true,
    },
  },
  mounted() {
    this.selectedFiles = [...this.userFiles];
  },
  methods: {
    handleFileUpload(event: Event) {
      const target = event.target as HTMLInputElement;
      const files = target.files;
      if (files) {
        if (this.selectedFiles.length + files.length > this.maxNumberOfFiles) {
          this.showErrorBanner = true;
          this.errorBannerMessage = `You can only upload ${this.maxNumberOfFiles} files. You need to remove files before you can continue.`;
        }
        for (let i = 0; i < files.length; i++) {
          const file = files[i];
          let fileErrors: string[] = [];

          if (file.size > this.maxFileSizeInBytes) {
            fileErrors.push(`This file is too big. Only files ${this.maxFileSizeInMB}MB or smaller are accepted.`);
          }
          if (!allowedFileTypes.includes(file.type)) {
            fileErrors.push(
              "This type of file is not accepted. The following file types are accepted: .txt, .pdf, .doc, .docx, .rtf, .xls, .xlsx, .jpg/jpeg, .gif, .png, .bmp, .tiff, .x-tiff",
            );
          }
          if (this.selectedFiles.some((f: FileItem) => f.file.name === file.name)) {
            fileErrors.push("This file with this name has already been uploaded. ");
          }
          const fileId = uuidv4(); // Generate a unique file ID using uuid
          const selectedFile: FileItem = {
            file,
            fileSize: file.size,
            fileName: file.name,
            progress: 0,
            fileId,
            fileErrors,
            storageFolder: "temporary",
          };
          this.selectedFiles.push(selectedFile);
          if (this.selectedFiles.length > 1) {
            const totalSize = this.selectedFiles.reduce((acc, sf) => acc + sf.file.size, 0);
            if (totalSize > this.maxFileSizeInBytes) {
              fileErrors.push(
                `The total file size exceeds the maximum allowed. Upload a file that is ${Functions.humanFileSize(selectedFile.file.size - (totalSize - this.maxFileSizeInBytes))} or smaller.`,
              );
            }
          }
          if (!(fileErrors.length > 0)) {
            this.uploadFileWithProgress(selectedFile);
          }
        }
      }
    },
    async uploadFileWithProgress(selectedFile: FileItem) {
      const fileClassification = "document";
      const fileTags = this.formatFileTags(selectedFile.file);
      try {
        const response = await uploadFile(selectedFile.fileId, selectedFile.file, fileClassification, fileTags, (progressEvent: AxiosProgressEvent) => {
          const fileIndex = this.selectedFiles.findIndex((f: FileItem) => f.fileId === selectedFile.fileId);
          const total = progressEvent.total ? progressEvent.total : 10485760;
          const progress = Math.round((progressEvent.loaded * 100) / total);
          if (fileIndex > -1) {
            this.selectedFiles[fileIndex].progress = progress;
          }
        });
        if (response.data) {
          const fileIndex = this.selectedFiles.findIndex((f: FileItem) => f.fileId === selectedFile.fileId);
          this.selectedFiles[fileIndex].progress = 101; // means API call was successful
        } else {
          this.removeFile(selectedFile);
          this.alertStore.setFailureAlert("An error occurred during file upload");
        }
      } catch (error) {
        this.removeFile(selectedFile);
        this.alertStore.setFailureAlert("Problem saving files. Check your connection and try again.");
        console.log(error);
      }
    },
    formatFileTags(file: File): string {
      const fileName = Functions.sanitizeFilename(file.name);
      const fileSize = Functions.humanFileSize(file.size); // Convert size to KB
      const fileFormat = file.name.split(".").pop(); // Get file extension

      const tags = {
        Name: fileName,
        Size: fileSize,
        Format: fileFormat,
      };
      return Object.entries(tags)
        .map(([key, value]) => `${key}=${value}`)
        .join(",");
    },
    async removeFile(selectedFile: FileItem) {
      try {
        this.selectedFiles = this.selectedFiles.filter((f: FileItem) => f.fileId !== selectedFile.fileId);
        if (!(selectedFile.fileErrors.length > 0) && selectedFile.storageFolder === "temporary" && this.deleteFileFromTempWhenRemoved) {
          await deleteFile(selectedFile.fileId);
        }
        this.$emit("delete:file", selectedFile);
        this.showErrorBanner = false;
      } catch (error) {
        this.alertStore.setFailureAlert("An error occurred during file deletion.");
      }
    },
    triggerFileInput() {
      const totalSize = this.selectedFiles.reduce((acc, f) => acc + f.file.size, 0);
      if (totalSize > this.maxFileSizeInBytes) {
        this.showErrorBanner = true;
        this.errorBannerMessage = `The total size of uploaded files is ${this.maxFileSizeInMB}MB. You have exceeded the limit. Reduce the number or size of the files and try again.`;
      } else if (this.selectedFiles.length >= this.maxNumberOfFiles) {
        this.showErrorBanner = true;
        this.errorBannerMessage = `You can only upload ${this.maxNumberOfFiles} files. You need to remove files before you can continue.`;
      } else {
        this.showErrorBanner = false;
        (this.$refs.fileInput as HTMLInputElement).click();
      }
      //fix to allow uploading the same file twice
      (this.$refs.fileInput as HTMLInputElement).value = "";
    },
    updateEmit() {
      this.$emit("update:files", this.selectedFiles);
    },
  },
});
</script>
