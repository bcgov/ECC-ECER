<template>
  <v-card variant="outlined" rounded="lg">
    <v-card-title>
      <div class="d-flex justify-space-between align-center">
        <div>
          <h2 class="text-wrap">{{ generateTitle }}</h2>
        </div>
      </div>
    </v-card-title>
    <v-card-text class="text-grey-dark">
      <v-row class="mb-4" no-gutters>
        <v-col cols="4">
          <p class="small">Program is offered</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">
            {{ program?.programTypes?.includes(programType) ? "Yes" : "No" }}
          </p>
        </v-col>
      </v-row>
      <template v-if="program?.programTypes?.includes(programType)">
        <v-row class="mb-4" no-gutters>
          <v-col cols="4">Area of Instruction</v-col>
          <v-col cols="4">Course number and name</v-col>
          <v-col cols="4">hours</v-col>
        </v-row>
        <v-row
          no-gutters
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
    </v-card-text>
  </v-card>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import { useDisplay } from "vuetify";
import type { Components } from "@/types/openapi";
import { getCoursesBasedOnProgramTypeGroupedByAreaOfInstruction } from "@/utils/functions";

import { useLoadingStore } from "@/store/loading";
import { useConfigStore } from "@/store/config";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "ProgramDetailAreaOfInstructionCard",
  components: {},
  setup: async () => {
    const loadingStore = useLoadingStore();
    const configStore = useConfigStore();
    const { mdAndDown, mobile } = useDisplay();
    const router = useRouter();

    return {
      configStore,
      loadingStore,
      mdAndDown,
      mobile,
      router,
    };
  },

  props: {
    program: {
      type: Object as PropType<Components.Schemas.Program>,
      required: true,
      default: () => ({}),
    },
    programType: {
      type: String as PropType<Components.Schemas.ProgramTypes>,
      required: true,
    },
  },
  computed: {
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
        this.program,
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
