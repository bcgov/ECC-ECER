<template>
  <v-row>
    <v-col cols="12" md="8" lg="6" xl="4">
      <EceDisplayValue
        label="Institution name"
        :model-value="modelValue.institutionName"
      />
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12" md="8" lg="6" xl="4">
      <EceDateInput
        v-if="isChangeRequest"
        :model-value="modelValue.startDate"
        label="Start date"
        :is-date="true"
        :rules="startDateRules"
        @update:model-value="(v) => $emit('update:modelValue', { ...modelValue, startDate: v })"
      />
      <EceDisplayValue
        v-else
        label="Start date"
        :model-value="modelValue.startDate"
        :is-date="true"
      />
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12" md="8" lg="6" xl="4">
      <EceDisplayValue
        label="End date"
        :model-value="modelValue.endDate"
        :is-date="true"
      />
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12" md="8" lg="6" xl="4">
      <EceDisplayValue
        label="Program types"
        :model-value="modelValue?.programTypes"
      />
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12" md="8" lg="6" xl="4">
      <EceTextField
        :model-value="modelValue.programName"
        label="Program name"
        :rules="[rules.required('Required')]"
        @update:model-value="(v: string) => $emit('update:modelValue', { ...modelValue, programName: v })"
      />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import EceDisplayValue from "@/components/inputs/EceDisplayValue.vue";
import EceDateInput from "@/components/inputs/EceDateInput.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import * as rules from "@/utils/formRules";
import { useProgramStore } from "@/store/program";
import { formatDate } from "@/utils/format";
import {getPrograms} from "@/api/program.ts";

export default defineComponent({
  name: "ProgramOverviewStep",
  components: { EceDisplayValue, EceDateInput, EceTextField },
  props: {
    modelValue: {
      type: Object,
      default: () => ({}),
    },
  },
  emits: ["update:modelValue"],
  data() {
    return {
      rules,
      fromProgramStartDate: null as string | undefined | null,
    };
  },
  setup() {
    const programStore = useProgramStore();
    return { programStore };
  },
  computed: {
    draftProgram() {
      return this.programStore.draftProgram;
    },
    isChangeRequest(): boolean {
      return this.draftProgram?.programProfileType === "ChangeRequest";
    },
    startDateRules(): Array<(v: string) => boolean | string> {
      const start = [this.fromProgramStartDate, this.draftProgram?.createdOn]
        .filter(Boolean)
        .sort((a, b) => new Date(b!).getTime() - new Date(a!).getTime())[0];//take the latest date
      const end = this.draftProgram?.endDate;
      if (!start || !end) return [rules.required("Required")];

      const formattedStart = formatDate(start, "LLLL d, yyyy");
      const formattedEnd = formatDate(end, "LLLL d, yyyy");
      const betweenRule = (v: string) => {
        if (!v) return true;
        return rules.dateBetweenRule(
          start,
          end,
          `Start date must be between ${formattedStart} and ${formattedEnd}`,
        )(v);
      };

      return [rules.required("Required"), betweenRule];
    },
  },
  async mounted() {
    if (this.isChangeRequest && this.programStore.draftProgram?.fromProgramProfileId) {
      await this.fetchFromProgramCreatedOn();
    }
  },
  methods: {
    async fetchFromProgramCreatedOn() {
      const fromProgramProfileId = this.programStore.draftProgram?.fromProgramProfileId;
      if (!fromProgramProfileId) {
        return;
      }
      try {
        const { data: programResult } = await getPrograms(fromProgramProfileId, [
          "Draft",
          "Denied",
          "Approved",
          "UnderReview",
          "ChangeRequestInProgress",
          "Inactive",
        ]);
        const fromProgram = programResult?.programs && programResult.programs.length > 0
          ? programResult.programs[0]
          : null;
        this.fromProgramStartDate = fromProgram?.startDate;
      } catch (error) {
        console.error("Error loading from program:", error);
      }
    },
  }
});
</script>
