<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <h1>All Program Profiles</h1>
      </v-col>
    </v-row>

    <Loading v-if="isLoading"></Loading>

    <div v-else>
      <v-row>
        <v-col>
          <p>
            This page contains program profiles for your institution dating back
            to {{ earliestProfileYear }}.
          </p>
        </v-col>
      </v-row>

      <v-row>
        <v-col>
          <!-- prettier-ignore -->
          <p>
            We are working to resolve technical issues affecting 2023/2024 and
            2024/2025 program profile data in the PSP Portal. If you require
            further assistance with a 2023/2024 or 2024/2025 program profile,
            please contact the PSP Team by
            <router-link :to="{ name: 'new-message' }">
              sending a message
            </router-link>.
          </p>
        </v-col>
      </v-row>

      <ProgramProfilesList
        :programs="allProgramProfiles"
        @withdrawn="fetchPrograms(page)"
      />
      <!-- Empty state -->
      <v-row v-if="allProgramProfiles.length === 0">
        <v-col cols="12">
          <p>No program profiles found.</p>
        </v-col>
      </v-row>

      <v-pagination
        v-if="programs.length > 0"
        v-model="currentPage"
        size="small"
        class="mt-4"
        elevation="2"
        :length="totalPages"
      ></v-pagination>
    </div>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Loading from "@/components/Loading.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import { getPrograms } from "@/api/program";
import { useLoadingStore } from "@/store/loading";
import type { Components } from "@/types/openapi";
import ECEHeader from "@/components/ECEHeader.vue";
import { EARLIEST_PROFILE_YEAR } from "@/utils/constant";
import ProgramProfilesList from "@/components/ProgramProfilesList.vue";

const PAGE_SIZE = 10;

export default defineComponent({
  name: "AllProgramProfiles",
  components: {
    ECEHeader,
    PageContainer,
    Loading,
    Breadcrumb,
    ProgramProfilesList,
  },
  setup() {
    const loadingStore = useLoadingStore();
    return { loadingStore };
  },
  data() {
    return {
      programs: [] as Components.Schemas.Program[],
      page: 1,
      loading: true,
    };
  },
  computed: {
    isLoading(): boolean {
      return this.loadingStore.isLoading("program_get") || this.loading;
    },
    allProgramProfiles(): Components.Schemas.Program[] {
      return this.programs.filter(
        (p) => p.status !== "Withdrawn" && p.status !== "Denied",
      );
    },
    earliestProfileYear(): number {
      return EARLIEST_PROFILE_YEAR;
    },
    currentPage: {
      get() {
        return this.page;
      },
      set(newValue: number) {
        this.page = newValue;
        this.fetchPrograms(newValue);
      },
    },
    totalPages() {
      return Math.ceil(this.programs.length / PAGE_SIZE);
    },
  },
  async mounted() {
    await this.fetchPrograms(this.page);
    this.loading = false;
  },
  methods: {
    async fetchPrograms(page: number) {
      const params = { page, pageSize: PAGE_SIZE };
      const response = await getPrograms("", [], params);
      this.programs = response.data?.programs || [];

      globalThis.scrollTo({ top: 0, behavior: "smooth" });
    },
  },
});
</script>
