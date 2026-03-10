<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>

    <Loading v-if="isLoading" />

    <div v-else-if="campus">
      <v-row>
        <v-col cols="12">
          <h1>Campus information</h1>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <CampusInformationCard
            :campus="campus"
            :institution-id="institutionId"
          />
        </v-col>
      </v-row>

      <!-- Programs offered -->
      <v-row class="mt-6">
        <v-col cols="12">
          <ECEHeader title="Programs offered" />
        </v-col>
      </v-row>

      <ProgramProfilesList v-if="programs.length > 0" :programs="programs" />

      <v-row v-if="programs.length === 0">
        <v-col cols="12">
          <p>No programs found.</p>
        </v-col>
      </v-row>

      <v-pagination
        v-if="programs.length > 0"
        v-model="currentProgramsPage"
        size="small"
        class="mt-4"
        elevation="2"
        :length="programsTotalPages"
      ></v-pagination>

      <!-- Active applications for this location -->
      <v-row class="mt-6">
        <v-col cols="12">
          <ECEHeader title="Active applications for this location" />
        </v-col>
      </v-row>

      <ProgramApplicationsList
        v-if="applications.length > 0"
        :applications="applications"
        @refresh-application-list="fetchApplications(applicationsPage)"
      />

      <v-row v-else>
        <v-col cols="12">
          <p>No active applications found.</p>
        </v-col>
      </v-row>

      <v-pagination
        v-if="applications.length > 0"
        v-model="currentApplicationsPage"
        size="small"
        class="mt-4"
        elevation="2"
        :length="applicationsTotalPages"
      ></v-pagination>
    </div>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Loading from "@/components/Loading.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import CampusInformationCard from "@/components/CampusInformationCard.vue";
import ProgramProfilesList from "@/components/ProgramProfilesList.vue";
import ProgramApplicationsList from "@/components/program-application/ProgramApplicationsList.vue";
import { getEducationInstitution } from "@/api/education-institution";
import { getPrograms } from "@/api/program";
import { getProgramApplications } from "@/api/program-application";
import { useLoadingStore } from "@/store/loading";
import type { Campus, Components } from "@/types/openapi";

const PAGE_SIZE = 9;

export default defineComponent({
  name: "Campus",
  components: {
    PageContainer,
    Loading,
    Breadcrumb,
    ECEHeader,
    CampusInformationCard,
    ProgramProfilesList,
    ProgramApplicationsList,
  },
  props: {
    institutionId: {
      type: String,
      required: true,
    },
    campusId: {
      type: String,
      required: true,
    },
  },
  setup() {
    const loadingStore = useLoadingStore();
    return { loadingStore };
  },
  data() {
    return {
      campus: null as Campus | null,
      programs: [] as Components.Schemas.Program[],
      programsCount: 0,
      programsPage: 1,
      applications: [] as Components.Schemas.ProgramApplication[],
      applicationsCount: 0,
      applicationsPage: 1,
    };
  },
  computed: {
    isLoading(): boolean {
      return (
        this.loadingStore.isLoading("education_institution_get") ||
        this.loadingStore.isLoading("program_get") ||
        this.loadingStore.isLoading("program_application_get")
      );
    },
    currentProgramsPage: {
      get() {
        return this.programsPage;
      },
      set(newValue: number) {
        this.programsPage = newValue;
        this.fetchPrograms(newValue);
      },
    },
    programsTotalPages(): number {
      return Math.ceil(this.programsCount / PAGE_SIZE);
    },
    currentApplicationsPage: {
      get() {
        return this.applicationsPage;
      },
      set(newValue: number) {
        this.applicationsPage = newValue;
        this.fetchApplications(newValue);
      },
    },
    applicationsTotalPages(): number {
      return Math.ceil(this.applicationsCount / PAGE_SIZE);
    },
  },
  async mounted() {
    await Promise.all([
      this.fetchCampus(),
      this.fetchPrograms(this.programsPage),
      this.fetchApplications(this.applicationsPage),
    ]);
  },
  methods: {
    async fetchCampus() {
      const institution = await getEducationInstitution();
      this.campus =
        institution?.campuses?.find((c) => c.id === this.campusId) ?? null;
    },
    async fetchPrograms(page: number) {
      const response = await getPrograms(
        "",
        ["Approved", "ChangeRequestInProgress", "UnderReview", "Draft"],
        { page, pageSize: PAGE_SIZE, campusId: this.campusId },
      );
      this.programs = response.data?.programs ?? [];
      this.programsCount = response.data?.totalProgramsCount ?? 0;
    },
    async fetchApplications(page: number) {
      const response = await getProgramApplications(
        { page, pageSize: PAGE_SIZE },
        "",
        [
          "Draft",
          "Submitted",
          "ReviewAnalysis",
          "InterimRecognition",
          "OnGoingRecognition",
        ],
        this.campusId,
      );
      this.applications = response.data?.applications ?? [];
      this.applicationsCount = response.data?.count ?? 0;
    },
  },
});
</script>
