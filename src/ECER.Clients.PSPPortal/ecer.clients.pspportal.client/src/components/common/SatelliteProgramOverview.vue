<template>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Institution name</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ userStore.educationInstitution?.name }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Location</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ campus }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Start Date</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ formatDate(campusStartDate) }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">End date</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ formatDate(campusEndDate) }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Contact person</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ contactPerson }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Program name</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ programApplicationObject?.programApplicationName }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Provincial certification types</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ programType }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Program Profile</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ programApplicationObject?.programProfileName }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Minimum students</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ programApplicationObject?.minimumEnrollment }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Maximum students</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ programApplicationObject?.maximumEnrollment }}
      </p>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { Components } from "@/types/openapi";
import type { NextStepPayload } from "@/components/program-application/ProgramApplication.vue";
import { mapProgramType, mapProgramStatus } from "@/api/program-application";
import { useUserStore } from "@/store/user";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "SatelliteProgramOverview",
  components: {},
  props: {
    programApplicationObject: {
      type: Object as PropType<Components.Schemas.ProgramApplication | null>,
      required: true,
    },
    contactPerson: {
      type: String,
      required: true,
    },
  },
  setup() {
    const userStore = useUserStore();
    return {
      userStore,
    };
  },
  emits: { next: (_payload: NextStepPayload) => true },
  computed: {
    statusText(): string {
      return mapProgramStatus(this.programApplicationObject?.status);
    },
    programType(): string {
      const types = this.programApplicationObject?.programTypes;
      if (!types?.length) return "—";
      return types.map(mapProgramType).join(", ");
    },
    campus(): string {
      if (this.programApplicationObject?.programCampuses) {
        return this.programApplicationObject?.programCampuses
          .map((c) => {
            return c.name;
          })
          .join(", ");
      }
      return "-";
    },
    campusStartDate(): string | null | undefined {
      if (this.programApplicationObject?.programCampuses) {
        return this.programApplicationObject?.programCampuses[0]?.startDate;
      }
      return "-";
    },
    campusEndDate(): string | null | undefined {
      if (this.programApplicationObject?.programCampuses) {
        return this.programApplicationObject?.programCampuses[0]?.endDate;
      }
      return "-";
    },
  },
  data() {
    return {};
  },
  async mounted() {},
  methods: {
    formatDate,
  },
});
</script>
