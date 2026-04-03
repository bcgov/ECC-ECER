<template>
  <Loading v-if="loading" />
  <PageContainer v-else>
    <v-row class="justify-space-between mb-4">
      <v-col cols="auto">
        <h1>Review program profile</h1>
      </v-col>
      <v-col cols="auto">
        <v-btn rounded="lg" variant="text" @click="printPage()">
          <v-icon
            color="secondary"
            icon="mdi-printer-outline"
            class="mr-2"
          ></v-icon>
          <a class="small">Print Preview</a>
        </v-btn>
      </v-col>
    </v-row>
    <v-card
      class="mb-5"
      v-for="programType in programTypes"
      variant="outlined"
      rounded="lg"
    >
      <v-card-title>
        <div class="d-flex justify-space-between align-center">
          <div>
            <h2 class="text-wrap">{{ generateTitle(programType) }}</h2>
          </div>
          <div v-if="editable">
            <v-tooltip location="top">
              <template #activator="{ props }">
                <v-btn
                  icon="mdi-pencil"
                  v-bind="props"
                  variant="plain"
                  @click="
                    router.push({
                      name: 'program-application-program-profile-area-of-instruction',
                      params: {
                        programApplicationId,
                        programType,
                      },
                    })
                  "
                />
              </template>
              <span>Edit {{ programType }} courses</span>
            </v-tooltip>
          </div>
        </div>
      </v-card-title>
      <v-card-text class="text-grey-dark">
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
          ] in getCoursesBasedOnProgramTypeGroupedByAreaOfInstruction(
            allCourses,
            programType,
          )"
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
        <v-row
          v-if="
            getNonAllocatedCoursesByType(allCourses, programType).length > 0
          "
          no-gutters
        >
          <v-col cols="4">Non-allocated courses</v-col>
          <v-col cols="8">
            <div
              v-for="unallocatedCourse in getNonAllocatedCoursesByType(
                allCourses,
                programType,
              )"
            >
              <v-row no-gutters>
                <v-col cols="6">
                  <strong>{{ getCourseTitle(unallocatedCourse) }}</strong>
                </v-col>
              </v-row>
            </div>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <v-row v-if="editable" class="d-print-none mt-4">
      <v-col>
        <v-btn rounded="lg" color="primary" @click="$emit('next', {})">
          Continue
        </v-btn>
      </v-col>
    </v-row>
  </PageContainer>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Loading from "@/components/Loading.vue";
import type { Components } from "@/types/openapi";
import { getProgramApplicationById } from "@/api/program-application";
import { getCourses } from "@/api/course";
import { useAlertStore } from "@/store/alert";
import { useLoadingStore } from "@/store/loading";
import { useConfigStore } from "@/store/config";
import { useRouter } from "vue-router";
import {
  getCoursesBasedOnProgramTypeGroupedByAreaOfInstruction,
  getNonAllocatedCoursesByType,
  getCourseTitle,
} from "@/utils/functions";
import type { NextStepPayload } from "@/components/program-application/ProgramApplication.vue";
import type { CourseAreaDetail } from "@/types/helperFunctions";

export default defineComponent({
  name: "ProgramApplicationReviewResponses",
  components: {
    PageContainer,
    Loading,
  },
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
  },
  setup() {
    const alertStore = useAlertStore();
    const loadingStore = useLoadingStore();
    const configStore = useConfigStore();
    const router = useRouter();
    return { alertStore, loadingStore, configStore, router };
  },
  emits: { next: (_payload: NextStepPayload) => true },
  computed: {
    loading(): boolean {
      return (
        this.loadingStore.isLoading("program_application_get") ||
        this.loadingStore.isLoading("courses_get")
      );
    },
  },
  data() {
    return {
      isLoading: true,
      programTypes: [] as Components.Schemas.ProgramTypes[],
      allCourses: [] as Components.Schemas.Course[],
      editable: false,
    };
  },
  async mounted() {
    await this.loadInformation();
  },
  methods: {
    getCoursesBasedOnProgramTypeGroupedByAreaOfInstruction,
    getNonAllocatedCoursesByType,
    getCourseTitle,
    printPage() {
      globalThis.print();
    },
    async loadInformation() {
      if (!this.programApplicationId) {
        console.warn("programApplicationId was not provided");
        return;
      }
      const programApplication = await getProgramApplicationById(
        this.programApplicationId,
      );
      if (programApplication.error || programApplication.data == null) {
        this.alertStore.setFailureAlert("Failed to load program application");
        console.error(programApplication.error);
        return;
      }

      this.programTypes = programApplication.data.programTypes || [];

      const isNewBasic =
        programApplication.data.programApplicationType ===
        "NewBasicECEPostBasicProgram";
      const requestType = isNewBasic ? "ProgramApplication" : "ProgramProfile";
      const requestId = isNewBasic
        ? this.programApplicationId
        : (programApplication.data.programProfileId ??
          this.programApplicationId);

      this.editable =
        isNewBasic &&
        (programApplication.data.status === "Draft" ||
          programApplication.data.statusReasonDetail === "RFAIrequested");

      this.allCourses =
        (await getCourses(
          requestId,
          requestType,
          programApplication.data.programTypes || [],
        )) || [];
    },
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
    generateTitle(programType: Components.Schemas.ProgramTypes): string {
      switch (programType) {
        case "Basic":
          return "ECE (Basic)";
        case "ITE":
          return "Infant and Toddler Educator";
        case "SNE":
          return "Special Needs Educator";
        default:
          return "Unknown Program Type";
      }
    },
  },
});
</script>
