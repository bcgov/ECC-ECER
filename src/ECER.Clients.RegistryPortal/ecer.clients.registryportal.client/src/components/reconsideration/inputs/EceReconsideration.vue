<template>
  <h2>General Information</h2>
  <br />
  <v-row>
    <v-col cols="3">Submit dispute by</v-col>
    <v-col>
      <strong>
        {{
          formatDate(modelValue.reconsiderationEndDate || "", "LLLL d, yyyy")
        }}
      </strong>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="3">Type of decision</v-col>
    <v-col>
      <strong>Intent to deny application</strong>
    </v-col>
  </v-row>
  <br />
  <h2>Explanation and evidence</h2>
  <br />
  <p>
    Provide the reason or reasons you are disputing the outcome of the ECE
    Registry's assessment or investigation. Include an explanation to support
    your reason(s) for disputing the outcome of the assessment or investigation.
    If applicable, include any sections of the CCALA or CCLR that you believe
    are relevant. If you are attaching additional documents as evidence, explain
    why they are relevant to your request.
  </p>
  <br />
  <h3>Dispute explanation</h3>
  <v-textarea
    id="explanationAndEvidence"
    :rules="[Rules.required('Enter your response')]"
    counter="5000"
    variant="outlined"
    color="primary"
    maxlength="5000"
    persistent-counter
    hide-details="auto"
    :auto-grow="true"
    @update:model-value="updateField('explanationAndEvidence', $event)"
  ></v-textarea>
  <FileUploader
    ref="fileUploader"
    :max-number-of-files="maxNumberOfFiles"
    @update:files="handleFileUpdate"
  />
</template>
<script lang="ts">
import { defineComponent } from "vue";
import type { PropType } from "vue";
import * as Rules from "@/utils/formRules";
import type { Components } from "@/types/openapi";
import { humanFileSize } from "@/utils/functions";
import { formatDate } from "@/utils/format";

import FileUploader from "../../FileUploader.vue";

const maxNumberOfFiles = 10;

export default defineComponent({
  name: "EceReconsideration",
  components: {
    FileUploader,
  },
  setup() {
    return { Rules, maxNumberOfFiles };
  },
  props: {
    modelValue: {
      type: Object as PropType<Components.Schemas.Reconsideration>,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_disputeData: Components.Schemas.Reconsideration) =>
      true,
  },
  data() {
    return {};
  },
  methods: {
    formatDate,
    updateField(
      fieldName: keyof Components.Schemas.Reconsideration,
      value: string,
    ) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: value,
      });
    },
    handleFileUpdate(filesArray: any[]) {
      let refFileUploader = this.$refs.fileUploader as typeof FileUploader;
      let areAttachedFilesValid = !refFileUploader.fileErrors;
      let isFileUploadInProgress = refFileUploader.filesInProgress;
      let files = []; // Reset attachments
      if (filesArray && filesArray.length > 0) {
        for (let i = 0; i < filesArray.length; i++) {
          const file = filesArray[i];

          // If file is valid and fully uploaded, add to attachments
          if (areAttachedFilesValid && !isFileUploadInProgress) {
            files.push({
              id: file.fileId,
              name: file.file.name,
              size: humanFileSize(file.file.size),
              extention: file.file.name.split(".").pop(),
            });
          }
        }
        this.$emit("update:model-value", { ...this.modelValue, files: files });
      }
    },
  },
});
</script>
