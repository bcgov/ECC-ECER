<template>
  <v-card class="px-5">
    <v-card-title class="pl-0 pr-0">
      <div ref="titleRow" class="d-flex align-center">
        <span style="flex: 1; white-space: normal">{{ name }}</span>
        <template v-if="rfaiRequired">
          <v-tooltip
            v-if="chipMode === 'icon'"
            text="Additional information requested"
            location="bottom"
          >
            <template #activator="{ props }">
              <v-icon v-bind="props" color="warning">
                mdi-alert-circle-outline
              </v-icon>
            </template>
          </v-tooltip>
          <v-chip
            v-else
            color="warning"
            variant="flat"
            size="small"
            class="flex-shrink-0 ml-2"
          >
            Additional information requested
          </v-chip>
        </template>
      </div>

      <!-- Hidden measurement elements. These are to track 
          card title and chip size to determine when two
          shrink/wrap. First we shrink the chip then we
          wrap the title
      -->
      <span
        ref="nameMeasure"
        aria-hidden="true"
        style="
          position: absolute;
          visibility: hidden;
          white-space: nowrap;
          pointer-events: none;
        "
      >
        {{ name }}
      </span>
      <span
        ref="fullChipMeasure"
        aria-hidden="true"
        style="position: absolute; visibility: hidden; pointer-events: none"
      >
        <v-chip color="warning" variant="flat" size="small">
          Additional information requested
        </v-chip>
      </span>
      <span
        ref="iconChipMeasure"
        aria-hidden="true"
        style="position: absolute; visibility: hidden; pointer-events: none"
      >
        <v-chip color="warning" variant="flat" size="small">
          <v-icon>mdi-alert-circle-outline</v-icon>
        </v-chip>
      </span>

      <v-divider />
    </v-card-title>
    <v-row no-gutters>
      <v-col cols="12" class="pt-2">
        <p class="font-weight-bold multiline">
          {{ question }}
        </p>
      </v-col>
      <v-col cols="12" class="pt-5">
        <v-textarea
          :model-value="modelValue?.answer ?? ''"
          color="primary"
          variant="outlined"
          hide-details="auto"
          :rules="[Rules.maxLength(5000)]"
          @update:model-value="onAnswerInput"
          :readonly="rfaiRequired ? !rfaiRequired : readOnly"
        />
      </v-col>
    </v-row>
    <v-row no-gutters class="pt-5">
      <suspense>
        <FileUploader
          :user-files="userFilesFromModel"
          :show-add-file-button="rfaiRequired ? rfaiRequired : !readOnly"
          :max-number-of-files="5"
          :can-delete-permanent-files="false"
          @update:files="onFilesUpdate"
          @delete:file="handleFileDelete"
        />
      </suspense>
    </v-row>
  </v-card>
</template>
<script lang="ts">
import { defineComponent, type PropType } from "vue";
import FileUploader from "@/components/common/FileUploader.vue";
import type { FileItem } from "@/components/common/UploadFileItem.vue";
import type { Components } from "@/types/openapi";
import * as Rules from "@/utils/formRules";
import { removeElementByIndex, parseHumanFileSize } from "@/utils/functions";

export interface QuestionModelValue {
  answer?: string;
  files?: Components.Schemas.FileInfo[];
  newFiles?: Components.Schemas.FileInfo[] | null;
  deletedFiles?: Components.Schemas.FileInfo[] | null;
}

function toFileItem(info: Components.Schemas.FileInfo): FileItem {
  return {
    file: new File([], info.name ?? ""),
    fileId: info.id ?? "",
    progress: 101,
    fileErrors: [],
    fileSize: parseHumanFileSize(info.size ?? ""),
    fileName: info.name ?? "",
    storageFolder: "permanent",
  };
}

export default defineComponent({
  name: "Question",
  components: { FileUploader },
  props: {
    modelValue: {
      type: Object as PropType<QuestionModelValue>,
      default: () => ({
        answer: "",
        files: [],
        addedFiles: [],
        deletedFiles: [],
      }),
    },
    name: {
      type: String,
    },
    question: {
      type: String,
    },
    rfaiRequired: {
      type: Boolean,
      default: false,
    },
    readOnly: {
      type: Boolean,
      default: false,
    },
  },
  emits: { "update:modelValue": (_payload: QuestionModelValue) => true },
  data() {
    return {
      Rules,
      userFilesFromModel: [] as FileItem[],
      deletedFiles: [] as Components.Schemas.FileInfo[],
      chipMode: "full" as "full" | "icon",
      titleResizeObserver: null as ResizeObserver | null,
    };
  },
  mounted() {
    const titleRow = this.$refs.titleRow as HTMLElement;
    if (titleRow) {
      this.titleResizeObserver = new ResizeObserver(() =>
        this.updateChipMode(),
      );
      this.titleResizeObserver.observe(titleRow);
    }
  },
  beforeUnmount() {
    this.titleResizeObserver?.disconnect();
  },
  created() {
    if (this.modelValue?.files?.length) {
      this.userFilesFromModel = this.modelValue.files.map(toFileItem);
    }
  },
  methods: {
    updateChipMode() {
      const titleRow = this.$refs.titleRow as HTMLElement;
      const nameMeasure = this.$refs.nameMeasure as HTMLElement;
      const fullChipMeasure = this.$refs.fullChipMeasure as HTMLElement;
      if (!titleRow || !nameMeasure || !fullChipMeasure) return;

      const available = titleRow.clientWidth;
      const textWidth = nameMeasure.offsetWidth;
      const fullChipWidth = fullChipMeasure.offsetWidth;
      const gap = 8; // ml-2

      this.chipMode =
        textWidth + gap + fullChipWidth <= available ? "full" : "icon";
    },
    onAnswerInput(value: string) {
      this.$emit("update:modelValue", {
        ...this.modelValue,
        answer: value ?? "",
      });
    },
    onFilesUpdate(files: FileItem[]) {
      this.userFilesFromModel = files;
      let newFilesWithData = [] as FileItem[]; // Reset attachments

      newFilesWithData = files.filter(
        (file) =>
          file.fileErrors.length === 0 &&
          file.progress === 101 &&
          file.storageFolder === "temporary",
      );

      this.$emit("update:modelValue", {
        ...this.modelValue,
        files: files
          .filter((f) => f.storageFolder === "permanent")
          .map((f) => ({ id: f.fileId, name: f.fileName })),
        newFiles: newFilesWithData.map((file) => ({
          id: file.fileId,
          ecerWebApplicationType: "PSP",
        })),
      });
    },
    handleFileDelete(fileItem: FileItem) {
      if (fileItem.storageFolder === "permanent") {
        //we need to add it to the list of deleted files for the backend to remove.
        this.deletedFiles?.push({
          id: fileItem.fileId,
          ecerWebApplicationType: "PSP",
        });
        let index = this.userFilesFromModel?.findIndex(
          (file) => file.fileId === fileItem.fileId,
        );
        if (index) {
          this.userFilesFromModel = removeElementByIndex(
            this.userFilesFromModel,
            index,
          );
        }
        this.$emit("update:modelValue", {
          ...this.modelValue,
          deletedFiles: this.deletedFiles,
        });
      }
    },
  },
});
</script>
