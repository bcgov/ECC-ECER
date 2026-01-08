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
          <p class="small font-weight-bold">{{ program?.programTypes?.includes(programType) ? "Yes" : "No" }}</p>
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
          v-for="[courseAreaOfInstructionId, courses] in getCoursesBasedOnProgramTypeGroupedByAreaOfInstruction"
          :key="courseAreaOfInstructionId"
        >
          <v-col cols="4">{{ configStore.areaOfInstructionNameById(courseAreaOfInstructionId) || courseAreaOfInstructionId }}</v-col>
          <v-col cols="8">
            <div v-for="course in courses" :key="course.courseName">
              <v-row no-gutters>
                <v-col cols="6">
                  {{ course.courseName }}
                </v-col>
                <v-col cols="6">
                  {{ course.hours }}
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
import { type Components } from "@/types/openapi";

import { useLoadingStore } from "@/store/loading";
import { useConfigStore } from "@/store/config";
import { useRouter } from "vue-router";

interface CourseAreaDetail {
  courseName: string;
  hours: number; // or string, depending on your data source
}

// The Map structure: Key is the ID, Value is the list of course details
type AreaOfInstructionWithCourseHoursMap = Map<string, CourseAreaDetail[]>;

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
    getCoursesBasedOnProgramTypeGroupedByAreaOfInstruction(): AreaOfInstructionWithCourseHoursMap | undefined {
      let filteredCourses = this.program?.courses?.filter((course: Components.Schemas.Course) => course.programType === this.programType); //filter out relevant courses here
      let courseAreaOfInstructionMap = new Map();
      filteredCourses?.forEach((course: Components.Schemas.Course) => {
        course.courseAreaOfInstruction?.forEach((area: Components.Schemas.CourseAreaOfInstruction) => {
          if (courseAreaOfInstructionMap.has(area.areaOfInstructionId)) {
            //areaOfInstructionExists -> append to array
            courseAreaOfInstructionMap.get(area.areaOfInstructionId).push({ courseName: course.courseNumber, hours: area.newHours });
          } else {
            //create new areaOfInstruction key
            courseAreaOfInstructionMap.set(area.areaOfInstructionId, [{ courseName: course.courseNumber, hours: area.newHours }]);
          }
        });
      });
      return courseAreaOfInstructionMap;
    },
  },
});
</script>
