<template>
  <v-dialog :model-value="show" @update:model-value="handleDialogClose($event)" :persistent="saving">
    <v-form ref="updateCourse">
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
                :rules="[Rules.required()]"
                :disabled="saving"
              ></EceTextField>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <EceTextField
                id="txtCourseName"
                v-model="courseTitle"
                label="Course name"
                :rules="[Rules.required()]"
                :disabled="saving"
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
                  type="number"
                  :disabled="saving"
                ></v-text-field>
              </v-col>
              <v-col><span v-if="area.minimumHours && area.minimumHours > 0"> / {{area.minimumHours}} hours required</span></v-col>
            </v-row>
          </div>
        </v-card-text>
        <v-card-actions class="ml-4 mb-4">
          <v-btn color="primary" variant="outlined"
                 :disabled="saving"
                 @click="handleCancel">Cancel</v-btn>
          <v-btn color="primary" variant="flat"
                 :loading="saving"
                 :disabled="saving"
                 @click="handleSave">Save changes</v-btn>
          <v-spacer></v-spacer>
        </v-card-actions>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script lang="ts">
import {defineComponent, type PropType, toRaw} from 'vue'
import EceTextField from "@/components/inputs/EceTextField.vue";
import type {Components} from "@/types/openapi";
import {useConfigStore} from "@/store/config";
import * as Rules from "@/utils/formRules";
import {number} from "@/utils/formRules";

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
    },
    saving: {
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
      courseNumber: "" as string,
      courseTitle: "" as string,
      localCourse: null as Components.Schemas.Course | null,
      originalAreaIds: new Set<string>(),
      Rules,
    };
  },
  created() {
    // Deep copy the course to avoid mutating the prop
    this.localCourse = this.course
      ? structuredClone(toRaw(this.course)) as Components.Schemas.Course
      : null;
    
    // Track which area IDs existed in the original course
    this.originalAreaIds = new Set(
      (this.course?.courseAreaOfInstruction || [])
        .map(c => c.areaOfInstructionId)
        .filter((id): id is string => id !== null && id !== undefined)
    );
    
    this.courseNumber = this.localCourse?.newCourseNumber ? this.localCourse?.newCourseNumber : this.localCourse?.courseNumber ?? "";
    this.courseTitle = this.localCourse?.newCourseTitle ? this.localCourse?.newCourseTitle : this.localCourse?.courseTitle ?? "";
  },
  computed: {
    areas(): Components.Schemas.AreaOfInstruction[] {
      return this.configStore.areaOfInstructionList.filter((area) => {
        return area?.programTypes?.includes(this.programType);
      });
    },
  },
  methods: {
    number,
    getAreaHoursValue(areaId: string | null | undefined): string {
      if (!areaId || !this.localCourse) {
        return "0";
      }
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

      let courseAreaOfInstruction = this.localCourse.courseAreaOfInstruction;
      const hoursValue = Number.parseFloat(value || "0");

      // Find existing entry for this area
      const existingCourseAreaOfInstruction = courseAreaOfInstruction.find(
        c => c.areaOfInstructionId === areaId
      );

      const wasInOriginal = this.originalAreaIds.has(areaId);

      if (hoursValue > 0) {
        if (existingCourseAreaOfInstruction) { //update existing
          existingCourseAreaOfInstruction.newHours = value;
        } else { //create new
          courseAreaOfInstruction.push({
            areaOfInstructionId: areaId,
            newHours: value
          });
        }
      } else if (existingCourseAreaOfInstruction && wasInOriginal) {
        // Keep original entries even at 0 hours, just update the value
        existingCourseAreaOfInstruction.newHours = "0";
      } else if (existingCourseAreaOfInstruction) {
        // Remove only if it was newly created (didn't exist in original)
        this.localCourse.courseAreaOfInstruction = courseAreaOfInstruction.filter(c => c.areaOfInstructionId !== areaId);
      }
      // else it doesn't exist and hours is 0, so don't create it
    },
    handleSave() {
      if (this.localCourse) {
        if(this.localCourse.courseNumber === this.courseNumber) {
          this.localCourse.newCourseNumber = null;
        } else {
          this.localCourse.newCourseNumber = this.courseNumber;
        }
        if (this.localCourse.courseTitle === this.courseTitle) {
          this.localCourse.newCourseTitle = null;
        } else {
          this.localCourse.newCourseTitle = this.courseTitle;
        }
        this.$emit("save", this.localCourse);
      }
    },
    handleCancel() {
      this.localCourse = null;
      this.originalAreaIds = new Set<string>();
      this.$emit("cancel");
    },
    handleDialogClose(value: boolean) {
      if(!value && !this.saving) {
        this.handleCancel();
      }
    }
  }
})
</script>
