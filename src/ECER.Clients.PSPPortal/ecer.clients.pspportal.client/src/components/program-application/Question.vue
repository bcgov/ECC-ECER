<template>
  <v-card class="px-5">
    <v-card-title class="pl-0 pr-0">
      {{ name }}
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
        />
      </v-col>
    </v-row>
    <v-row no-gutters class="pt-5">
      <suspense>
        <FileUploader
          :user-files="userFilesFromModel"
          :show-add-file-button="true"
          :max-number-of-files="5"
          :can-delete-permanent-files="false"
          @update:files="onFilesUpdate"
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

export interface QuestionModelValue {
  answer?: string;
  files?: Components.Schemas.FileInfo[];
}

function toFileItem(info: Components.Schemas.FileInfo): FileItem {
  return {
    file: new File([], info.name ?? ""),
    fileId: info.id ?? "",
    progress: 101,
    fileErrors: [],
    fileSize: 0,
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
      default: () => ({ answer: "", files: [] }),
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
  },
  emits: ["update:modelValue"],
  data() {
    return {
      Rules,
      userFilesFromModel: [] as FileItem[],
    };
  },
  created() {
    if (this.modelValue?.files?.length) {
      this.userFilesFromModel = this.modelValue.files.map(toFileItem);
    }
  },
  methods: {
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
          .filter((f) => f.storageFolder === "permanent")
          .map((f) => ({ id: f.fileId, name: f.fileName })),
      });
    },
  },
});
</script>
