<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <h1>Program Applications</h1>
      </v-col>
    </v-row>

    <Loading v-if="isLoading"></Loading>

    <div class="mt-4" v-else>
      <v-row>
        <v-col class="d-flex" cols="12">
          <p class="align-self-center mr-4"><strong>SHOW:</strong></p>
          <v-btn-toggle
            v-model="filter"
            color="primary"
            mandatory
            @update:model-value="fetchPrograms(page)"
          >
            <v-btn value="active">Active</v-btn>
            <v-btn value="inactive">Inactive</v-btn>
            <v-btn value="all">All</v-btn>
          </v-btn-toggle>
        </v-col>
      </v-row>
      <v-row v-if="programApplications.length > 0">
        <v-col
          v-for="(programApplication, index) in programApplications"
          :key="getProgramKey(programApplication, index)"
          cols="12"
          md="6"
          lg="4"
        >
          <ProgramApplicationCard :program-application="programApplication" />
        </v-col>
      </v-row>

      <!-- Empty state -->
      <v-row v-else>
        <v-col cols="12">
          <p>No program applications found.</p>
        </v-col>
      </v-row>

      <v-pagination
        v-if="programApplications.length > 0"
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
import ProgramApplicationCard from "@/components/program-application/ProgramApplicationCard.vue";
import { getProgramApplications } from "@/api/program-application";
import { useLoadingStore } from "@/store/loading";
import { useRouter } from "vue-router";
import type { Components } from "@/types/openapi";

const PAGE_SIZE = 10;

export default defineComponent({
  name: "ProgramApplications",
  components: {
    PageContainer,
    Loading,
    Breadcrumb,
    ProgramApplicationCard,
  },
  setup() {
    const loadingStore = useLoadingStore();
    const router = useRouter();
    return { loadingStore, router };
  },
  data() {
    return {
      programApplications: [] as Components.Schemas.ProgramApplication[],
      count: -1,
      page: 1,
      loading: true,
      filter: "active",
      activeStatus: [
        "Draft",
        "InterimRecognition",
        "OnGoingRecognition",
        "PendingReview",
        "ReviewAnalysis",
        "RFAI",
        "Submitted",
      ] as Components.Schemas.ApplicationStatus[],
      inactiveStatus: [
        "RefusetoApprove",
        "Withdrawn",
      ] as Components.Schemas.ApplicationStatus[],
    };
  },
  computed: {
    isLoading(): boolean {
      return (
        this.loadingStore.isLoading("program_application_get") || this.loading
      );
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
      return Math.ceil(this.programApplications.length / PAGE_SIZE);
    },
  },
  async mounted() {
    await this.fetchPrograms(this.page);
    this.loading = false;
  },
  methods: {
    getProgramKey(
      program: Components.Schemas.ProgramApplication,
      index: number,
    ): string {
      return program.id ?? `${program.status}-${index}`;
    },
    getStatues(): Components.Schemas.ApplicationStatus[] {
      if (this.filter === "active") {
        return this.activeStatus;
      } else if (this.filter === "inactive") {
        return this.inactiveStatus;
      } else {
        return [...this.activeStatus, ...this.inactiveStatus];
      }
    },
    async fetchPrograms(page: number) {
      const params = { page, pageSize: PAGE_SIZE };
      const response = await getProgramApplications(
        params,
        "",
        this.getStatues(),
      );
      this.programApplications = response.data?.applications || [];
      this.count = response.data?.count || 0;

      globalThis.scrollTo({ top: 0, behavior: "smooth" });
    },
  },
});
</script>
