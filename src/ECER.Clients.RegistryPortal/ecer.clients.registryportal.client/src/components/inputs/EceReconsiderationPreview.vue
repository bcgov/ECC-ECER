<template>
  <PreviewCard
    title="Dispute details"
    portal-stage="Reconsideration"
    :editable="true"
  >
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Submit dispute by</p>
        </v-col>
        <v-col>
          <strong>{{ formattedSubmitDisputeByDate }}</strong>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Type of decision</p>
        </v-col>
        <v-col>
          <strong>{{ mapReconsiderationToTypeOfDecision() }}</strong>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Dispute explanation</p>
        </v-col>
        <v-col>
          <p class="font-weight-bold small">{{ disputeExplanation }}</p>
        </v-col>
      </v-row>
      <v-row v-if="wizardStore.wizardData.reconsideration.files.length > 0">
        <v-col cols="4">
          <p class="small">
            <v-icon>mdi-attachment</v-icon>
            Attached files
          </p>
        </v-col>
        <v-col>
          <p
            class="font-weight-bold small"
            v-for="(file, index) in files"
            :key="index"
          >
            {{ file.name }}
          </p>
        </v-col>
      </v-row>
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useWizardStore } from "@/store/wizard";
import { formatDate } from "@/utils/format";
import type { Components } from "@/types/openapi";
import { mapReconsiderationTypeToTypeOfDecision } from "@/api/reconsideration";
import { mapInvestigationReconsiderationTypeToTypeOfDecision } from "@/api/investigationReconsideration.ts";
export default defineComponent({
  name: "EceReconsiderationPreview",
  components: {
    PreviewCard,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    return {
      wizardStore,
    };
  },
  methods: {
    mapReconsiderationToTypeOfDecision() {
      const reconsideration = this.wizardStore.wizardData[
        this.wizardStore?.wizardConfig?.steps?.reconsideration?.form?.inputs
          ?.reconsideration?.id || ""
      ] as
        | Components.Schemas.Reconsideration
        | Components.Schemas.InvestigationReconsideration;

      if (!reconsideration) {
        console.warn(
          "reconsideration not found check the wizard configuration",
        );
        return "";
      }

      if (
        reconsideration.type === "ImmediateAction" ||
        reconsideration.type === "InvestigationOutcome"
      ) {
        return mapInvestigationReconsiderationTypeToTypeOfDecision(
          reconsideration,
        );
      }
      //it's a regular reconsideration request just use reconsideration mappings
      return mapReconsiderationTypeToTypeOfDecision(reconsideration);
    },
  },
  computed: {
    disputeExplanation() {
      return this.wizardStore.wizardData[
        this.wizardStore?.wizardConfig?.steps?.reconsideration?.form?.inputs
          ?.reconsideration?.id || ""
      ].explanationAndEvidence;
    },
    formattedSubmitDisputeByDate() {
      return formatDate(
        this.wizardStore.wizardData[
          this.wizardStore?.wizardConfig?.steps?.reconsideration?.form?.inputs
            ?.reconsideration?.id || ""
        ].reconsiderationEndDate || "",
        "LLLL d, yyyy",
      );
    },
    files(): Components.Schemas.FileInfo[] {
      return this.wizardStore.wizardData[
        this.wizardStore?.wizardConfig?.steps?.reconsideration?.form?.inputs
          ?.reconsideration?.id || ""
      ].files;
    },
  },
});
</script>
