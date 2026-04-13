<template>
  <Loading v-if="loadingStore.isLoading('courses_get')" />
  <template v-else-if="canEditProgramProfile">
    <h2>Program profile - ECE ({{ programType }})</h2>
    <br />
    <p>This page will build your program profile with the ECE Registry</p>
    <br />
    <h2>Areas of instruction</h2>
    <br />
    <p>
      You will need to input all courses in this program and allocate course
      hours to each area of instruction.
    </p>
    <br />
    <h3>Adding a course</h3>
    <br />
    <p>To add a course to your program profile:</p>
    <ul class="ml-10">
      <li>Select "Add course"</li>
      <li>Input the course number and course name</li>
      <li>
        For each course, input the number of hours the course satisfies towards
        each area of instruction (in some cases, one course may be applicable to
        more than one area of instruction)
      </li>
    </ul>
    <br />
    <h3>Updating a course</h3>
    <br />
    <p>To edit a course on your program profile:</p>
    <ul class="ml-10">
      <li>Select the pencil icon beside the course name</li>
      <li>Make any changes and click "Save"</li>
      <li>If you need to delete a course, select the trashcan icon</li>
    </ul>
    <br />
    <p>
      Note: If you do not input any hours for a course, it will be shown in the
      "Non-allocated courses" section on this page
    </p>
    <br />
    <AreaOfInstructionComponent
      :courses="courses ?? []"
      :id="programApplicationId"
      type="ProgramApplication"
      :program-type="programType"
      :include-total-hours="showTotalHours"
      :area-subtitles="generateSubtitleMap"
      @reload-courses="loadCourses"
    ></AreaOfInstructionComponent>
    <v-row class="mt-4">
      <v-col>
        <v-btn color="primary" @click="saveAndContinue">
          Save and continue
        </v-btn>
      </v-col>
    </v-row>
  </template>
  <Alert v-else class="mt-10" type="error">
    <p class="small">This application type does not have access to this page</p>
  </Alert>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useConfigStore } from "@/store/config";
import { useLoadingStore } from "@/store/loading";

import type { Components } from "@/types/openapi";
import type { NextStepPayload } from "@/components/program-application/ProgramApplication.vue";
import { formatDate } from "@/utils/format";
import { getCourses } from "@/api/course";

import * as Rules from "@/utils/formRules";
import { MIN_HOURS_ITE_SNE } from "@/utils/constant";

import Loading from "@/components/Loading.vue";
import AreaOfInstructionComponent from "../../program-profile/AreaOfInstructionComponent.vue";
import Alert from "@/components/Alert.vue";

export default defineComponent({
  name: "EceProgramAreaInput",
  components: {
    Alert,
    Loading,
    AreaOfInstructionComponent,
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
    applicationType: {
      type: String,
      required: true,
    },
  },
  emits: { next: (_payload: NextStepPayload) => true },
  setup: () => {
    const configStore = useConfigStore();
    const loadingStore = useLoadingStore();

    return {
      configStore,
      loadingStore,
    };
  },
  data() {
    return {
      filteredAreasOfInstruction: [] as Components.Schemas.AreaOfInstruction[],
      Rules,
      MIN_HOURS_ITE_SNE,
      programApplication: null as Components.Schemas.ProgramApplication | null,
      courses: [] as Components.Schemas.Course[] | undefined,
      canEditProgramProfile: true,
    };
  },
  async mounted() {
    if (this.applicationType === "NewBasicECEPostBasicProgram") {
      await this.loadCourses();
      this.filteredAreasOfInstruction =
        this.configStore.areaOfInstructionList.filter((area) =>
          area.programTypes?.includes(this.programType),
        );
    } else {
      this.canEditProgramProfile = false;
    }
  },
  watch: {
    programType: "loadCourses",
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
      const subtitleMap: Record<string, string> = {};
      this.configStore.areaOfInstructionList
        .filter((area) => area.parentAreaOfInstructionId != null)
        .forEach((area) => {
          const parentArea = this.configStore.areaOfInstructionList.find(
            (a) => a.id === area.parentAreaOfInstructionId,
          );
          if (area.id && parentArea?.name) {
            subtitleMap[area.id] =
              `${area.name} is included in ${parentArea.name}.`;
          }
        });
      return subtitleMap;
    },
  },
  methods: {
    formatDate,
    async loadCourses() {
      this.courses = await getCourses(
        this.programApplicationId,
        "ProgramApplication",
        [this.programType],
      );
    },
    saveAndContinue() {
      this.$emit("next", { programType: this.programType });
    },
  },
});
</script>
