<template>
    <v-card 
      flat
      rounded="lg"
      :border="true"
      color="support-surface-info"
      class="border-support-border-info"
    >
      <v-card-text>
        <div
          v-for="(areaGroup, areaIndex) in groupedAreas"
          :key="areaGroup.areaOfInstructionId"
        >
          <v-card-title class="pl-0">{{ areaGroup.areaName }}</v-card-title>
          <v-card-subtitle v-if="areaGroup.subtitle" class="pl-0 pb-4">{{ areaGroup.subtitle }}</v-card-subtitle>
          <div v-if="areaIndex === 0 && showProgressBar" class="mb-4 d-flex align-center">
            <v-progress-linear
              :model-value="overallProgressPercentage"
              height="25"
              rounded
              rounded-bar
              :color="overallProgressColor"
              bg-color="#f0f2f4"
              class="flex-grow-1"
            >
              <template v-slot:default="{ value }">
                <span><strong>{{ overallTotalHours }}</strong> total hours / <strong>{{ areaGroup.minimumHours }}</strong> required hours</span>
              </template>
            </v-progress-linear>
            <v-icon
              v-if="overallProgressPercentage >= 100"
              icon="mdi-check"
              :color="overallProgressColor"
              size="28"
              class="ml-2"
            ></v-icon>
          </div>
          <div v-if="areaGroup.courses.some((c: CourseAreaOfInstructionWithCourse) => (c.courseNumber || c.courseTitle))">
            <template
              v-for="(courseArea, index) in areaGroup.courses"
              :key="courseArea.courseAreaOfInstructionId || index"
            >
              <v-row
                v-if="(courseArea.courseNumber || courseArea.courseTitle)"
                no-gutters
                align="center"
                class="pl-4 mb-2 bg-white rounded-lg border"
              >
              <v-col cols="8">
                <span class="font-weight-bold">
                  {{ getCourseTitle(courseArea) }}
                </span>
              </v-col>
              <v-col>
                <span>{{ courseArea.newHours }} hours</span>
              </v-col>
              <v-col cols="auto" class="d-flex">
                <v-divider vertical></v-divider>
                <v-btn
                  icon="mdi-pencil"
                  variant="plain"
                  @click="handleEdit(courseArea)"
                ></v-btn>
              </v-col>
            </v-row>
            </template>
          </div>
          
          <p v-else class="text-grey-darken-1 mb-4">No courses added yet for this area.</p>
        </div>
        <v-row v-if="groupedAreas.length > 0" no-gutters class="pl-4 pt-2 font-weight-bold">
          <v-col cols="8">
            <span>Total hours</span>
          </v-col>
          <v-col>
            <span class="font-weight-bold">{{ overallTotalHours }} hours</span>
          </v-col>
        </v-row>
        
        <p v-if="groupedAreas.length === 0" class="text-grey-darken-1">No courses added yet.</p>
      </v-card-text>
    </v-card>
  </template>
  
  <script lang="ts">
  import { defineComponent, type PropType } from "vue";
  import type { Components } from "@/types/openapi";
  import { useConfigStore } from "@/store/config";
  
  interface CourseAreaOfInstructionWithCourse extends Components.Schemas.CourseAreaOfInstruction {
    courseTitle?: string | null;
    courseNumber?: string | null;
  }

  interface AreaGroup {
    areaOfInstructionId: string;
    areaName: string;
    subtitle: string;
    minimumHours: number;
    courses: CourseAreaOfInstructionWithCourse[];
    totalHours: number;
    progressPercentage: number;
    progressColor: string;
  }
  
  export default defineComponent({
    name: "AreaOfInstructionCard",
    components: {},
    setup() {
      const configStore = useConfigStore();
      return {
        configStore,
      };
    },
    props: {
      courseAreaOfInstructions: {
        type: Array as PropType<CourseAreaOfInstructionWithCourse[]>,
        required: true,
        default: () => [],
      },
      areaSubtitles: {
        type: Object as PropType<Record<string, string>>,
        required: false,
        default: () => ({}),
      },
      showProgressBar: {
        type: Boolean,
        required: false,
        default: true,
      },
      areaId: {
        type: String,
        required: false,
        default: null,
      },
    },
    emits: ["edit"],
    computed: {
      groupedAreas(): AreaGroup[] {
        // Group courses by areaOfInstructionId (filter out courses without areaOfInstructionId)
        const grouped = new Map<string, CourseAreaOfInstructionWithCourse[]>();
        this.courseAreaOfInstructions.forEach((course) => {
          // Only process courses with a valid areaOfInstructionId
          const areaId = course.areaOfInstructionId;
          if (areaId && areaId.trim()) {
            if (!grouped.has(areaId)) {
              grouped.set(areaId, []);
            }
            grouped.get(areaId)!.push(course);
          }
        });

        // If areaId prop is provided, ensure that area is always included even with no courses
        if (this.areaId && !grouped.has(this.areaId)) {
          grouped.set(this.areaId, []);
        }

        // Convert to array with computed values for each area
        return Array.from(grouped.entries()).map(([areaId, courses]) => {
          const areaOfInstruction = this.configStore.areaOfInstructionList.find(
            (area) => area.id === areaId
          );
          const areaName = areaOfInstruction?.name || areaId;
          // Use provided subtitle from prop, or fallback to default based on minimum hours
          const subtitle = this.areaSubtitles[areaId] || 
            (areaOfInstruction?.minimumHours ? `A minimum of ${areaOfInstruction.minimumHours} is required.` : "");
          const minimumHours = areaOfInstruction?.minimumHours || 0;
          
          const totalHours = courses.reduce((sum, courseArea) => {
            const hours = Number.parseFloat(courseArea.newHours || "0");
            return sum + hours;
          }, 0);

          const progressPercentage = minimumHours === 0 
            ? 0 
            : Math.min((totalHours / minimumHours) * 100, 100);
          
          const progressColor = progressPercentage >= 100 ? "#66CB7B" : "#FACC75";

          return {
            areaOfInstructionId: areaId,
            areaName,
            subtitle,
            minimumHours,
            courses,
            totalHours,
            progressPercentage,
            progressColor,
          };
        });
      },
      overallTotalHours(): number {
        return this.courseAreaOfInstructions.reduce((sum, courseArea) => {
          const hours = Number.parseFloat(courseArea.newHours || "0");
          return sum + hours;
        }, 0);
      },
      overallProgressPercentage(): number {
        if (this.groupedAreas.length === 0) return 0;
        const firstArea = this.groupedAreas[0];
        if (!firstArea) return 0;
        const minimumHours = firstArea.minimumHours;
        if (minimumHours === 0) return 0;
        const percentage = (this.overallTotalHours / minimumHours) * 100;
        return Math.min(percentage, 100); // Cap at 100%
      },
      overallProgressColor(): string {
        return this.overallProgressPercentage >= 100 ? "#66CB7B" : "#FACC75";
      },
    },
    methods: {
      getCourseTitle(courseArea: CourseAreaOfInstructionWithCourse): string {
        const courseNumber = courseArea.courseNumber || "";
        const courseTitle = courseArea.courseTitle || "";
        
        if (courseNumber && courseTitle) {
          return `${courseNumber} - ${courseTitle}`;
        } else if (courseNumber) {
          return courseNumber;
        } else if (courseTitle) {
          return courseTitle;
        } else {
          return "Untitled Course";
        }
      },
      handleEdit(courseArea: Components.Schemas.CourseAreaOfInstruction) {
        this.$emit("edit", courseArea);
      },
    },
  });
  </script>
  
  <style scoped>
    :deep(.v-progress-linear__background) {
      opacity: 1;
    }
  </style>
