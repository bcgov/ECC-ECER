<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <h1 class="mb-4">Begin an application</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <div class="d-flex flex-column ga-3">
          <p>
            To begin your application, provide information about the program.
            When you have completed these questions, choose “Continue” to save
            your responses and create your draft application.
          </p>
          <p>
            You can save and exit the form and continue with your submission
            later from your PSP Portal dashboard.
          </p>
        </div>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <Callout title="If you need assistance" type="warning">
          <p>
            You may reach out to the Registry for assistance at any time by
            <router-link to="/messages/new">sending a message</router-link>
            <span>.</span>
          </p>
        </Callout>
      </v-col>
    </v-row>
    <v-form ref="beginApplicationForm">
      <v-row>
        <v-col cols="12">
          <div class="d-flex flex-column ga-3">
            <h3>Program information</h3>
            <EceTextField
              v-model="programName"
              label="Program name"
              :rules="[Rules.required('Enter a program name')]"
            ></EceTextField>
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <div class="d-flex flex-column ga-3">
            <p class="font-weight-bold">
              The following program information will determine the application
              questions you need to complete. You will not be able to edit these
              responses later.
            </p>
            <p>
              To apply to offer early childhood education program at a satelitte
              location, an institution must already have a recognized basic or
              post-basic early childhood education program.
            </p>

            <v-row>
              <v-col cols="6">
                <p class="pb-3">Program Profile</p>
                <v-select
                  v-model="selectedProgramProfileId"
                  class="mt-2"
                  variant="outlined"
                  :items="programProfiles"
                  item-title="name"
                  item-value="id"
                  :rules="[Rules.required('Required')]"
                  hide-details="auto"
                ></v-select>
                <p>
                  Select the Registry-recognized program for which you are
                  requesting approval to offer a satellite location.
                </p>
              </v-col>
            </v-row>
          </div>
        </v-col>
      </v-row>
      <v-row v-if="selectedProgramProfileId">
        <v-col>
          <p class="pb-3">Provincial certification type</p>
          <v-input
            v-model="provincialCertificationTypeValues"
            label="Provincial certification type"
            :rules="[
              Rules.atLeastOneOptionRequired(
                'Select a provincial certification type',
              ),
            ]"
            class="pb-2"
          >
            <v-row dense>
              <v-col v-for="type in programTypes" cols="12">
                <v-checkbox
                  v-model="provincialCertificationTypeValues"
                  :value="type"
                  :label="mapCertificationType(type)"
                  density="compact"
                  hide-details
                />
              </v-col>
            </v-row>
          </v-input>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <p class="pb-3">Delivery method(s)</p>
          <v-radio-group
            id="programConfirmationRadio"
            v-model="deliveryType"
            color="primary"
            :rules="[Rules.required('Select a delivery method')]"
          >
            <v-radio
              label="In-person - All coursework, including faculty practicum supervision, is in person"
              value="Inperson"
            ></v-radio>
            <v-radio
              label="Hybrid - Combination of in-person and online delivery; faculty practicum supervision is in person"
              value="Hybrid"
            ></v-radio>
            <v-radio
              label="Online - All coursework, including faculty practicum supervision, is online"
              value="Online"
            ></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <p class="pb-3">Location</p>
          <p class="pb-3">Select where this program will be administered.</p>
          <v-radio-group
            id="campusRadio"
            v-model="selectedCampusId"
            color="primary"
            :rules="[Rules.required('Select a campus')]"
          >
            <div
              v-for="campus in userStore.educationInstitution?.campuses?.filter(
                (x) => x.isSatelliteOrTemporaryLocation,
              )"
            >
              <v-radio
                :label="campus.generatedName || '-'"
                :value="campus.id || null"
              />
            </div>
          </v-radio-group>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <v-btn
            rounded="lg"
            color="primary"
            :loading="loadingStore.isLoading('program_application_post')"
            :disabled="loadingStore.isLoading('program_application_post')"
            @click="createApplication"
          >
            Continue
          </v-btn>
        </v-col>
      </v-row>
    </v-form>
  </PageContainer>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import ECEHeader from "@/components/ECEHeader.vue";
import type { VForm } from "vuetify/components";
import Callout from "@/components/common/Callout.vue";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import * as Rules from "@/utils/formRules";
import { useLoadingStore } from "@/store/loading";
import { getPrograms } from "@/api/program";
import type { Components } from "@/types/openapi";
import { useUserStore } from "@/store/user";
import type { CreateApplication } from "@/types/helperFunctions";

export default defineComponent({
  name: "BeginSatelliteApplication",
  components: {
    EceCheckbox,
    EceTextField,
    Breadcrumb,
    PageContainer,
    Callout,
    ECEHeader,
  },
  props: {
    campusId: {
      type: String,
      required: true,
    },
  },
  setup() {
    const loadingStore = useLoadingStore();
    const userStore = useUserStore();
    return { loadingStore, userStore };
  },
  emits: ["create-application"],
  data() {
    return {
      programName: "" as string,
      provincialCertificationTypeValues: [] as string[],
      deliveryType: "" as string,
      selectedProgramProfileId: "" as string,
      selectedCampusId: "" as string,
      Rules,
      createApplicationObject: {} as CreateApplication,
      programProfiles: [] as Components.Schemas.Program[],
    };
  },
  computed: {
    programTypes() {
      let selectedProfile = this.programProfiles.find(
        (profile) => profile.id === this.selectedProgramProfileId,
      );
      return selectedProfile?.programTypes;
    },
  },
  async mounted() {
    this.selectedCampusId = this.campusId;
    this.programProfiles =
      (await getPrograms(undefined, ["Approved"]))?.data?.programs || [];
  },
  methods: {
    async createApplication() {
      const { valid } = await (
        this.$refs.beginApplicationForm as VForm
      ).validate();

      if (!valid) return;

      this.createApplicationObject = {
        programName: this.programName,
        provincialCertificationTypeValues:
          this.provincialCertificationTypeValues,
        deliveryType: this.deliveryType,
        programProfileId: this.selectedProgramProfileId,
        campusId: this.selectedCampusId,
        applicationType: "SatelliteProgram",
      };
      this.$emit("create-application", this.createApplicationObject);
    },
    mapCertificationType(type: string) {
      switch (type) {
        case "Basic":
          return "Early childhood education (Basic)";
        case "ITE":
          return "Infant and Toddler Educator (ITE)";
        case "SNE":
          return "Special Needs Educator (SNE)";
        default:
          return "-";
      }
    },
  },
});
</script>
