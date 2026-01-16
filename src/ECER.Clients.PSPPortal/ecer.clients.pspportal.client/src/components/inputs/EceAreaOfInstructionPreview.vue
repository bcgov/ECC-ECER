<template>
  <PreviewCard :title="getTitle" :portal-stage="getPortalStage">
    <template #content>
      <v-row class="mb-4">
        <v-col cols="4">
          <p class="small">Program is offered</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">
            {{
              programStore.draftProgram?.programTypes?.includes(programType)
                ? "Yes"
                : "No"
            }}
          </p>
        </v-col>
      </v-row>
      <template
        v-if="programStore.draftProgram?.programTypes?.includes(programType)"
      >
        <v-row
          class="mb-4"
          v-for="[
            courseAreaOfInstructionId,
            courses,
          ] in getCoursesBasedOnProgramTypeGroupedByAreaOfInstruction"
          :key="courseAreaOfInstructionId"
        >
          <v-col cols="4">
            {{
              configStore.areaOfInstructionNameById(
                courseAreaOfInstructionId,
              ) || courseAreaOfInstructionId
            }}
          </v-col>
          <v-col cols="8">
            <div v-for="course in courses">
              <v-row no-gutters>
                <v-col cols="6">
                  <strong>{{ getCourseName(course) }}</strong>
                </v-col>
                <v-col cols="6">
                  <strong>{{ course.hours }}</strong>
                </v-col>
              </v-row>
            </div>
          </v-col>
        </v-row>
      </template>
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { Components } from "@/types/openapi";
import type { ProgramStage } from "@/types/wizard";
import { getCoursesBasedOnProgramTypeGroupedByAreaOfInstruction } from "@/utils/functions";

import { useConfigStore } from "@/store/config";
import { useProgramStore } from "@/store/program";

import PreviewCard from "../common/PreviewCard.vue";

export default defineComponent({
  name: "EceAreaOfInstructionPreview",
  components: {
    PreviewCard,
  },
  props: {
    programType: {
      type: String as PropType<Components.Schemas.ProgramTypes>,
      required: true,
    },
  },
  setup: () => {
    const configStore = useConfigStore();
    //using programStore for data since technically changes to courses are separate from the wizardFlow.
    //we have a separate endpoint for course edits. ProgramStore.draftProgram will have the updated information to display.
    const programStore = useProgramStore();

    return { configStore, programStore };
  },
  computed: {
    getPortalStage(): ProgramStage {
      switch (this.programType) {
        case "Basic":
          return "EarlyChildhood";
        case "ITE":
          return "InfantAndToddler";
        case "SNE":
          return "SpecialNeeds";
      }
    },
    getTitle(): string {
      switch (this.programType) {
        case "Basic":
          return "Basic Early Childhood Educator";
        case "ITE":
          return "Infant and Toddler Educator";
        case "SNE":
          return "Special Needs Educator";
      }
    },
    generateTitle(): string {
      switch (this.programType) {
        case "Basic":
          return "Basic Early Childhood Educator";
        case "ITE":
          return "Infant and Toddler Educator";
        case "SNE":
          return "Special Needs Educator";
        default:
          return "Unknown Program Type";
      }
    },
    // this method will return a Map that looks like this {Key = AreaOfInstructionId, Values = Array of courses with name and hours}
    getCoursesBasedOnProgramTypeGroupedByAreaOfInstruction():
      | AreaOfInstructionWithCourseHoursMap
      | undefined {
      return getCoursesBasedOnProgramTypeGroupedByAreaOfInstruction(
        this.programStore.draftProgram,
        this.programType,
      );
    },
  },
  methods: {
    getCourseName(course: CourseAreaDetail): string {
      if (course.courseNumber && course.courseTitle) {
        return `${course.courseNumber} - ${course.courseTitle}`;
      }
      if (course.courseNumber) {
        return course.courseNumber;
      }
      if (course.courseTitle) {
        return course.courseTitle;
      }

      return "";
    },
  },
});
</script>
