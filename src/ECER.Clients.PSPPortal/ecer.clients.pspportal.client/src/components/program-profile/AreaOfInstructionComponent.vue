<template>
  <div>
    <h2 class="mb-4">Required areas of instruction</h2>
    <v-row v-if="includeTotalHours" justify="center" class="mb-4">
      <v-col cols="12" :md="10">
        <TotalHoursOfInstructionCard
          :total-hours="totalHours"
          :required-hours="requiredHours"
        />
      </v-col>
    </v-row>

    <div v-if="$slots.description" class="mb-4">
      <slot name="description"></slot>
    </div>

    <AreaOfInstructionCard
      v-for="(area, index) in filteredAreas"
      :key="area.id || undefined"
      class="mb-4"
      :course-area-of-instructions="getCoursesForArea(area.id)"
      :area-subtitles="getAreaSubtitles(area.id)"
      :area-id="area.id || undefined"
      :show-progress-bar="(area.minimumHours && area.minimumHours > 0) || false"
      @edit="handleEdit"
    />

    <NonAllocatedCoursesCard
      v-if="nonAllocatedCourses.length > 0"
      :courses="nonAllocatedCourses"
      @edit="handleEdit"
    />
  </div>
  <!-- this is to block the user from progressing if hours are not met -->
  <v-input :rules="generateRulesByProgramType()" :max-errors="5"></v-input>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { Components } from "@/types/openapi";
import { useConfigStore } from "@/store/config";
import { getAreaOfInstructionList } from "@/api/configuration";
import AreaOfInstructionCard from "./AreaOfInstructionCard.vue";
import NonAllocatedCoursesCard from "./NonAllocatedCoursesCard.vue";
import TotalHoursOfInstructionCard from "./TotalHoursOfInstructionCard.vue";

const MIN_HOURS_ITE_SNE = 450;

interface CourseAreaOfInstructionWithCourse
  extends Components.Schemas.CourseAreaOfInstruction {
  courseTitle?: string | null;
  courseNumber?: string | null;
}

export default defineComponent({
  name: "AreaOfInstructionComponent",
  components: {
    AreaOfInstructionCard,
    NonAllocatedCoursesCard,
    TotalHoursOfInstructionCard,
  },
  props: {
    programType: {
      type: String as PropType<Components.Schemas.ProgramTypes>,
      required: true,
    },
    program: {
      type: Object as PropType<Components.Schemas.Program>,
      required: true,
    },
    areaSubtitles: {
      type: Object as PropType<Record<string, string>>,
      required: false,
      default: () => ({}),
    },
    includeTotalHours: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  emits: ["edit"],
  setup() {
    const configStore = useConfigStore();
    return {
      configStore,
    };
  },
  data() {
    return {
      areaOfInstructionList: [] as Components.Schemas.AreaOfInstruction[],
      loading: false,
    };
  },
  computed: {
    filteredAreas(): Components.Schemas.AreaOfInstruction[] {
      if (
        !this.areaOfInstructionList ||
        this.areaOfInstructionList.length === 0
      ) {
        return [];
      }

      const filtered = this.areaOfInstructionList.filter((area) => {
        return area?.programTypes?.includes(this.programType);
      });

      // Exclude child guidance. They are grouped together later
      const hasProgramDevelopment = filtered.some(
        (area) =>
          area.name === "Program Development, Curriculum and Foundations",
      );
      const hasChildGuidance = filtered.some(
        (area) => area.name === "Child Guidance",
      );

      if (hasProgramDevelopment && hasChildGuidance) {
        return filtered.filter((area) => area.name !== "Child Guidance");
      }

      return filtered;
    },
    nonAllocatedCourses(): Components.Schemas.Course[] {
      if (!this.program?.courses) {
        return [];
      }

      // Filter courses by programType first
      const coursesForProgramType = this.program.courses.filter(
        (course) => course.programType === this.programType,
      );

      // Find courses that have no allocated hours to any area
      return coursesForProgramType.filter((course) => {
        return (
          !course.courseAreaOfInstruction ||
          course.courseAreaOfInstruction.length === 0
        );
      });
    },
    totalHours(): number {
      if (!this.program?.courses) {
        return 0;
      }
      let total = 0;
      this.program.courses
        .filter((course) => course.programType === this.programType)
        .forEach((course) => {
          if (course.courseAreaOfInstruction) {
            course.courseAreaOfInstruction.forEach((courseArea) => {
              total += Number.parseFloat(courseArea.newHours || "0");
            });
          }
        });
      return total;
    },
    requiredHours(): number {
      return this.filteredAreas.reduce((sum, area) => {
        return sum + (area.minimumHours || 0);
      }, 0);
    },
  },
  async mounted() {
    await this.loadAreaOfInstructionList();
  },
  methods: {
    async loadAreaOfInstructionList() {
      this.loading = true;
      try {
        if (
          this.configStore.areaOfInstructionList &&
          this.configStore.areaOfInstructionList.length > 0
        ) {
          this.areaOfInstructionList = this.configStore.areaOfInstructionList;
        } else {
          const list = await getAreaOfInstructionList();
          if (list) {
            this.areaOfInstructionList = list;
            this.configStore.areaOfInstructionList = list;
          }
        }
      } catch (error) {
        console.error("Error loading area of instruction list:", error);
      } finally {
        this.loading = false;
      }
    },
    getCoursesForArea(
      areaId: string | null | undefined,
    ): CourseAreaOfInstructionWithCourse[] {
      if (!areaId || !this.program?.courses) {
        return [];
      }

      const coursesForArea: CourseAreaOfInstructionWithCourse[] = [];

      // Find the area to check its name
      const area = this.areaOfInstructionList.find((a) => a.id === areaId);
      const isProgramDevelopment =
        area?.name === "Program Development, Curriculum and Foundations";

      // If this is Program Development, Curriculum and Foundations, also find Child Guidance area
      let childGuidanceAreaId: string | null | undefined;
      if (isProgramDevelopment) {
        const childGuidanceArea = this.areaOfInstructionList.find(
          (a) => a.name === "Child Guidance",
        );
        childGuidanceAreaId = childGuidanceArea?.id;
      }

      // Filter courses by programType and extract CourseAreaOfInstructions
      this.program.courses
        .filter((course) => course.programType === this.programType)
        .forEach((course) => {
          if (course.courseAreaOfInstruction) {
            course.courseAreaOfInstruction.forEach((courseArea) => {
              const matchesCurrentArea =
                courseArea.areaOfInstructionId === areaId;
              // If Program Development, also include courses from Child Guidance
              const matchesChildGuidance =
                isProgramDevelopment &&
                childGuidanceAreaId &&
                courseArea.areaOfInstructionId === childGuidanceAreaId;

              if (matchesCurrentArea || matchesChildGuidance) {
                coursesForArea.push({
                  ...courseArea,
                  courseTitle: course.courseTitle,
                  courseNumber: course.courseNumber,
                });
              }
            });
          }
        });

      return coursesForArea;
    },
    getAreaSubtitles(areaId: string | null | undefined) {
      const areaIds = new Set(
        this.getCoursesForArea(areaId).map((c) => c.areaOfInstructionId),
      );

      return Object.fromEntries(
        Object.entries(this.areaSubtitles).filter(([key, value]) =>
          areaIds.has(key),
        ),
      );
    },
    handleEdit(courseArea: Components.Schemas.CourseAreaOfInstruction) {
      this.$emit("edit", courseArea);
    },
    generateRulesByProgramType() {
      //filteredAreas already combines Program Development + Child Guidance
      const rules = this.filteredAreas.map((area) => {
        if (area.minimumHours) {
          const minimumHours = area.minimumHours;
          let totalHours = this.getCoursesForArea(area.id).reduce(
            (sum, courseArea) =>
              sum + Number.parseFloat(courseArea.newHours || "0"),
            0,
          );

          return () =>
            totalHours >= minimumHours ||
            `${area.name} has not met the required hours`;
        }

        //no minimum hours defined, always valid
        return true;
      });

      const everyAreaHasHours = this.filteredAreas.every((area) => {
        this.getCoursesForArea(area.id).some((courseArea) => {
          Number.parseFloat(courseArea.newHours || "0") > 0;
        });
      });

      switch (this.programType) {
        case "Basic":
          //no additional rules
          break;
        case "ITE":
        case "SNE":
          // every area of instruction must be greater than 0 course hours
          const moreThanZeroHoursRules = this.filteredAreas.map((area) => {
            return () =>
              this.getCoursesForArea(area.id).some(
                (courseArea) =>
                  Number.parseFloat(courseArea.newHours || "0") > 0,
              ) || `${area.name} must have course hours assigned`;
          });
          rules.push(...moreThanZeroHoursRules);

          // total required hours must total at least 450
          rules.push(
            () =>
              this.totalHours >= MIN_HOURS_ITE_SNE ||
              `Total course hours must be at least ${MIN_HOURS_ITE_SNE} hours`,
          );
          break;
        default:
          console.warn(
            `Unknown program type '${this.programType}' for generating rules.`,
          );
      }
      return rules;
    },
  },
});
</script>
