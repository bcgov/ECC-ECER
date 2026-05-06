<template>
  <v-row>
    <v-col>
      <div class="d-flex flex-column ga-3">
        <p>
          <v-icon size="large" icon="mdi-attachment" />
          Supporting documents
        </p>
        <p>
          You can upload images, Microsoft Word documents, Microsoft Excel
          documents, and PDFs. Max file size accepted is 10MB. The maximum
          number of files for each question is 5.
        </p>
        <p class="font-weight-bold">
          All attachments must be directly related to your request. Do not
          upload any personal or sensitive information (e.g., transcripts,
          resumes, CVs, etc.)
        </p>
        <v-row
          v-if="
            showAddFileButton &&
            selectedFiles.length < maxNumberOfFiles &&
            !filesInProgress
          "
          no-gutters
          class="pa-3 bg-hawkes-blue rounded"
        >
          <v-col class="pa-0">
            <v-btn
              prepend-icon="mdi-plus"
              variant="text"
              color="primary"
              @click="triggerFileInput"
            >
              Add file
            </v-btn>
          </v-col>
          <v-spacer />
          <v-col cols="6" class="pa-0">
            <v-select
              v-if="selectableExistingFiles.length > 0"
              :key="selectedFiles.length"
              label="Select a previously uploaded file"
              :items="selectableExistingFiles"
              :item-title="(f) => f.fileName ?? ''"
              :item-value="(f) => f"
              variant="outlined"
              density="compact"
              hide-details
              style="background-color: white"
              @update:model-value="(val) => val && selectExistingFile(val)"
            />
          </v-col>
        </v-row>
        <p v-if="selectedFiles.length > 0">
          <span class="text-success font-weight-bold">
            <v-icon>mdi-file-check-outline</v-icon>
            Attached files {{ selectedFiles.length }}/{{ maxNumberOfFiles }}
          </span>
          <span v-if="selectedFiles.length >= maxNumberOfFiles">
            - No more files can be added. You can only add 5 files.
          </span>
        </p>
      </div>

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
        <v-divider
          v-if="selectedFiles.length > 0"
          class="border-opacity-100"
          color="ash-grey"
        ></v-divider>
        <UploadFileItem
          v-for="(file, index) in selectedFiles"
          :key="index"
          :file-item="file"
          :upload-progress="file.progress"
          :errors="file.fileErrors"
          :can-delete="
            canDeletePermanentFiles || file.storageFolder === 'temporary'
          "
          @delete-file="removeFile"
        ></UploadFileItem>
      </v-list>

      <v-input
        :model-value="selectedFiles"
        :hide-details="'auto'"
        :rules="[
          !fileErrors,
          !tooManyFiles,
          !filesInProgress,
          !fileTooLarge,
          !allFilesTooLarge,
          !duplicateFileName,
          ...rules,
        ]"
      />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import type { AxiosProgressEvent } from "axios";
import { v4 as uuidv4 } from "uuid";
import { defineComponent, type PropType } from "vue";

import {
  deleteProgramApplicationFile,
  shareExistingDocument,
  uploadProgramApplicationFile,
  type ApplicationFileInfo,
} from "@/api/programApplicationFiles";
import Alert from "@/components/Alert.vue";
import Callout from "@/components/common/Callout.vue";
import UploadFileItem, {
  type FileItem,
} from "@/components/common/UploadFileItem.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationFilesStore } from "@/store/applicationFiles";
import * as Functions from "@/utils/functions";

const allowedFileTypes = new Set([
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
]);

export default defineComponent({
  name: "ProgramApplicationFileUploader",
  components: { UploadFileItem, Alert, Callout },
  props: {
    userFiles: {
      type: Array as PropType<FileItem[]>,
      required: false,
      default: () => [],
    },
    programApplicationId: {
      type: String,
      required: true,
    },
    componentGroupId: {
      type: String,
      required: true,
    },
    componentId: {
      type: String,
      required: true,
    },
    canDeletePermanentFiles: {
      type: Boolean,
      required: false,
      default: false,
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
    const applicationFilesStore = useApplicationFilesStore();
    const maxFileSizeInMB = 10;
    const maxFileSizeInBytes = maxFileSizeInMB * 1024 * 1024;
    return {
      alertStore,
      applicationFilesStore,
      maxFileSizeInBytes,
      maxFileSizeInMB,
    };
  },
  data() {
    return {
      selectedFiles: [] as FileItem[],
      showErrorBanner: false,
      errorBannerMessage: "",
      firstLoad: true,
    };
  },
  computed: {
    availableFiles(): ApplicationFileInfo[] {
      return this.applicationFilesStore.getFiles(this.programApplicationId);
    },
    selectableExistingFiles(): ApplicationFileInfo[] {
      const attachedNames = new Set(
        this.selectedFiles.map((f) => f.fileName.toLowerCase()),
      );
      return this.availableFiles.filter(
        (f) => !attachedNames.has((f.fileName ?? "").toLowerCase()),
      );
    },
    fileErrors(): boolean {
      return this.selectedFiles.some((f) => f.fileErrors.length !== 0);
    },
    tooManyFiles(): boolean {
      return this.selectedFiles.length > this.maxNumberOfFiles;
    },
    filesInProgress(): boolean {
      return this.selectedFiles.some((f) => f.progress < 101);
    },
    fileTooLarge(): boolean {
      return this.selectedFiles.some(
        (f) => f.file.size > this.maxFileSizeInBytes,
      );
    },
    allFilesTooLarge(): boolean {
      const total = this.selectedFiles.reduce((acc, f) => acc + f.file.size, 0);
      return total > this.maxFileSizeInBytes;
    },
    duplicateFileName(): boolean {
      const seen = new Set<string>();
      for (const f of this.selectedFiles) {
        const lower = f.fileName.toLowerCase();
        if (seen.has(lower)) return true;
        seen.add(lower);
      }
      return false;
    },
  },
  watch: {
    selectedFiles: {
      handler() {
        if (this.firstLoad) {
          this.firstLoad = false;
        } else {
          this.$emit("update:files", this.selectedFiles);
        }
      },
      deep: true,
    },
  },
  async mounted() {
    this.selectedFiles = [...this.userFiles];
    await this.applicationFilesStore.refreshFiles(this.programApplicationId);
  },
  methods: {
    isNameTakenByApplication(fileName: string): boolean {
      const lower = fileName.toLowerCase();
      return this.availableFiles.some(
        (f) =>
          (f.fileName ?? "").toLowerCase() === lower &&
          !this.selectedFiles.some(
            (s) => s.shareDocumentUrlId === f.shareDocumentUrlId,
          ),
      );
    },
    handleFileUpload(event: Event) {
      const target = event.target as HTMLInputElement;
      const files = target.files;
      if (!files) return;

      if (this.selectedFiles.length + files.length > this.maxNumberOfFiles) {
        this.showErrorBanner = true;
        this.errorBannerMessage = `You can only upload ${this.maxNumberOfFiles} files. You need to remove files before you can continue.`;
      }

      for (const file of Array.from(files)) {
        this.addFileToQueue(file);
      }
    },
    addFileToQueue(file: File) {
      const fileErrors = this.validateFile(file);
      const fileId = uuidv4();
      const selectedFile: FileItem = {
        file,
        fileSize: Functions.humanFileSize(file.size),
        fileName: file.name,
        progress: 0,
        fileId,
        fileErrors,
        storageFolder: "permanent",
      };
      this.selectedFiles.push(selectedFile);

      if (this.selectedFiles.length > 1) {
        const totalSize = this.selectedFiles.reduce(
          (acc, sf) => acc + sf.file.size,
          0,
        );
        if (totalSize > this.maxFileSizeInBytes) {
          fileErrors.push(
            `The total file size exceeds the maximum allowed. Upload a file that is ${Functions.humanFileSize(selectedFile.file.size - (totalSize - this.maxFileSizeInBytes))} or smaller.`,
          );
        }
      }

      if (fileErrors.length === 0) {
        this.uploadFileWithProgress(selectedFile);
      }
    },
    validateFile(file: File): string[] {
      const fileErrors: string[] = [];
      if (file.size > this.maxFileSizeInBytes) {
        fileErrors.push(
          `This file is too big. Only files ${this.maxFileSizeInMB}MB or smaller are accepted.`,
        );
      }
      if (!allowedFileTypes.has(file.type)) {
        fileErrors.push(
          "This type of file is not accepted. The following file types are accepted: .txt, .pdf, .doc, .docx, .rtf, .xls, .xlsx, .jpg/jpeg, .gif, .png, .bmp, .tiff, .x-tiff",
        );
      }
      if (
        this.selectedFiles.some(
          (f) => f.fileName.toLowerCase() === file.name.toLowerCase(),
        )
      ) {
        fileErrors.push("A file with this name has already been added.");
      } else if (this.isNameTakenByApplication(file.name)) {
        fileErrors.push(
          "A file with this name has already been uploaded to this application.",
        );
      }
      return fileErrors;
    },
    async uploadFileWithProgress(selectedFile: FileItem) {
      try {
        const response = await uploadProgramApplicationFile(
          this.programApplicationId,
          this.componentGroupId,
          this.componentId,
          selectedFile.fileId,
          selectedFile.file,
          (progressEvent: AxiosProgressEvent) => {
            const idx = this.selectedFiles.findIndex(
              (f) => f.fileId === selectedFile.fileId,
            );
            const total = progressEvent.total ?? 10485760;
            const progress = Math.round((progressEvent.loaded * 100) / total);
            if (idx > -1 && this.selectedFiles[idx]) {
              this.selectedFiles[idx].progress = progress;
            }
          },
        );

        if (response.data) {
          const idx = this.selectedFiles.findIndex(
            (f) => f.fileId === selectedFile.fileId,
          );
          if (this.selectedFiles[idx]) {
            this.selectedFiles[idx].progress = 101;
            this.selectedFiles[idx].shareDocumentUrlId =
              response.data.shareDocumentUrlId ?? undefined;
          }
          await this.applicationFilesStore.refreshFiles(
            this.programApplicationId,
          );
        } else {
          this.removeFile(selectedFile);
          this.alertStore.setFailureAlert(
            "An error occurred during file upload",
          );
        }
      } catch (error) {
        this.removeFile(selectedFile);
        this.alertStore.setFailureAlert(
          "Problem saving files. Check your connection and try again.",
        );
        console.log(error);
      }
    },
    async selectExistingFile(appFile: ApplicationFileInfo) {
      if (!appFile.documentUrlId) return;

      const fileItem: FileItem = {
        file: new File([], appFile.fileName ?? ""),
        fileId: appFile.documentUrlId,
        shareDocumentUrlId: undefined,
        progress: 0,
        fileErrors: [],
        fileSize: appFile.fileSize ?? "",
        fileName: appFile.fileName ?? "",
        storageFolder: "permanent",
        isLinking: true,
      };
      this.selectedFiles.push(fileItem);

      const response = await shareExistingDocument(
        this.programApplicationId,
        this.componentGroupId,
        this.componentId,
        appFile.documentUrlId,
      );

      const idx = this.selectedFiles.findIndex(
        (f) => f.fileId === fileItem.fileId,
      );
      if (!response.data) {
        this.alertStore.setFailureAlert("Failed to attach the selected file.");
        if (idx > -1) this.selectedFiles.splice(idx, 1);
        return;
      }

      if (idx > -1 && this.selectedFiles[idx]) {
        this.selectedFiles[idx].shareDocumentUrlId =
          response.data.shareDocumentUrlId ?? undefined;
        this.selectedFiles[idx].fileSize =
          response.data.fileSize ?? fileItem.fileSize;
        this.selectedFiles[idx].progress = 101;
        this.selectedFiles[idx].isLinking = false;
      }
    },
    async removeFile(selectedFile: FileItem) {
      const idx = this.selectedFiles.findIndex(
        (f) => f.fileId === selectedFile.fileId,
      );
      if (idx > -1 && this.selectedFiles[idx])
        this.selectedFiles[idx].isDeleting = true;

      try {
        if (
          selectedFile.fileErrors.length === 0 &&
          selectedFile.shareDocumentUrlId
        ) {
          await deleteProgramApplicationFile(
            this.programApplicationId,
            selectedFile.shareDocumentUrlId,
          );
          await this.applicationFilesStore.refreshFiles(
            this.programApplicationId,
          );
        }

        this.selectedFiles = this.selectedFiles.filter(
          (f) => f.fileId !== selectedFile.fileId,
        );
        this.$emit("delete:file", selectedFile);
        this.showErrorBanner = false;
      } catch (error) {
        if (idx > -1 && this.selectedFiles[idx])
          this.selectedFiles[idx].isDeleting = false;
        this.alertStore.setFailureAlert(
          "An error occurred during file deletion.",
        );
        console.error(error);
      }
    },
    triggerFileInput() {
      const totalSize = this.selectedFiles.reduce(
        (acc, f) => acc + f.file.size,
        0,
      );
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
      (this.$refs.fileInput as HTMLInputElement).value = "";
    },
  },
});
</script>
