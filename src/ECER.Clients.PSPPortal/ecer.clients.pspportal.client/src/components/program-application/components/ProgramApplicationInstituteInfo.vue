<template>
  <!-- Online/Hybrid: full defaults, no overrides needed -->
  <ProgramApplicationInstituteInfoLayout
    v-if="applicationType === 'AddOnlineorHybridDeliveryMethod'"
    :program-application-id="programApplicationId"
    @next="$emit('next', $event)"
  />

  <!-- New Campus: hide campus section -->
  <ProgramApplicationInstituteInfoLayout
    v-else-if="applicationType === 'NewCampusatRecognizedPrivateInstitution'"
    :program-application-id="programApplicationId"
    @next="$emit('next', $event)"
  >
    <template #campus-section />
  </ProgramApplicationInstituteInfoLayout>

  <!-- Basic/Post-Basic: condensed summary, add post-basic disclaimer -->
  <ProgramApplicationInstituteInfoLayout
    v-else-if="applicationType === 'NewBasicECEPostBasicProgram'"
    :program-application-id="programApplicationId"
    @next="$emit('next', $event)"
  >
    <template
      #application-summary="{
        programType,
        institution,
        programApplicationObject,
      }"
    >
      <v-row>
        <v-col cols="3">Provincial Certification Type</v-col>
        <v-col class="font-weight-bold" cols="3">
          {{ programType }}
        </v-col>
      </v-row>
      <v-row class="mt-n3">
        <v-col cols="3">Delivery method</v-col>
        <v-col class="font-weight-bold" cols="3">
          {{ programApplicationObject?.deliveryType }}
        </v-col>
      </v-row>
      <v-row class="mt-n3">
        <v-col cols="3">Institution</v-col>
        <v-col class="font-weight-bold" cols="3">
          {{ institution?.name }}
        </v-col>
      </v-row>
    </template>

    <template #program-info-prefix>
      <v-row>
        <v-col cols="12">
          <p>
            To apply for a post-basic early childhood education program (ITE
            and/or SNE), an institution must either:
          </p>
          <ul class="ml-6">
            <li>
              Already have a recognized basic early childhood education program,
              or
            </li>
            <li>
              Apply for a basic early childhood education and post-basic early
              childhood education program concurrently.
            </li>
          </ul>
        </v-col>
      </v-row>
    </template>
  </ProgramApplicationInstituteInfoLayout>

  <!-- Satellite: hide campus, program length, delivery, enrollment; add date fields -->
  <ProgramApplicationInstituteInfoLayout
    v-else-if="applicationType === 'SatelliteProgram'"
    :program-application-id="programApplicationId"
    @next="$emit('next', $event)"
  >
    <template #campus-section></template>
    <template #program-length-field></template>
    <template #delivery-section></template>
    <template #enrollment-section></template>

    <template
      #satellite-fields="{
        satelliteStartDate,
        onUpdateSatelliteStartDate,
        satelliteEndDate,
        onUpdateSatelliteEndDate,
        endDateValidation,
        requiredValidation,
      }"
    >
      <v-row>
        <v-col cols="6">
          <EceDateInput
            :model-value="satelliteStartDate"
            @update:model-value="onUpdateSatelliteStartDate"
            label="Satellite location start date"
            :rules="[requiredValidation()]"
          ></EceDateInput>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="6">
          <EceDateInput
            :model-value="satelliteEndDate"
            @update:model-value="onUpdateSatelliteEndDate"
            label="Satellite location end date"
            :rules="[
              requiredValidation(),
              endDateValidation(satelliteStartDate),
            ]"
          ></EceDateInput>
        </v-col>
      </v-row>
    </template>
  </ProgramApplicationInstituteInfoLayout>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import ProgramApplicationInstituteInfoLayout from "./ProgramApplicationInstituteInfoLayout.vue";
import type { NextStepPayload } from "@/components/program-application/ProgramApplication.vue";
import EceDateInput from "@/components/inputs/EceDateInput.vue";

export default defineComponent({
  name: "ProgramApplicationInstituteInfo",
  components: { ProgramApplicationInstituteInfoLayout, EceDateInput },
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
    applicationType: {
      type: String,
      required: true,
    },
  },
  emits: { next: (_payload: NextStepPayload) => true },
});
</script>
