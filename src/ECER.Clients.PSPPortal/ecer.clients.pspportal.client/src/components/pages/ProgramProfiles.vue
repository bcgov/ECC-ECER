<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <h1>Program Profiles</h1>
      </v-col>
    </v-row>

    <Loading v-if="isLoading"></Loading>

    <div v-else>
      <!-- Program profile review section - only shown when there are programs with Draft or UnderReview status -->
      <div v-if="programsRequiringReview.length > 0">
        <v-row>
          <v-col>
            <ECEHeader title="Program profile review" />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <div class="d-flex flex-column ga-3 pb-5">
              <p>
                The program profile review process is conducted annually by the EDE Registry to ensure requirements are met for continued recognition in B.C.
              </p>
              <p class="font-weight-bold">
                You will need to review your program profile for accuracy and to ensure that courses meet the minimum hours for each of the
                provincially-required areas of instruction.
              </p>
            </div>
            <v-sheet color="support-surface-info " class="w-100 px-12 py-7">
              <div class="text-support-border-info">
                <h2 class="mb-3">Review process</h2>
                <p>Click the "Review now" button to begin.</p>
                <ol class="ml-8 pb-3">
                  <li>Review the information we have for your program(s) and make changes if required.</li>
                  <li>Confirm and submit the program profile to the ECE Registry. You will be asked to sign off on the changes as the final step.</li>
                  <li>The ECE Registry will then proceed with their review as required.</li>
                </ol>
                <p>In some cases, the ECE Registry may request additional information about a program, particularly if changes were made.</p>
                <p class="font-weight-bold">You will be notified when the review process has been completed by the ECE Registry.</p>
              </div>
            </v-sheet>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <ECEHeader title="Program profiles requiring review" />
          </v-col>
        </v-row>
        <v-row>
          <v-col v-for="(program, index) in programsRequiringReview" :key="getProgramKey(program, index, 'review')" cols="12" md="6" lg="4">
            <ProgramProfileCard :program="program" />
          </v-col>
        </v-row>
      </div>

      <!-- Current program profiles section -->
      <v-row v-if="currentProgramProfiles.length > 0">
        <v-col>
          <ECEHeader title="Current program profiles" />
        </v-col>
      </v-row>
      <v-row>
        <v-col v-for="(program, index) in currentProgramProfiles" :key="getProgramKey(program, index, 'current')" cols="12" md="6" lg="4">
          <ProgramProfileCard :program="program" />
        </v-col>
      </v-row>

      <!-- Empty state -->
      <v-row v-if="programsRequiringReview.length === 0 && currentProgramProfiles.length === 0">
        <v-col cols="12">
          <p>No program profiles found.</p>
        </v-col>
      </v-row>
    </div>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Loading from "@/components/Loading.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import ProgramProfileCard from "@/components/program-profile/ProgramProfileCard.vue";
import { getPrograms } from "@/api/program";
import { useLoadingStore } from "@/store/loading";
import { useRouter } from "vue-router";
import type { Components } from "@/types/openapi";
import ECEHeader from "@/components/ECEHeader.vue";

export default defineComponent({
  name: "ProgramProfiles",
  components: {
    ECEHeader,
    PageContainer,
    Loading,
    Breadcrumb,
    ProgramProfileCard,
  },
  setup() {
    const loadingStore = useLoadingStore();
    const router = useRouter();
    return { loadingStore, router };
  },
  data() {
    return {
      programs: [] as Components.Schemas.Program[],
      loading: true,
    };
  },
  computed: {
    isLoading(): boolean {
      return this.loadingStore.isLoading("program_get") || this.loading;
    },
    programsRequiringReview(): Components.Schemas.Program[] {
      return this.programs.filter((p) => p.status === "Draft" || p.status === "UnderReview");
    },
    currentProgramProfiles(): Components.Schemas.Program[] {
      return this.programs.filter((p) => "Approved" === p.status || "Inactive" === p.status);
    },
  },
  async mounted() {
    await this.fetchPrograms();
    this.loading = false;
  },
  methods: {
    getProgramKey(program: Components.Schemas.Program, index: number, prefix: string): string {
      return program.id ?? `${prefix}-${index}`;
    },
    async fetchPrograms() {
      const { data: programs } = await getPrograms();
      this.programs = programs || [];
    },
  },
});
</script>
