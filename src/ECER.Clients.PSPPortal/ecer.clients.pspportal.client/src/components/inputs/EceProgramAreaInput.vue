<template>
  <h2>Program offering</h2>
  <br />
  <p>
    This program profile is for the following time period:
    <strong>{{ `${formatDate(wizardStore.wizardData?.startDate, "LLLL d, yyyy")} - ${formatDate(wizardStore.wizardData?.endDate, "LLLL d, yyyy")}` }}</strong>
  </p>
  <br />
  <p>
    Will your educational institution offer the
    <strong>{{ generateProgramTypeTitle }}</strong>
    program during this time period?
  </p>
  <v-radio-group v-model="programOffered" :rules="[Rules.requiredRadio()]" @update:model-value="(value) => $emit('update:model-value', value as boolean)">
    <v-radio label="Yes" :value="true"></v-radio>
    <v-radio label="No" :value="false"></v-radio>
  </v-radio-group>
  <Callout v-if="!programOffered" type="warning">
    <h3>You may continue to the next page</h3>
    <p>As you are not offering this program at this time, you do not have to update the course hours for this {{ programType }} program.</p>
    <br />
    <p>Press continue to move through to the next page.</p>
  </Callout>
  <template v-else>
    <h2>Provincial requirements</h2>
    <br />
    <ul v-if="programType === 'Basic'" class="ml-10">
      <li>Basic ECE education must total a minimum of {{ calculateMinimumHoursRequired }} hours, including practicum</li>
      <li>Practicum must account for a minimum of {{ calculatePracticumHours }} hours</li>
      <li>Each area of instruction has a minimum number of required course hours</li>
    </ul>
    <ul v-else-if="programType === 'ITE'" class="ml-10">
      <li>ITE education must total a minimum of {{ calculateMinimumHoursRequired }} hours, including practicum</li>
      <li>Practicum must account for a minimum of {{ calculatePracticumHours }} hours</li>
    </ul>
    <ul v-else-if="programType === 'SNE'" class="ml-10">
      <li>SNE education must total a minimum of {{ calculateMinimumHoursRequired }} hours, including practicum</li>
      <li>Practicum must account for a minimum of {{ calculatePracticumHours }} hours</li>
    </ul>
    <br />
    <p>
      For a detailed description of Provincial requirements, refer to
      <a href="#" target="_blank">Table 1, 2 or 3 of the Child Care Occupational Competencies.</a>
    </p>
    <br />
    <h2>Required areas of instruction</h2>
    <br />
    <p>The courses included in your program are shown here, grouped by areas of instruction.</p>
    <br />
    <p>
      Edit any courses as required to ensure that this program profile reflects the correct course information. The following information is editable: course
      number, course name, course hours allocated to each area of instruction. In some cases, a course may be applicable to more than one area of instruction.
    </p>
  </template>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useWizardStore } from "@/store/wizard";
import { useConfigStore } from "@/store/config";

import type { Components } from "@/types/openapi";
import { formatDate } from "@/utils/format";

import * as Rules from "@/utils/formRules";

import Callout from "../common/Callout.vue";

export default defineComponent({
  name: "EceProgramAreaInput",
  components: { Callout },
  props: {
    modelValue: {
      type: Boolean,
      required: true,
    },
    programType: {
      type: String as () => Components.Schemas.ProgramTypes,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_model_value: boolean) => true,
    updatedValidation: (_errorState: boolean) => true,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    const configStore = useConfigStore();

    return {
      configStore,
      wizardStore,
    };
  },
  data() {
    return {
      programOffered: this.modelValue,
      filteredAreasOfInstruction: [] as Components.Schemas.AreaOfInstruction[],
      Rules,
    };
  },
  mounted() {
    this.filteredAreasOfInstruction = this.configStore.areaOfInstructionList.filter((area) => area.programTypes?.includes(this.programType));
  },
  computed: {
    generateProgramTypeTitle(): string {
      switch (this.programType) {
        case "Basic":
          return "Basic Early Childhood Educator";
        case "ITE":
          return "Infant and Toddler Educator";
        case "SNE":
          return "Special Needs Educator";
        default:
          return "Unknown program type";
      }
    },
    calculatePracticumHours(): number {
      return this.filteredAreasOfInstruction.filter((area) => area.name === "Practicum").reduce((total, area) => total + (area?.minimumHours || 0), 0);
    },
    calculateMinimumHoursRequired(): number {
      return this.filteredAreasOfInstruction.reduce((total, area) => total + (area?.minimumHours || 0), 0);
    },
  },
  methods: {
    formatDate,
  },
});
</script>
