<template>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Educational Institution</p>
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
        {{ location }}
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
      <p class="small">Program profile</p>
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
  <v-row class="mb-n5" v-if="showDeliverySection()">
    <v-col cols="4">
      <p class="small">Online method(s) of instruction</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ onlineMethodsOfInstruction }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5" v-if="showDeliverySection()">
    <v-col cols="4">
      <p class="small">Delivery method for practicum instructor supervision</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ practicumInstructorDeliveryMethods }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Program enrollment options</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ enrollmentOptions }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5">
    <v-col cols="4">
      <p class="small">Admission options</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ admissionOptions }}
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
  <v-row class="mb-n5" v-if="isHybrid">
    <v-col cols="4">
      <p class="small">Percentage of online instructional hours</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ programApplicationObject?.onlineDeliveryHoursPercentage }}
      </p>
    </v-col>
  </v-row>
  <v-row class="mb-n5" v-if="isHybrid">
    <v-col cols="4">
      <p class="small">Percentage of in-person instructional hours</p>
    </v-col>
    <v-col>
      <p class="small font-weight-bold">
        {{ programApplicationObject?.inPersonHoursPercentage }}
      </p>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import {
  mapAdmissionOptions,
  mapDeliveryMethods,
  mapEnrollmentOptions,
  mapProgramType,
} from "@/api/program-application";
import { useUserStore } from "@/store/user";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "OnlineOrHybridProgramOverview",
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
  computed: {
    programType(): string {
      const types = this.programApplicationObject?.programTypes;
      if (!types?.length) return "-";
      return types.map(mapProgramType).join(", ");
    },
    enrollmentOptions(): string {
      const types = this.programApplicationObject?.enrollmentOptions;
      if (!types?.length) return "-";
      return types.map(mapEnrollmentOptions).join(", ");
    },
    admissionOptions(): string {
      const types = this.programApplicationObject?.admissionOptions;
      if (!types?.length) return "-";
      return types.map(mapAdmissionOptions).join(", ");
    },
    practicumInstructorDeliveryMethods(): string {
      const types = this.programApplicationObject?.deliveryMethod;
      if (!types?.length) return "-";
      return types.map(mapDeliveryMethods).join(", ");
    },
    location(): string {
      if (this.programApplicationObject?.programCampuses?.length) {
        return this.programApplicationObject.programCampuses
          .map((campus) => campus.name)
          .filter((name): name is string => Boolean(name))
          .join(", ");
      }
      return "-";
    },
    onlineMethodsOfInstruction(): string {
      const methods = this.programApplicationObject?.onlineMethodOfInstruction;
      if (!methods?.length) return "-";
      return methods.join(", ");
    },
    isHybrid(): boolean {
      return this.programApplicationObject?.deliveryType === "Hybrid";
    },
  },
  methods: {
    showDeliverySection(): boolean {
      return this.programApplicationObject?.deliveryType !== "Inperson";
    },
  },
});
</script>
