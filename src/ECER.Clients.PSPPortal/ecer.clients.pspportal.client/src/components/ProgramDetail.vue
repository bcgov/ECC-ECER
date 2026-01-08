<template>
  <Loading v-if="loadingStore.isLoading('program_get')" />
  <template v-else>
    <v-sheet class="bg-primary">
      <v-container>
        <v-row>
          <v-col>
            <div>
              <strong>{{ programProfile?.name }}</strong>
            </div>
          </v-col>
        </v-row>
      </v-container>
    </v-sheet>
    <v-container>
      <h1>Program Details</h1>
      <v-row>
        <v-col class="d-flex justify-end">
          <v-btn rounded="lg" variant="text" @click="printPage()">
            <v-icon color="secondary" icon="mdi-printer-outline" class="mr-2"></v-icon>
            <a class="small">Print Preview</a>
          </v-btn>
        </v-col>
      </v-row>

      <v-card class="mb-4" variant="outlined" rounded="lg">
        <v-card-title>
          <div class="d-flex justify-space-between align-center">
            <div>
              <h2 class="text-wrap">Program overview</h2>
            </div>
          </div>
        </v-card-title>
        <!-- CONTENT -->
        <v-card-text class="text-grey-dark">
          <v-row>
            <v-col cols="4">
              <p class="small">Institution name</p>
            </v-col>
            <v-col>
              <p class="small font-weight-bold">{{ programProfile?.postSecondaryInstituteName }}</p>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="4">
              <p class="small">Start date</p>
            </v-col>
            <v-col>
              <p class="small font-weight-bold">{{ programProfile?.startDate }}</p>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="4">
              <p class="small">End date</p>
            </v-col>
            <v-col>
              <p class="small font-weight-bold">{{ programProfile?.endDate }}</p>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="4">
              <p class="small">Program types(s)</p>
            </v-col>
            <v-col>
              <p class="small font-weight-bold">{{ programProfile?.programTypes?.join(", ") }}</p>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="4">
              <p class="small">Program name</p>
            </v-col>
            <v-col>
              <p class="small font-weight-bold">TODO I think this is missing from the mappings</p>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
      <!-- Area of instructions -->
      <ProgramDetailAreaOfInstructionCard class="mb-4" programType="Basic" :programProfile="programProfile" />
      <ProgramDetailAreaOfInstructionCard class="mb-4" programType="ITE" :programProfile="programProfile" />
      <ProgramDetailAreaOfInstructionCard class="mb-4" programType="SNE" :programProfile="programProfile" />
      <v-row v-if="showUpdateProgramProfileButton">
        <v-col class="d-flex justify-end">
          <v-btn rounded="lg" variant="text">
            <v-icon color="secondary" icon="mdi-printer-outline" class="mr-2"></v-icon>
            <div class="small">Update program profile TODO: Not Implemented</div>
          </v-btn>
        </v-col>
      </v-row>
    </v-container>
  </template>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";
import { type Components } from "@/types/openapi";

import { useLoadingStore } from "@/store/loading";
import { useRouter } from "vue-router";
import { getPrograms } from "@/api/program";

import Loading from "./Loading.vue";
import ProgramDetailAreaOfInstructionCard from "./ProgramDetailAreaOfInstructionCard.vue";

export default defineComponent({
  name: "Profile",
  components: { ProgramDetailAreaOfInstructionCard, Loading },
  setup: async () => {
    const loadingStore = useLoadingStore();
    const { mdAndDown, mobile } = useDisplay();
    const router = useRouter();

    const programProfiles = await getPrograms();
    console.log(programProfiles);

    return {
      loadingStore,
      mdAndDown,
      mobile,
      router,
    };
  },
  data() {
    return {
      programProfile: undefined as Components.Schemas.Program | undefined,
    };
  },
  props: {
    programId: {
      type: String,
      required: true,
    },
  },

  // CreateMap<ProgramStatus, ecer_Program_StatusCode>()
  // .ConvertUsing(status =>
  //     status == ProgramStatus.Draft ? ecer_Program_StatusCode.RequiresReview :
  //     status == ProgramStatus.UnderReview ? ecer_Program_StatusCode.UnderRegistryReview :
  //     status == ProgramStatus.Approved ? ecer_Program_StatusCode.RegistryReviewComplete :
  //     status == ProgramStatus.Denied ? ecer_Program_StatusCode.Denied :
  //     status == ProgramStatus.Inactive ? ecer_Program_StatusCode.Inactive :
  //                                           ecer_Program_StatusCode.RequiresReview);
  computed: {
    showUpdateProgramProfileButton(): boolean {
      if (this.programProfile?.status === "Inactive") {
        return false;
      }

      //registry review complete
      if (this.programProfile?.status === "Approved") {
        return true;
      }

      if (this.programProfile?.status === "UnderReview") {
        return true;
      }
      //need change request in progress

      //default
      return false;
    },
  },
  async mounted() {
    this.programProfile = (await getPrograms(this.programId)).data?.[0];
  },
  methods: {
    printPage() {
      globalThis.print();
    },
  },
});
</script>
