<template>
  <v-dialog :model-value="show" @update:model-value="handleDialogClose($event)">
    <v-card class="pa-4">
      <v-card-title>
        Update Course
      </v-card-title>
      <v-card-text>
        <v-row>
          <v-col>
            <EceTextField
              id="txtCourseNumber"
              v-model="courseNumber"
              label="Course number"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <EceTextField
              id="txtCourseName"
              v-model="courseTitle"
              label="Course name"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col class="mb-6">
            <h3>Areas of instruction</h3>
            <p>Indicate the number of hours this course satisfies for each area of instruction.</p>
          </v-col>
        </v-row>
        <div v-for="area in areas" :key="area.id || undefined">
          <v-row
            no-gutters
            align="center"
            class="pl-4 mb-2 bg-white rounded-lg border"
          >
            <v-col>{{ area.name }}</v-col>
            <v-col class="d-flex justify-end">
              <v-text-field
                :model-value="getAreaHoursValue(area.id)"
                @update:model-value="setAreaHours(area.id, $event)"
                class="pa-2" 
                max-width="6rem"
              ></v-text-field>
            </v-col>
            <v-col><span v-if="area.minimumHours && area.minimumHours > 0"> / {{area.minimumHours}} hours required</span></v-col>
          </v-row>
        </div>
      </v-card-text>
      <v-card-actions class="ml-4 mb-4">
        <v-btn color="primary" variant="outlined"
               @click="handleCancel">Cancel</v-btn>
        <v-btn color="primary" variant="flat"
               @click="handleSave">Save changes</v-btn>
        <v-spacer></v-spacer>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import {defineComponent, type PropType, watch} from 'vue'
import EceTextField from "@/components/inputs/EceTextField.vue";
import type {Components} from "@/types/openapi";
import {useConfigStore} from "@/store/config";

export default defineComponent({
  name: "EditCourseDialog",
  components: {EceTextField},
  props: {
    course: {
      type: Object as PropType<Components.Schemas.Course | null>,
      required: true
    },
    programType: {
      type: String as PropType<Components.Schemas.ProgramTypes>,
      required: true
    },
    show: {
      type: Boolean,
      default: false
    }
  },
  emits: ["save", "cancel"],
  setup() {
    const configStore = useConfigStore();
    return {
      configStore
    };
  },
  data() {
    return {
      localCourse: null as Components.Schemas.Course | null,
      originalAreaIds: new Set<string>()
    };
  },
  created() {
    // Deep copy the course to avoid mutating the prop
    this.localCourse = JSON.parse(JSON.stringify(this.course)) as Components.Schemas.Course;
    
    // Ensure courseId is preserved (may not be in type definition but needed by backend)
    if (this.course && 'courseId' in this.course && !('courseId' in (this.localCourse as any))) {
      (this.localCourse as any).courseId = (this.course as any).courseId;
    }
    
    // Track which area IDs existed in the original course
    this.originalAreaIds = new Set(
      (this.course?.courseAreaOfInstruction || [])
        .map(c => c.areaOfInstructionId)
        .filter((id): id is string => id !== null && id !== undefined)
    );
  },
  computed: {
    areas(): Components.Schemas.AreaOfInstruction[] {
      return this.configStore.areaOfInstructionList.filter((area) => {
        return area?.programTypes?.includes(this.programType);
      });
    },
    courseNumber: {
      get(): string {
        return this.localCourse?.courseNumber ?? "";
      },
      set(value: string) {
        if (this.localCourse) {
          this.localCourse.courseNumber = value;
        }
      }
    },
    courseTitle: {
      get(): string {
        return this.localCourse?.courseTitle ?? "";
      },
      set(value: string) {
        if (this.localCourse) {
          this.localCourse.courseTitle = value;
        }
      }
    }
  },
  methods: {
    getAreaHoursValue(areaId: string | null | undefined): string {
      if (!areaId || !this.localCourse) {
        return "0";
      }

      // Ensure courseAreaOfInstruction array exists
      if (!this.localCourse.courseAreaOfInstruction) {
        this.localCourse.courseAreaOfInstruction = [];
      }

      // Find existing entry for this area
      const areaInstruction = this.localCourse.courseAreaOfInstruction.find(
        c => c.areaOfInstructionId === areaId
      );

      return areaInstruction?.newHours ?? "0";
    },
    setAreaHours(areaId: string | null | undefined, value: string) {
      if (!areaId || !this.localCourse) {
        return;
      }

      // Ensure courseAreaOfInstruction array exists
      if (!this.localCourse.courseAreaOfInstruction) {
        this.localCourse.courseAreaOfInstruction = [];
      }

      const courseAreaOfInstruction = this.localCourse.courseAreaOfInstruction;
      const hoursValue = Number.parseFloat(value || "0");

      // Find existing entry for this area
      const existingIndex = courseAreaOfInstruction.findIndex(
        c => c.areaOfInstructionId === areaId
      );

      const wasInOriginal = this.originalAreaIds.has(areaId);

      if (hoursValue > 0) {
        // If hours > 0, update or create entry
        if (existingIndex >= 0) {
          // Update existing entry
          const existingEntry = courseAreaOfInstruction[existingIndex];
          if (existingEntry) {
            existingEntry.newHours = value;
          }
        } else {
          // Create new entry only if hours > 0
          courseAreaOfInstruction.push({
            areaOfInstructionId: areaId,
            newHours: value
          });
        }
      } else {
        if (existingIndex >= 0) {
          const existingEntry = courseAreaOfInstruction[existingIndex];
          if (existingEntry) {
            if (wasInOriginal) {
              // Keep original entries even at 0 hours, just update the value
              existingEntry.newHours = "0";
            } else {
              // Remove only if it was newly created (didn't exist in original)
              courseAreaOfInstruction.splice(existingIndex, 1);
            }
          }
        }
        // If it doesn't exist and hours is 0, don't create it
      }
    },
    handleSave() {
      if (this.localCourse) {
        this.$emit("save", this.localCourse);
      }
    },
    handleCancel() {
      this.localCourse = null;
      this.originalAreaIds = new Set<string>();
      this.$emit("cancel");
    },
    handleDialogClose(value: boolean) {
      if(!value) {
        this.handleCancel();
      }
    }
  }
})
</script>
