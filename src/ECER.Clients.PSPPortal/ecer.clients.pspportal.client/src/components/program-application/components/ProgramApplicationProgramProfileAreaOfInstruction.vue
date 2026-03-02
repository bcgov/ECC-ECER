<template>
  <h2>Provincial requirements</h2>
  <br />
  <ul v-if="programType === 'Basic'" class="ml-10">
    <li>
      Basic ECE education must total a minimum of
      {{ calculateMinimumHoursRequired }} hours, including practicum
    </li>
    <li>
      Practicum must account for a minimum of
      {{ calculatePracticumHours }} hours
    </li>
    <li>
      Each area of instruction has a minimum number of required course hours
    </li>
  </ul>
  <ul v-else-if="programType === 'ITE'" class="ml-10">
    <li>
      ITE education must total a minimum of {{ MIN_HOURS_ITE_SNE }} hours,
      including practicum
    </li>
    <li>Practicum must account for a minimum of 200 hours</li>
  </ul>
  <ul v-else-if="programType === 'SNE'" class="ml-10">
    <li>
      SNE education must total a minimum of {{ MIN_HOURS_ITE_SNE }} hours,
      including practicum
    </li>
    <li>Practicum must account for a minimum of 200 hours</li>
  </ul>
  <br />
  <p>
    For a detailed description of Provincial requirements, refer to
    <a
      href="https://www2.gov.bc.ca/assets/gov/education/early-learning/teach/ece/bc_occupational_competencies.pdf"
      target="_blank"
    >
      Table 1, 2 or 3 of the Child Care Occupational Competencies.
    </a>
  </p>
  <br />
  <AreaOfInstructionComponent
    :program="programStore.draftProgram"
    :program-type="programType"
    :include-total-hours="showTotalHours"
    :area-subtitles="generateSubtitleMap"
    @reload-program="reloadProgram"
  >
    <template #description>
      <p>
        The courses included in your program are shown here, grouped by areas of
        instruction.
      </p>
      <br />
      <p>
        Edit any courses as required to ensure that this program profile
        reflects the correct course information. The following information is
        editable: course number, course name, course hours allocated to each
        area of instruction. In some cases, a course may be applicable to more
        than one area of instruction.
      </p>
    </template>
  </AreaOfInstructionComponent>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useConfigStore } from "@/store/config";

import type { Components } from "@/types/openapi";
import { formatDate } from "@/utils/format";
import { getPrograms } from "@/api/program";

import * as Rules from "@/utils/formRules";
import { MIN_HOURS_ITE_SNE } from "@/utils/constant";

// import Callout from "../common/Callout.vue";
// import AreaOfInstructionComponent from "../program-profile/AreaOfInstructionComponent.vue";

export default defineComponent({
  name: "EceProgramAreaInput",
  components: {
    /*Callout, AreaOfInstructionComponent*/
  },
  props: {
    programType: {
      type: String as () => Components.Schemas.ProgramTypes,
      required: true,
    },
    programApplicationId: {
      type: String,
      required: true,
    },
  },
  emits: {},
  setup: () => {
    const configStore = useConfigStore();

    return {
      configStore,
    };
  },
  data() {
    return {
      filteredAreasOfInstruction: [] as Components.Schemas.AreaOfInstruction[],
      Rules,
      MIN_HOURS_ITE_SNE,
      programApplication: null as Components.Schemas.ProgramApplication | null,
    };
  },
  mounted() {
    this.filteredAreasOfInstruction =
      this.configStore.areaOfInstructionList.filter((area) =>
        area.programTypes?.includes(this.programType),
      );
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
      return this.filteredAreasOfInstruction
        .filter((area) => area.name === "Practicum")
        .reduce((total, area) => total + (area?.minimumHours || 0), 0);
    },
    calculateMinimumHoursRequired(): number {
      return this.filteredAreasOfInstruction.reduce(
        (total, area) => total + (area?.minimumHours || 0),
        0,
      );
    },
    showTotalHours(): boolean {
      switch (this.programType) {
        case "Basic":
          return false;
        case "ITE":
        case "SNE":
          return true;
        default:
          return false;
      }
    },
    generateSubtitleMap(): Record<string, string> {
      //specific subtitle only for Area of Instruction Child Guidance for ProgramType Basic
      const childGuidanceAreaOfInstruction =
        this.configStore.areaOfInstructionList.filter(
          (area) =>
            area.name === "Child Guidance" &&
            area.programTypes?.includes("Basic"),
        );

      if (
        childGuidanceAreaOfInstruction &&
        childGuidanceAreaOfInstruction.length > 0
      ) {
        const childGuidanceAreaOfInstructionId =
          childGuidanceAreaOfInstruction[0]?.id;

        return {
          [childGuidanceAreaOfInstructionId!]:
            "Child guidance is included in Program Development, Curriculum and Foundations.",
        };
      }
      console.warn(
        "Child Guidance area of instruction not found for Basic program type.",
      );
      return {};
    },
  },
  methods: {
    formatDate,
    async reloadProgram() {
      console.warn("Not implemented yet");
      //   const programId = this.programStore.draftProgram?.id;
      //   if (!programId) {
      //     return;
      //   }
      //   try {
      //     const { data: programResult } = await getPrograms(programId, [
      //       "Draft",
      //       "Denied",
      //       "Approved",
      //       "UnderReview",
      //       "ChangeRequestInProgress",
      //       "Inactive",
      //     ]);
      //     const program =
      //       programResult?.programs && programResult.programs.length > 0
      //         ? programResult.programs[0]
      //         : null;
      //     if (program) {
      //       // Update the store with the reloaded program
      //       this.programStore.setDraftProgramFromProfile(program);
      //     }
      //   } catch (error) {
      //     console.error("Error loading program:", error);
      //   }
    },
  },
});
</script>
