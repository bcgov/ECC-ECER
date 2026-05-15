<template>
  <Card has-top-border top-border-size="medium" class="program-question-card">
    <v-card-title class="pa-0 mb-3">
      <div ref="titleRow" class="d-flex align-start ga-3">
        <h3 class="flex-grow-1 multiline text-break">{{ name }}</h3>

        <div
          v-if="rfaiRequired || hasUnsavedChanges"
          class="d-flex flex-column align-end ga-1 question-meta"
        >
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
              class="flex-shrink-0"
            >
              Additional information requested
            </v-chip>
          </template>
          <span
            v-if="hasUnsavedChanges"
            class="small text-error"
            data-testid="unsaved-changes-flag"
          >
            Unsaved changes
          </span>
        </div>

        <!-- Hidden measurement elements for RFAI chip responsive collapse.
             Algorithm: shrink chip first (full -> icon), then wrap title. -->
        <span
          ref="nameMeasure"
          aria-hidden="true"
          class="position-absolute opacity-0 text-no-wrap"
        >
          {{ name }}
        </span>
        <span
          ref="fullChipMeasure"
          aria-hidden="true"
          class="position-absolute opacity-0 pointer-events-none"
        >
          <v-chip color="warning" variant="flat" size="small">
            Additional information requested
          </v-chip>
        </span>
        <span
          ref="iconChipMeasure"
          aria-hidden="true"
          class="position-absolute opacity-0 pointer-events-none"
        >
          <v-chip color="warning" variant="flat" size="small">
            <v-icon>mdi-alert-circle-outline</v-icon>
          </v-chip>
        </span>
      </div>
    </v-card-title>

    <v-divider class="mb-3" />

    <v-row no-gutters>
      <v-col cols="12">
        <p class="multiline">
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
          :readonly="rfaiRequired ? !rfaiRequired : readOnly"
          @update:model-value="onAnswerInput"
        />
      </v-col>
    </v-row>
    <v-row no-gutters class="pt-5">
      <suspense>
        <ProgramApplicationFileUploader
          :user-files="userFilesFromModel"
          :show-add-file-button="rfaiRequired ? rfaiRequired : !readOnly"
          :max-number-of-files="5"
          :can-delete-permanent-files="rfaiRequired || !readOnly"
          :program-application-id="programApplicationId"
          :component-group-id="componentGroupId"
          :component-id="componentId"
          @update:files="onFilesUpdate"
          @delete:file="handleFileDelete"
        />
      </suspense>
    </v-row>
  </Card>
</template>
<script lang="ts">
import { defineComponent, type PropType } from "vue";
import Card from "@/components/Card.vue";
import ProgramApplicationFileUploader from "@/components/program-application/ProgramApplicationFileUploader.vue";
import type { FileItem } from "@/components/common/UploadFileItem.vue";
import type { Components } from "@/types/openapi";
import * as Rules from "@/utils/formRules";
import { removeElementByIndex } from "@/utils/functions";

export interface QuestionModelValue {
  answer?: string;
  files?: Components.Schemas.FileInfo[];
}

function toFileItem(info: Components.Schemas.FileInfo): FileItem {
  return {
    file: new File([], info.name ?? ""),
    fileId: info.id ?? "",
    shareDocumentUrlId: info.shareDocumentUrlId ?? undefined,
    progress: 101,
    fileErrors: [],
    fileSize: info.size ?? "",
    fileName: info.name ?? "",
    storageFolder: "permanent",
  };
}

export default defineComponent({
  name: "Question",
  components: { Card, ProgramApplicationFileUploader },
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
    hasUnsavedChanges: {
      type: Boolean,
      default: false,
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
  },
  emits: { "update:modelValue": (_payload: QuestionModelValue) => true },
  data() {
    return {
      Rules,
      userFilesFromModel: [] as FileItem[],
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
      const gap = 12; // ga-3 on parent flex row

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

      this.$emit("update:modelValue", {
        ...this.modelValue,
        files: files
          .filter((f) => f.progress === 101 && f.fileErrors.length === 0)
          .map((f) => ({
            id: f.fileId,
            shareDocumentUrlId: f.shareDocumentUrlId,
            name: f.fileName,
          })),
      });
    },
    handleFileDelete(fileItem: FileItem) {
      // Permanent files are deleted immediately by FileUploader via the delete endpoint.
      // Update local state to reflect removal.
      const index = this.userFilesFromModel?.findIndex(
        (file) => file.fileId === fileItem.fileId,
      );
      if (index !== undefined && index >= 0) {
        this.userFilesFromModel = removeElementByIndex(
          this.userFilesFromModel,
          index,
        );
      }
      this.$emit("update:modelValue", {
        ...this.modelValue,
        files: this.userFilesFromModel
          .filter((f) => f.progress === 101 && f.fileErrors.length === 0)
          .map((f) => ({
            id: f.fileId,
            shareDocumentUrlId: f.shareDocumentUrlId,
            name: f.fileName,
          })),
      });
    },
  },
});
</script>
