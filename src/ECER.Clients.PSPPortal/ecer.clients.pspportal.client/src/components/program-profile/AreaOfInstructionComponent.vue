<template>
  <Loading v-if="loading"></Loading>
  <template v-else>
    <h2 v-if="type === 'ProgramProfile'" class="mb-4">
      Required areas of instruction
    </h2>
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

    <v-btn
      v-if="type === 'ProgramApplication'"
      id="btnAddCourse"
      class="mb-5"
      rounded="lg"
      color="primary"
      @click="handleAddCourse"
    >
      Add course
    </v-btn>

    <AreaOfInstructionCard
      v-for="(area, index) in filteredAreas"
      :key="area.id || index"
      class="mb-4"
      :course-area-of-instructions="getCoursesForArea(area.id)"
      :area-subtitles="getAreaSubtitles(area.id)"
      :area-id="area.id || undefined"
      :show-progress-bar="(area.minimumHours && area.minimumHours > 0) || false"
      :show-delete-button="type === 'ProgramApplication'"
      @edit="handleEdit"
      @delete="handleAreaOfInstructionCourseDelete"
      :loading="loadingStore.isLoading('course_delete')"
    />

    <NonAllocatedCoursesCard
      v-if="nonAllocatedCourses.length > 0"
      :courses="nonAllocatedCourses"
      :show-delete-button="type === 'ProgramApplication'"
      @edit="handleCourseEdit"
      @delete="handleCourseDelete"
      :loading="loadingStore.isLoading('course_delete')"
    />
  </template>
  <AddEditCourseDialog
    v-if="selectedCourse"
    :show="showAddEditCourseDialog"
    :program-type="programType"
    :course="selectedCourse"
    :courseList="courses"
    :saving="saving"
    :courseDialogMode="courseDialogMode"
    @save="handleCourseSave"
    @cancel="
      showAddEditCourseDialog = false;
      selectedCourse = null;
    "
    @click-outside="
      showAddEditCourseDialog = false;
      selectedCourse = null;
    "
    @exit="
      showAddEditCourseDialog = false;
      selectedCourse = null;
    "
  />
  <ConfirmationDialog
    v-if="selectedCourseToDelete"
    :show="showConfirmationDialog"
    cancel-button-text="Cancel"
    accept-button-text="Remove course"
    title="Remove course"
    :loading="loadingStore.isLoading('course_delete')"
    @accept="deleteCourse"
    @cancel="
      showConfirmationDialog = false;
      selectedCourseToDelete = null;
    "
    @click-outside="
      showConfirmationDialog = false;
      selectedCourseToDelete = null;
    "
    @exit="
      showConfirmationDialog = false;
      selectedCourseToDelete = null;
    "
  >
    <template #confirmation-text>
      <p>
        Are you sure you want to remove this course from the program
        application?
      </p>
      <br />
      <p>
        <strong>{{ getCourseTitle(selectedCourseToDelete) }}</strong>
      </p>
      <br />
      <p>
        Removing this course will also remove its allocated hours from all areas
        of instruction.
      </p>
    </template>
  </ConfirmationDialog>

  <!-- this is to block the user from progressing if hours are not met -->
  <v-input
    v-if="type === 'ProgramProfile'"
    v-model="courses"
    :rules="generateRulesByProgramType()"
    :max-errors="5"
  ></v-input>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { Components } from "@/types/openapi";
import { useConfigStore } from "@/store/config";
import { useAlertStore } from "@/store/alert";
import { getAreaOfInstructionList } from "@/api/configuration";
import { updateCourse, addCourse, deleteCourse } from "@/api/course";
import AreaOfInstructionCard from "./AreaOfInstructionCard.vue";
import AddEditCourseDialog from "./AddEditCourseDialog.vue";
import NonAllocatedCoursesCard from "./NonAllocatedCoursesCard.vue";
import TotalHoursOfInstructionCard from "./TotalHoursOfInstructionCard.vue";
import ConfirmationDialog from "../ConfirmationDialog.vue";
import Loading from "@/components/Loading.vue";
import { useLoadingStore } from "@/store/loading";
import { MIN_HOURS_ITE_SNE } from "@/utils/constant";
import { getCourseTitle } from "@/utils/functions";

interface CourseAreaOfInstructionWithCourse
  extends Components.Schemas.CourseAreaOfInstruction {
  courseTitle?: string | null;
  courseNumber?: string | null;
}

export default defineComponent({
  name: "AreaOfInstructionComponent",
  components: {
    AreaOfInstructionCard,
    AddEditCourseDialog,
    ConfirmationDialog,
    NonAllocatedCoursesCard,
    TotalHoursOfInstructionCard,
    Loading,
  },
  props: {
    programType: {
      type: String as PropType<Components.Schemas.ProgramTypes>,
      required: true,
    },
    courses: {
      type: Object as PropType<Components.Schemas.Course[]>,
      required: true,
    },
    // this can refer to a either programProfile or programApplication id depending on programType
    id: {
      type: String,
      required: true,
    },
    type: {
      type: String as PropType<Components.Schemas.FunctionType>,
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
  emits: ["courseEdit", "reloadCourses"],
  setup() {
    const configStore = useConfigStore();
    const alertStore = useAlertStore();
    const loadingStore = useLoadingStore();
    return {
      configStore,
      alertStore,
      loadingStore,
    };
  },
  data() {
    return {
      areaOfInstructionList: [] as Components.Schemas.AreaOfInstruction[],
      saving: false,
      requiredHours: MIN_HOURS_ITE_SNE,
      selectedCourse: null as Components.Schemas.Course | null,
      showAddEditCourseDialog: false,
      courseDialogMode: "edit" as "edit" | "add",
      showConfirmationDialog: false,
      selectedCourseToDelete: null as Components.Schemas.Course | null,
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

      // Exclude areas with parents. They are included later
      let result = filtered.filter(
        (area) => area.parentAreaOfInstructionId === null,
      );

      // Sort by displayOrder (null values go to the end)
      return result.sort((a, b) => {
        if (a.displayOrder === null || a.displayOrder === undefined) return 1;
        if (b.displayOrder === null || b.displayOrder === undefined) return -1;
        return a.displayOrder.localeCompare(b.displayOrder, undefined, {
          numeric: true,
        });
      });
    },
    loading(): boolean {
      return (
        this.loadingStore.isLoading("courses_get") ||
        this.loadingStore.isLoading("course_put") ||
        this.loadingStore.isLoading("course_post") ||
        this.loadingStore.isLoading("program_get")
      );
    },
    nonAllocatedCourses(): Components.Schemas.Course[] {
      if (!this.courses) {
        return [];
      }

      // Filter courses by programType first
      const coursesForProgramType = this.courses.filter(
        (course) => course.programType === this.programType,
      );

      // Find courses that have no allocated hours to any area
      return coursesForProgramType.filter((course) => {
        if (
          !course.courseAreaOfInstruction ||
          course.courseAreaOfInstruction.length === 0
        ) {
          return true;
        }
        return course.courseAreaOfInstruction.every(
          (area) => !area.newHours || Number.parseFloat(area.newHours) === 0,
        );
      });
    },
    totalHours(): number {
      if (!this.courses) {
        return 0;
      }
      let total = 0;
      this.courses
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
  },
  async mounted() {
    await this.loadAreaOfInstructionList();
  },
  methods: {
    getCourseTitle,
    async loadAreaOfInstructionList() {
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
      }
    },
    getCoursesForArea(
      areaId: string | null | undefined,
    ): CourseAreaOfInstructionWithCourse[] {
      if (!areaId || !this.courses) {
        return [];
      }
      const coursesForArea: CourseAreaOfInstructionWithCourse[] = [];
      // Collect courses directly assigned to this area
      this.courses
        .filter((course) => course.programType === this.programType)
        .forEach((course) => {
          if (course.courseAreaOfInstruction) {
            course.courseAreaOfInstruction.forEach((courseArea) => {
              if (
                courseArea.areaOfInstructionId === areaId &&
                courseArea?.newHours &&
                Number.parseFloat(courseArea.newHours) > 0
              ) {
                coursesForArea.push({
                  ...courseArea,
                  courseTitle: course.newCourseTitle ?? course.courseTitle,
                  courseNumber: course.newCourseNumber ?? course.courseNumber,
                });
              }
            });
          }
        });
      // Collect courses for any child areas (parentAreaOfInstructionId === areaId)
      const childAreas = this.areaOfInstructionList.filter(
        (a) => a.parentAreaOfInstructionId === areaId,
      );
      childAreas.forEach((childArea) => {
        if (!childArea.id) return;
        let hasCoursesForChildArea = false;
        this.courses
          .filter((course) => course.programType === this.programType)
          .forEach((course) => {
            if (course.courseAreaOfInstruction) {
              course.courseAreaOfInstruction.forEach((courseArea) => {
                if (courseArea.areaOfInstructionId === childArea.id) {
                  hasCoursesForChildArea = true;
                  coursesForArea.push({
                    ...courseArea,
                    courseTitle: course.newCourseTitle ?? course.courseTitle,
                    courseNumber: course.newCourseNumber ?? course.courseNumber,
                  });
                }
              });
            }
          });
        // If the child area has no courses, add a placeholder so it still renders
        if (!hasCoursesForChildArea) {
          coursesForArea.push({
            areaOfInstructionId: childArea.id,
            newHours: "0",
          } as CourseAreaOfInstructionWithCourse);
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
    handleCourseEdit(course: Components.Schemas.Course) {
      this.selectedCourse = course;
      this.courseDialogMode = "edit";
      this.showAddEditCourseDialog = true;
    },
    handleEdit(
      areaOfInstructionCourse: Components.Schemas.CourseAreaOfInstruction,
    ) {
      this.selectedCourse =
        this.courses?.find((course) =>
          course?.courseAreaOfInstruction?.some(
            (areaCourse) =>
              areaCourse.courseAreaOfInstructionId ===
              areaOfInstructionCourse.courseAreaOfInstructionId,
          ),
        ) || null;
      this.courseDialogMode = "edit";
      this.showAddEditCourseDialog = true;
    },
    handleAddCourse() {
      this.selectedCourse = {
        courseId: "",
        courseTitle: "",
        courseNumber: "",
        programType: this.programType,
        courseAreaOfInstruction: [],
      } as Components.Schemas.Course;
      this.showAddEditCourseDialog = true;
      this.courseDialogMode = "add";
    },
    async handleCourseSave(updatedCourse: Components.Schemas.Course) {
      if ((!this.id && this.courseDialogMode === "edit") || !updatedCourse) {
        console.log(
          "Invalid course save data for edit mode. This should not happen.",
        );
        this.alertStore.setFailureAlert(
          "Sorry, something went wrong and your changes could not be saved. Try again later.",
        );
        return;
      }
      this.saving = true;
      try {
        let error;
        if (this.courseDialogMode === "edit") {
          error = (
            await updateCourse(
              this.id,
              updatedCourse as Components.Schemas.Course,
              this.type,
            )
          )?.error;
        } else if (this.courseDialogMode === "add") {
          error = (await addCourse(this.id, updatedCourse, this.type))?.error;
        }

        if (error) {
          this.alertStore.setFailureAlert(
            "Sorry, something went wrong and your changes could not be saved. Try again later.",
          );
          this.saving = false;
        } else {
          this.alertStore.setSuccessAlert(
            "Course has been updated successfully.",
          );
          this.$emit("reloadCourses");
          this.showAddEditCourseDialog = false;
          this.selectedCourse = null;
          this.saving = false;
        }
      } catch (error) {
        this.saving = false;
        console.error("Error saving course:", error);
        this.alertStore.setFailureAlert(
          "Sorry, something went wrong and your changes could not be saved. Try again later.",
        );
      }
    },
    handleCourseDelete(course: Components.Schemas.Course) {
      this.selectedCourseToDelete = course;
      this.showConfirmationDialog = true;
    },
    handleAreaOfInstructionCourseDelete(
      areaOfInstructionCourse: Components.Schemas.CourseAreaOfInstruction,
    ) {
      const courseToDelete =
        this.courses?.find((course) =>
          course?.courseAreaOfInstruction?.some(
            (areaCourse) =>
              areaCourse.courseAreaOfInstructionId ===
              areaOfInstructionCourse.courseAreaOfInstructionId,
          ),
        ) || null;

      if (!courseToDelete) {
        console.warn("course not found, this should not happen");
        this.alertStore.setFailureAlert(
          "Sorry, something went wrong and the course could not be deleted. Try again later.",
        );
        return;
      }

      this.selectedCourseToDelete = courseToDelete;
      this.showConfirmationDialog = true;
    },
    async deleteCourse() {
      if (this.selectedCourseToDelete) {
        const { error } = await deleteCourse(
          this.selectedCourseToDelete.courseId || "",
          this.type === "ProgramApplication" ? this.id : undefined,
        );
        if (error) {
          this.alertStore.setFailureAlert(
            "Sorry, something went wrong and the course could not be deleted. Try again later.",
          );
        } else {
          this.alertStore.setSuccessAlert(
            "Course has been deleted successfully.",
          );
          this.$emit("reloadCourses");
          this.showConfirmationDialog = false;
        }
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
            // total required hours must total at least 450
            const moreThanMinimumHoursRule = () =>
              this.totalHours >= MIN_HOURS_ITE_SNE ||
              `Total course hours must be at least ${MIN_HOURS_ITE_SNE} hours`;
            rules.push(moreThanMinimumHoursRule);
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
