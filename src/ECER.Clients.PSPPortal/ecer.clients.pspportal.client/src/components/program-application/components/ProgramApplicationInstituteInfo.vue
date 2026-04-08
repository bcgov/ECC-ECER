<template>
  <!-- Online/Hybrid: campus changed to location input conditional input for hybrid deliveryType -->
  <ProgramApplicationInstituteInfoLayout
    v-if="applicationType === 'AddOnlineorHybridDeliveryMethod'"
    :program-application-id="programApplicationId"
    @next="$emit('next', $event)"
  >
    <template #campus-section-title><h2>Location</h2></template>
    <template #campus-section-subtitle>
      <p>
        Select where this program will be offered. A first-time application for
        a basic early childhood education program is restricted to one location.
      </p>
      <br />
    </template>
    <template
      #delivery-hours-section-for-online-hybrid="{
        programApplicationObject,
        inPersonHoursPercentage,
        onlineDeliveryHoursPercentage,
        onUpdateInPersonHoursPercentage,
        onUpdateOnlineDeliveryHoursPercentage,
      }"
    >
      <template v-if="programApplicationObject.deliveryType === 'Hybrid'">
        <v-row>
          <v-col cols="12">
            <p>Approximate percentage of instructional hours (hybrid only)</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <div class="d-flex flex-column ga-3">
              <EceTextField
                :model-value="inPersonHoursPercentage"
                label="In-person delivery"
                :rules="[
                  Rules.numberToDecimalPlaces(2),
                  Rules.numberWithinRange(0, 100, false),
                ]"
                @update:model-value="onUpdateInPersonHoursPercentage"
              ></EceTextField>
            </div>
          </v-col>
          <v-col cols="4">
            <div class="d-flex flex-column ga-3">
              <EceTextField
                :model-value="onlineDeliveryHoursPercentage"
                label="Online delivery"
                :rules="[
                  Rules.numberToDecimalPlaces(2),
                  Rules.numberWithinRange(0, 100, false),
                ]"
                @update:model-value="onUpdateOnlineDeliveryHoursPercentage"
              ></EceTextField>
            </div>
          </v-col>
        </v-row>
      </template>
    </template>
  </ProgramApplicationInstituteInfoLayout>

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
        <v-col cols="12" sm="4" xl="3">Provincial Certification Type</v-col>
        <v-col class="font-weight-bold" cols="12" sm="8" xl="9">
          {{ programType }}
        </v-col>
      </v-row>
      <v-row class="mt-n3">
        <v-col cols="12" sm="4" xl="3">Delivery method</v-col>
        <v-col class="font-weight-bold" cols="12" sm="8" xl="9">
          {{ programApplicationObject?.deliveryType }}
        </v-col>
      </v-row>
      <v-row class="mt-n3">
        <v-col cols="12" sm="4" xl="3">Institution</v-col>
        <v-col class="font-weight-bold" cols="12" sm="8" xl="9">
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
import EceTextField from "@/components/inputs/EceTextField.vue";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "ProgramApplicationInstituteInfo",
  components: {
    ProgramApplicationInstituteInfoLayout,
    EceDateInput,
    EceTextField,
  },
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
  data() {
    return {
      Rules,
    };
  },
  emits: { next: (_payload: NextStepPayload) => true },
});
</script>
