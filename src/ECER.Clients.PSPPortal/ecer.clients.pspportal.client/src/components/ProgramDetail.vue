<template>
  <Loading v-if="loadingStore.isLoading('program_get')" />
  <template v-else>
    <v-sheet class="bg-primary">
      <v-container>
        <v-row>
          <v-col>
            <div>
              <strong>{{ program?.name }}</strong>
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
              <p class="small font-weight-bold">{{ program?.postSecondaryInstituteName }}</p>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="4">
              <p class="small">Start date</p>
            </v-col>
            <v-col>
              <p class="small font-weight-bold">{{ program?.startDate }}</p>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="4">
              <p class="small">End date</p>
            </v-col>
            <v-col>
              <p class="small font-weight-bold">{{ program?.endDate }}</p>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="4">
              <p class="small">Program types(s)</p>
            </v-col>
            <v-col>
              <p class="small font-weight-bold">{{ program?.programTypes?.join(", ") }}</p>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="4">
              <p class="small">Program name</p>
            </v-col>
            <v-col>
              <p class="small font-weight-bold">{{ program?.name }}</p>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
      <!-- Area of instructions -->
      <ProgramDetailAreaOfInstructionCard class="mb-4" programType="Basic" :program="program" />
      <ProgramDetailAreaOfInstructionCard class="mb-4" programType="ITE" :program="program" />
      <ProgramDetailAreaOfInstructionCard class="mb-4" programType="SNE" :program="program" />
      <v-row v-if="showUpdateProgramProfileButton">
        <v-col color="primary">
          <v-btn rounded="lg" color="primary">Update program profile TODO: Not Implemented</v-btn>
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
import { DateTime } from "luxon";

import Loading from "./Loading.vue";
import ProgramDetailAreaOfInstructionCard from "./ProgramDetailAreaOfInstructionCard.vue";

export default defineComponent({
  name: "Profile",
  components: { ProgramDetailAreaOfInstructionCard, Loading },
  setup: async () => {
    const loadingStore = useLoadingStore();
    const { mdAndDown, mobile } = useDisplay();
    const router = useRouter();

    return {
      loadingStore,
      mdAndDown,
      mobile,
      router,
    };
  },
  async mounted() {
    this.programProfiles = (await getPrograms(undefined, ["Draft", "Denied", "Approved", "UnderReview", "ChangeRequestInProgress", "Inactive"]))?.data || [];
  },
  data() {
    return {
      programProfiles: [] as Components.Schemas.Program[],
    };
  },
  props: {
    program: {
      type: Object as () => Components.Schemas.Program,
      required: true,
    },
  },
  computed: {
    showUpdateProgramProfileButton(): boolean {
      if (this.program?.status === "Inactive") {
        return false;
      }

      if (this.program?.status === "Approved" || this.program?.status === "UnderReview" || this.program?.status === "ChangeRequestInProgress") {
        return true;
      }

      if (!this.thisIsTheLatestProfile) {
        //another profile exists for the next year
        return false;
      }

      //default
      return false;
    },
    thisIsTheLatestProfile(): boolean {
      //check if there is a profile that exists next year
      this.programProfiles?.every((programProfile) => {
        return DateTime.fromISO(programProfile?.startDate || "") <= DateTime.fromISO(this.program?.startDate || "");
      });
      return true;
    },
  },
  methods: {
    printPage() {
      globalThis.print();
    },
  },
});
</script>
