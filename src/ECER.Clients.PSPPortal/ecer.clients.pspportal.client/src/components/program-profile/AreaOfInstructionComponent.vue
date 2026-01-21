<template>
  <div>
    <Loading v-if="loading"></Loading>
    <div v-else>
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
        :key="area.id || index"
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
    </div>
    <EditCourseDialog
      v-if="selectedCourse"
      :show="showEditCourseDialog"
      :program-type="programType"
      :course="selectedCourse"
      :saving="saving"
      @save="handleCourseSave"
      @cancel="showEditCourseDialog=false; selectedCourse=null"
    />
  </div>
  <!-- this is to block the user from progressing if hours are not met -->
  <v-input :rules="generateRulesByProgramType()" :max-errors="5"></v-input>
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
import Loading from "@/components/Loading.vue";
import { useLoadingStore } from "@/store/loading";

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
    EditCourseDialog,
    NonAllocatedCoursesCard,
    TotalHoursOfInstructionCard,
    Loading,
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
    const loadingStore = useLoadingStore();
    const programStore = useProgramStore();
    return {
      configStore,
      alertStore,
      loadingStore,
      programStore,
    };
  },
  data() {
    return {
      areaOfInstructionList: [] as Components.Schemas.AreaOfInstruction[],
      saving: false,
      requiredHours: 450,
      selectedCourse: null as Components.Schemas.Course | null,
      showEditCourseDialog: false,
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
    loading(): boolean {
      return (
        this.loadingStore.isLoading('program_get') ||
        this.loadingStore.isLoading('course_put')
      );
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
    }
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
                  courseTitle: course.newCourseTitle ? course.newCourseTitle : course.courseTitle,
                  courseNumber: course.newCourseNumber ? course.newCourseNumber : course.courseNumber,
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
        console.log("Invalid course save data. This should not happen.")
        this.alertStore.setFailureAlert(
          "Sorry, something went wrong and your changes could not be saved. Try again later."
        );
        return;
      }
      this.saving = true;
      try {
        const { error } = await updateCourse(this.program.id, [updatedCourse as Components.Schemas.Course]);

        if (error) {
          this.alertStore.setFailureAlert(
            "Sorry, something went wrong and your changes could not be saved. Try again later."
          );
          this.saving = false;
        } else {
          this.alertStore.setSuccessAlert("Course has been updated successfully.");
          this.showEditCourseDialog = false;
          this.selectedCourse = null;
          this.saving = false;
          await this.loadProgram(this.program.id);
        }
      } catch (error) {
        this.saving = false;
        console.error("Error saving course:", error);
        this.alertStore.setFailureAlert(
          "Sorry, something went wrong and your changes could not be saved. Try again later."
        );
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

      switch (this.programType) {
        case "Basic":
          //no additional rules
          break;
        case "ITE":
        case "SNE":
          {
            // every area of instruction must be greater than 0 course hours
            const moreThanZeroHoursRules = this.filteredAreas.map((area) => {
              return () =>
                this.getCoursesForArea(area.id).some(
                  (courseArea) =>
                    Number.parseFloat(courseArea.newHours || "0") > 0,
                ) || `${area.name} must have course hours assigned`;
            });

            // total required hours must total at least 450
            const moreThanMinimumHoursRule = () =>
              this.totalHours >= MIN_HOURS_ITE_SNE ||
              `Total course hours must be at least ${MIN_HOURS_ITE_SNE} hours`;
            rules.push(...moreThanZeroHoursRules, moreThanMinimumHoursRule);
          }
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
