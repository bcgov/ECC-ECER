<template>
  <v-card 
    flat
    rounded="lg"
    :border="true"
    color="#EDEBE9"
  >
    <v-card-text>
      <v-card-title class="pl-0">Non-allocated courses</v-card-title>
      <v-card-subtitle class="pl-0 pb-4">
        Courses listed here have no hours allocated toward any of the required areas of instruction. If changes are required, edit the course to allocate hours.
      </v-card-subtitle>
      <div v-if="courses.length > 0">
        <v-row
          v-for="(course, index) in courses"
          :key="`course-${index}-${course.courseNumber}`"
          no-gutters
          align="center"
          class="pl-4 mb-2 bg-white rounded-lg border"
        >
          <v-col cols="8">
            <span class="font-weight-bold">
              {{ getCourseTitle(course) }}
            </span>
          </v-col>
          <v-col>
            <!-- Empty column to match layout with hours column -->
          </v-col>
          <v-col cols="auto" class="d-flex">
            <v-divider vertical></v-divider>
            <v-btn
              icon="mdi-pencil"
              variant="plain"
              @click="handleEdit(course)"
            ></v-btn>
          </v-col>
        </v-row>
      </div>
      
      <p v-else class="text-grey-darken-1">No courses added yet.</p>
    </v-card-text>
  </v-card>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "NonAllocatedCoursesCard",
  components: {},
  props: {
    courses: {
      type: Array as PropType<Components.Schemas.Course[]>,
      required: true,
      default: () => [],
    },
  },
  emits: ["edit"],
  methods: {
    getCourseTitle(course: Components.Schemas.Course): string {
      const courseNumber = course.courseNumber || "";
      const courseTitle = course.courseTitle || "";
      
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
    handleEdit(course: Components.Schemas.Course) {
      this.$emit("edit", course);
    },
  },
});
</script>

<style scoped>

</style>
