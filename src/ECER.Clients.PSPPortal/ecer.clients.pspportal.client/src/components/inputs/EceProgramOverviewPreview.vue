<template>
  <PreviewCard title="Program overview" portal-stage="ProgramOverview">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Institution name</p>
        </v-col>
        <v-col>
          <p id="institutionName" class="small font-weight-bold">
            {{ institutionName }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Start date</p>
        </v-col>
        <v-col>
          <p id="startDate" class="small font-weight-bold">
            {{ startDateFormatted }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">End date</p>
        </v-col>
        <v-col>
          <p id="endDate" class="small font-weight-bold">
            {{ endDateFormatted }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Program types(s)</p>
        </v-col>
        <v-col>
          <p id="programTypes" class="small font-weight-bold">
            {{ programTypes }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Program name</p>
        </v-col>
        <v-col>
          <p id="programName" class="small font-weight-bold">
            {{ programName }}
          </p>
        </v-col>
      </v-row>
    </template>
  </PreviewCard>
</template>
<script lang="ts">
import { defineComponent } from "vue";

import { useProgramStore } from "@/store/program";
import { useWizardStore } from "@/store/wizard";
import { formatDate } from "@/utils/format";

import PreviewCard from "../common/PreviewCard.vue";

export default defineComponent({
  name: "EceProgramOverviewPreview",
  components: {
    PreviewCard,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    const programStore = useProgramStore();
    return {
      wizardStore,
      programStore,
    };
  },
  computed: {
    institutionName() {
      return this.programStore.draftProgram?.postSecondaryInstituteName || "";
    },
    startDateFormatted() {
      return formatDate(
        this.programStore.draftProgram?.startDate || "",
        "LLL d, yyyy",
      );
    },
    endDateFormatted() {
      return formatDate(
        this.programStore.draftProgram?.endDate || "",
        "LLL d, yyyy",
      );
    },
    programTypes() {
      return this.programStore.draftProgram?.programTypes?.join(", ");
    },
    programName() {
      return this.programStore.draftProgram?.programName;
    },
  },
});
</script>
