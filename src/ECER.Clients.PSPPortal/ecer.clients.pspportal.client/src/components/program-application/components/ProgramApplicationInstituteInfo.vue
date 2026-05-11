<template>
  <!-- Online/Hybrid: campus changed to location input conditional input for hybrid deliveryType -->
  <ProgramApplicationInstituteInfoLayout
    ref="instituteInfoLayoutRef"
    v-if="
      applicationType === 'AddOnlineorHybridDeliveryMethod' &&
      programApplicationObject !== null
    "
    :programApplicationObject="programApplicationObject"
    :is-rfai="isRFAI"
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
      <template
        v-if="
          programApplicationObject.deliveryType === 'Hybrid' &&
          programApplicationObject !== null
        "
      >
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
                  Rules.validNumber(),
                  Rules.numberWithinRange(0, 100, false),
                  Rules.numberToDecimalPlaces(2),
                ]"
                @update:model-value="onUpdateInPersonHoursPercentage"
                :readonly="isRFAI"
              ></EceTextField>
            </div>
          </v-col>
          <v-col cols="4">
            <div class="d-flex flex-column ga-3">
              <EceTextField
                :model-value="onlineDeliveryHoursPercentage"
                label="Online delivery"
                :rules="[
                  Rules.validNumber(),
                  Rules.numberWithinRange(0, 100, false),
                  Rules.numberToDecimalPlaces(2),
                ]"
                @update:model-value="onUpdateOnlineDeliveryHoursPercentage"
                :readonly="isRFAI"
              ></EceTextField>
            </div>
          </v-col>
        </v-row>
      </template>
    </template>
  </ProgramApplicationInstituteInfoLayout>

  <!-- New Campus: hide campus section -->
  <ProgramApplicationInstituteInfoLayout
    ref="instituteInfoLayoutRef"
    v-else-if="
      applicationType === 'NewCampusatRecognizedPrivateInstitution' &&
      programApplicationObject !== null
    "
    :programApplicationObject="programApplicationObject"
    :is-rfai="isRFAI"
    @next="$emit('next', $event)"
  >
    <template #campus-section />
  </ProgramApplicationInstituteInfoLayout>

  <!-- Basic/Post-Basic: condensed summary, add post-basic disclaimer -->
  <ProgramApplicationInstituteInfoLayout
    ref="instituteInfoLayoutRef"
    v-else-if="
      applicationType === 'NewBasicECEPostBasicProgram' &&
      programApplicationObject !== null
    "
    :programApplicationObject="programApplicationObject"
    :is-rfai="isRFAI"
    @next="$emit('next', $event)"
  >
    <template #application-summary="{ programType, institution, deliveryType }">
      <v-row>
        <v-col cols="12" sm="4" xl="3">Provincial Certification Type</v-col>
        <v-col class="font-weight-bold" cols="12" sm="8" xl="9">
          {{ programType }}
        </v-col>
      </v-row>
      <v-row class="mt-n3">
        <v-col cols="12" sm="4" xl="3">Delivery method</v-col>
        <v-col class="font-weight-bold" cols="12" sm="8" xl="9">
          {{ deliveryType }}
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

    <template
      #delivery-hours-section-for-online-hybrid="{
        programApplicationObject,
        inPersonHoursPercentage,
        onlineDeliveryHoursPercentage,
        onUpdateInPersonHoursPercentage,
        onUpdateOnlineDeliveryHoursPercentage,
      }"
    >
      <template
        v-if="
          programApplicationObject !== null &&
          programApplicationObject.deliveryType === 'Hybrid'
        "
      >
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
                  Rules.validNumber(),
                  Rules.numberWithinRange(0, 100, false),
                  Rules.numberToDecimalPlaces(2),
                ]"
                @update:model-value="onUpdateInPersonHoursPercentage"
                :readonly="isRFAI"
              ></EceTextField>
            </div>
          </v-col>
          <v-col cols="4">
            <div class="d-flex flex-column ga-3">
              <EceTextField
                :model-value="onlineDeliveryHoursPercentage"
                label="Online delivery"
                :rules="[
                  Rules.validNumber(),
                  Rules.numberWithinRange(0, 100, false),
                  Rules.numberToDecimalPlaces(2),
                ]"
                @update:model-value="onUpdateOnlineDeliveryHoursPercentage"
                :readonly="isRFAI"
              ></EceTextField>
            </div>
          </v-col>
        </v-row>
      </template>
    </template>
  </ProgramApplicationInstituteInfoLayout>

  <!-- Satellite: hide campus, program length, delivery, enrollment; add date fields -->
  <ProgramApplicationInstituteInfoLayout
    ref="instituteInfoLayoutRef"
    v-else-if="
      applicationType === 'SatelliteProgram' &&
      programApplicationObject !== null
    "
    :programApplicationObject="programApplicationObject"
    :is-rfai="isRFAI"
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
            :readonly="isRFAI"
          ></EceDateInput>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="6">
          <EceDateInput
            :model-value="satelliteEndDate"
            @update:model-value="onUpdateSatelliteEndDate"
            label="Satellite location end date"
            :readonly="isRFAI"
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
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import * as Rules from "@/utils/formRules";
import { getProgramApplicationById } from "@/api/program-application";
import type { Components } from "@/types/openapi";
import { cloneDeep } from "lodash";

export default defineComponent({
  name: "ProgramApplicationInstituteInfo",
  components: {
    ProgramApplicationInstituteInfoLayout,
    EceDateInput,
    EceTextField,
    ConfirmationDialog,
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
  async beforeRouteLeave(_to, _from, next) {
    const instituteInfoLayout = this.$refs
      .instituteInfoLayoutRef as InstanceType<
      typeof ProgramApplicationInstituteInfoLayout
    >;
    const leave = await instituteInfoLayout.waitForConfirmation();
    next(leave);
  },
  async beforeRouteUpdate(_to, _from, next) {
    const instituteInfoLayout = this.$refs
      .instituteInfoLayoutRef as InstanceType<
      typeof ProgramApplicationInstituteInfoLayout
    >;
    const leave = await instituteInfoLayout.waitForConfirmation();
    next(leave);
  },
  async mounted() {
    await this.fetchApplication();
  },
  computed: {
    isRFAI(): boolean {
      return (
        this.programApplicationObject?.status !== undefined &&
        (this.programApplicationObject?.status === "InterimRecognition" ||
          this.programApplicationObject?.status === "ReviewAnalysis") &&
        this.programApplicationObject?.statusReasonDetail === "RFAIrequested"
      );
    },
  },
  data() {
    return {
      Rules,
      programApplicationObject:
        null as Components.Schemas.ProgramApplication | null,
    };
  },
  emits: { next: (_payload: NextStepPayload) => true, refreshNav: () => true },
  methods: {
    async fetchApplication() {
      const result = await getProgramApplicationById(this.programApplicationId);
      if (result.error || result.data == null) {
        console.error("Failed to retrieve program application:", result.error);
      } else {
        this.programApplicationObject = cloneDeep(result.data);
      }
    },
  },
});
</script>
