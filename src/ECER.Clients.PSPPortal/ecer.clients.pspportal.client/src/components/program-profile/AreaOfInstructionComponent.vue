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
      @edit="handleCourseEdit"
    />
    <EditCourseDialog
      v-if="selectedCourse"
      :show="showEditCourseDialog"
      :program-type="programType"
      :course="selectedCourse"
      @save="handleCourseSave"
      @cancel="showEditCourseDialog=false; selectedCourse=null"
    />
  </div>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { Components } from "@/types/openapi";
import { useConfigStore } from "@/store/config";
import { useAlertStore } from "@/store/alert";
import { useProgramStore } from "@/store/program";
import { getAreaOfInstructionList } from "@/api/configuration";
import {getPrograms, updateCourse} from "@/api/program";
import AreaOfInstructionCard from "./AreaOfInstructionCard.vue";
import EditCourseDialog from "./EditCourseDialog.vue";
import NonAllocatedCoursesCard from "./NonAllocatedCoursesCard.vue";
import TotalHoursOfInstructionCard from "./TotalHoursOfInstructionCard.vue";

interface CourseAreaOfInstructionWithCourse
  extends Components.Schemas.CourseAreaOfInstruction {
  courseTitle?: string | null;
  courseNumber?: string | null;
}

export default defineComponent({
  name: "AreaOfInstructionComponent",
  components: {
    AreaOfInstructionCard,
    EditCourseDialog,
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
  emits: ["courseEdit"],
  setup() {
    const configStore = useConfigStore();
    const alertStore = useAlertStore();
    const programStore = useProgramStore();
    return {
      configStore,
      alertStore,
      programStore,
    };
  },
  data() {
    return {
      areaOfInstructionList: [] as Components.Schemas.AreaOfInstruction[],
      loading: false,
      selectedCourse: null as Components.Schemas.Course | null,
      showEditCourseDialog: false,
      totalHours: 450
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
        if(!course.courseAreaOfInstruction || course.courseAreaOfInstruction.length === 0) {
          return true
        }
        return course.courseAreaOfInstruction.every(area => !area.newHours || Number.parseFloat(area.newHours) === 0);
      });
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

      // First, collect Program Development courses
      this.program.courses
        .filter((course) => course.programType === this.programType)
        .forEach((course) => {
          if (course.courseAreaOfInstruction) {
            course.courseAreaOfInstruction.forEach((courseArea) => {
              const matchesCurrentArea =
                courseArea.areaOfInstructionId === areaId;

              if (matchesCurrentArea && courseArea?.newHours && Number.parseFloat(courseArea.newHours) > 0) {
                coursesForArea.push({
                  ...courseArea,
                  courseTitle: course.courseTitle,
                  courseNumber: course.courseNumber,
                });
              }
            });
          }
        });

      // If this is Program Development, also collect Child Guidance courses separately
      if (isProgramDevelopment) {
        const childGuidanceArea = this.areaOfInstructionList.find(
          (a) => a.name === "Child Guidance",
        );
        const childGuidanceAreaId = childGuidanceArea?.id;
        let hasChildGuidanceCourses = false;

        if (childGuidanceAreaId) {
          this.program.courses
            .filter((course) => course.programType === this.programType)
            .forEach((course) => {
              if (course.courseAreaOfInstruction) {
                course.courseAreaOfInstruction.forEach((courseArea) => {
                  if (courseArea.areaOfInstructionId === childGuidanceAreaId) {
                    hasChildGuidanceCourses = true;
                    coursesForArea.push({
                      ...courseArea,
                      courseTitle: course.courseTitle,
                      courseNumber: course.courseNumber,
                    });
                  }
                });
              }
            });

          // If Child Guidance exists but has no courses, add a placeholder entry
          if (!hasChildGuidanceCourses) {
            coursesForArea.push({
              areaOfInstructionId: childGuidanceAreaId,
              newHours: "0",
            } as CourseAreaOfInstructionWithCourse);
          }
        }
      }

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
    handleCourseEdit(course: Components.Schemas.Course) {
      this.selectedCourse = course;
      this.showEditCourseDialog = true;
    },
    handleEdit(areaOfInstructionCourse: Components.Schemas.CourseAreaOfInstruction) {
      this.selectedCourse = this.program?.courses
        ?.find(course =>
          course?.courseAreaOfInstruction?.some(areaCourse => areaCourse.courseAreaOfInstructionId === areaOfInstructionCourse.courseAreaOfInstructionId)
        ) || null;
      this.showEditCourseDialog = true;
    },
    async handleCourseSave(updatedCourse: Components.Schemas.Course) {
      if (!this.program?.id || !updatedCourse) {
        return;
      }

      // Ensure courseId is set (may not be in type definition but needed by backend)
      const courseWithId = { ...updatedCourse } as Components.Schemas.Course & { courseId?: string };
      if (!courseWithId.courseId && this.selectedCourse && 'courseId' in this.selectedCourse) {
        courseWithId.courseId = (this.selectedCourse as any).courseId;
      }

      // Ensure courseAreaOfInstruction has required fields
      if (courseWithId.courseAreaOfInstruction) {
        courseWithId.courseAreaOfInstruction = courseWithId.courseAreaOfInstruction.map(
          (area) => ({
            ...area,
            areaOfInstructionId: area.areaOfInstructionId || "",
            // Ensure CourseAreaOfInstructionId is set if it exists
            courseAreaOfInstructionId: area.courseAreaOfInstructionId || undefined,
          })
        );
      }

      const { error } = await updateCourse(this.program.id, [courseWithId as Components.Schemas.Course]);

      if (error) {
        this.alertStore.setFailureAlert(
          "Sorry, something went wrong and your changes could not be saved. Try again later."
        );
      } else {
        this.alertStore.setSuccessAlert("Course has been updated successfully.");
        // Close the dialog
        this.showEditCourseDialog = false;
        this.selectedCourse = null;
        // Reload program and save to store
        await this.loadProgram(this.program.id);
        // Emit event to parent to refresh program data if needed
        this.$emit("courseEdit");
      }
    },
    async loadProgram(programId?: string | null | undefined) {
      if (!programId) {
        return;
      }
      this.loading = true;
      try {
        const { data: programs } = await getPrograms(programId, [
          "Draft",
          "Denied",
          "Approved",
          "UnderReview",
          "ChangeRequestInProgress",
          "Inactive",
        ]);
        const program = programs && programs.length > 0 ? programs[0] : null;
        if (program) {
          // Update the store with the reloaded program
          this.programStore.setDraftProgramFromProfile(program);
        }
      } catch (error) {
        console.error("Error loading program:", error);
      } finally {
        this.loading = false;
      }
    }
  },
});
</script>
