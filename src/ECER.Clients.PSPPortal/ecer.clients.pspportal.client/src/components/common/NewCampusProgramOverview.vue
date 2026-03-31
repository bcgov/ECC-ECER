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
      <p class="small">Campus</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ campus }}
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
      <p class="small">Program type</p>
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
      <p class="small">Delivery method(s)</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ programApplicationObject?.deliveryType }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Program length</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ programApplicationObject?.programLength }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Program enrollment options</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ mapEnrollmentOptions }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Admission options</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ mapAdmissionOptions }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Minimum enrollment per course</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ programApplicationObject?.minimumEnrollment }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Maximum enrollment per course</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ programApplicationObject?.maximumEnrollment }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5" v-if="showDeliverySection()">
    <v-col cols="4">
      <p class="small">Online method(s) of instruction</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ programApplicationObject?.onlineMethodOfInstruction?.join(", ") }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5" v-if="showDeliverySection()">
    <v-col cols="4">
      <p class="small">Delivery method for practicum instructor supervision</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ mapDeliveryMethods }}
      </p>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { Components } from "@/types/openapi";
import type { NextStepPayload } from "@/components/program-application/ProgramApplication.vue";
import {
  mapProgramType,
  mapEnrollmentOptions,
  mapAdmissionOptions,
  mapDeliveryMethods,
  mapProgramStatus,
} from "@/api/program-application";
import { useUserStore } from "@/store/user";

export default defineComponent({
  name: "NewCampusProgramOverview",
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
    mapEnrollmentOptions() {
      const types = this.programApplicationObject?.enrollmentOptions;
      if (!types?.length) return "—";
      return types.map(mapEnrollmentOptions).join(", ");
    },
    mapAdmissionOptions() {
      const types = this.programApplicationObject?.admissionOptions;
      if (!types?.length) return "—";
      return types.map(mapAdmissionOptions).join(", ");
    },
    mapDeliveryMethods() {
      const types = this.programApplicationObject?.deliveryMethod;
      if (!types?.length) return "—";
      return types.map(mapDeliveryMethods).join(", ");
    },
    campus(): string {
      if (this.programApplicationObject?.programCampuses) {
        let programCampusIds =
          this.programApplicationObject?.programCampuses.map((c) => {
            if (c.campusId !== null && c.campusId !== undefined) {
              return c.campusId;
            }
          });

        let campusObj = this.userStore.educationInstitution?.campuses?.filter(
          (c) => {
            if (c.id !== null && c.id !== undefined) {
              return programCampusIds.includes(c.id);
            }
          },
        );
        console.log(campusObj);
        if (!campusObj?.length) return "-";
        return campusObj.map((c) => c.generatedName).join(", ");
      }
      return "-";
    },
  },
  data() {
    return {};
  },
  async mounted() {},
  methods: {
    showDeliverySection(): boolean {
      return this.programApplicationObject?.deliveryType !== "Inperson";
    },
  },
});
</script>
