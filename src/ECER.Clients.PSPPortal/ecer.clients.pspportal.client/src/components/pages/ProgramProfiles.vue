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
        <div v-if="!onlyChangeRequestInProgress">
          <v-row>
            <v-col>
              <ECEHeader title="Program profile review" />
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12">
              <div class="d-flex flex-column ga-3 pb-5">
                <p>
                  The program profile review process is conducted annually by
                  the ECE Registry to ensure requirements are met for continued
                  recognition in B.C.
                </p>
                <p class="font-weight-bold">
                  You will need to review your program profile for accuracy and
                  to ensure that courses meet the minimum hours for each of the
                  provincially-required areas of instruction.
                </p>
              </div>
              <v-sheet color="support-surface-info " class="w-100 px-12 py-7">
                <div class="text-support-border-info">
                  <h2 class="mb-3">Review process</h2>
                  <p>Click the "Review now" button to begin.</p>
                  <ol class="ml-8 pb-3">
                    <li>
                      Review the information we have for your program(s) and
                      make changes if required.
                    </li>
                    <li>
                      Confirm and submit the program profile to the ECE
                      Registry. You will be asked to sign off on the changes as
                      the final step.
                    </li>
                    <li>
                      The ECE Registry will then proceed with their review as
                      required.
                    </li>
                  </ol>
                  <p>
                    In some cases, the ECE Registry may request additional
                    information about a program, particularly if changes were
                    made.
                  </p>
                  <p class="font-weight-bold">
                    You will be notified when the review process has been
                    completed by the ECE Registry.
                  </p>
                </div>
              </v-sheet>
            </v-col>
          </v-row>
        </div>
        <v-row>
          <v-col>
            <ECEHeader title="Program profiles requiring review" />
          </v-col>
        </v-row>
        <ProgramProfilesList
          :programs="programsRequiringReview"
          @withdrawn="fetchPrograms"
        />
      </div>

      <!-- Current program profiles section -->
      <v-row>
        <v-col>
          <ECEHeader title="Current program profiles" />
        </v-col>
      </v-row>
      <ProgramProfilesList
        :programs="currentProgramProfiles"
        @withdrawn="fetchPrograms"
      />
      <!-- Empty state -->
      <v-row
        v-if="
          programsRequiringReview.length === 0 &&
          currentProgramProfiles.length === 0
        "
      >
        <v-col cols="12">
          <p>No program profiles found.</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <div>
            <v-btn
              block
              size="x-large"
              variant="outlined"
              color="primary"
              @click="router.push('/all-program-profiles')"
              class="force-full-content"
            >
              View all program profiles
              <v-icon size="large" icon="mdi-arrow-right" />
            </v-btn>
          </div>
        </v-col>
      </v-row>
    </div>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { DateTime } from "luxon";
import PageContainer from "@/components/PageContainer.vue";
import Loading from "@/components/Loading.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import ProgramProfileCard from "@/components/program-profile/ProgramProfileCard.vue";
import ProgramProfilesList from "@/components/ProgramProfilesList.vue";
import { getPrograms } from "@/api/program";
import { useLoadingStore } from "@/store/loading";
import { useRouter } from "vue-router";
import type { Components } from "@/types/openapi";
import ECEHeader from "@/components/ECEHeader.vue";

const PAGE_SIZE = 0;

export default defineComponent({
  name: "ProgramProfiles",
  components: {
    ECEHeader,
    PageContainer,
    Loading,
    Breadcrumb,
    ProgramProfileCard,
    ProgramProfilesList,
  },
  setup() {
    const loadingStore = useLoadingStore();
    const router = useRouter();
    return { loadingStore, router };
  },
  data() {
    return {
      programs: [] as Components.Schemas.Program[],
      programCount: -1,
      page: 1,
      loading: true,
    };
  },
  computed: {
    isLoading(): boolean {
      return this.loadingStore.isLoading("program_get") || this.loading;
    },
    programsRequiringReview(): Components.Schemas.Program[] {
      return this.programs.filter(
        (p) => p.status === "Draft" || p.status === "UnderReview",
      );
    },
    onlyChangeRequestInProgress(): boolean {
      return !this.programsRequiringReview.some(
        (p) => p.programProfileType === "AnnualReview",
      );
    },
    currentProgramProfiles(): Components.Schemas.Program[] {
      return this.programs.filter(
        (p) =>
          "Approved" === p.status || "ChangeRequestInProgress" === p.status,
      );
    },
    currentYearStart(): Date {
      let yearStart;
      const today = new Date();
      if (today.getMonth() > 8) {
        yearStart = new Date(today.setMonth(8, 1));
      } else {
        yearStart = new Date(today.setFullYear(today.getFullYear() - 1, 8, 1));
      }
      yearStart.setHours(0, 0, 0, 0);
      return yearStart;
    },
  },
  async mounted() {
    await this.fetchPrograms();
    this.loading = false;
  },
  methods: {
    async fetchPrograms(page: number = 1) {
      const params = { page, pageSize: PAGE_SIZE };
      const response = await getPrograms("", [], params);
      this.programs = response.data?.programs || [];
      this.programCount = response.data?.totalProgramsCount || 0;

      globalThis.scrollTo({ top: 0, behavior: "smooth" });
    },
  },
});
</script>

<style scoped>
::v-deep(.force-full-content .v-btn__content) {
  flex: 1 1 auto !important;
  justify-content: space-between;
}
</style>
