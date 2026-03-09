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

      <ProgramProfilesList :programs="programs" />

      <v-row v-if="programs.length === 0">
        <v-col cols="12">
          <p>No programs found for this campus.</p>
        </v-col>
      </v-row>

      <!-- Active applications for this location -->
      <v-row class="mt-6">
        <v-col cols="12">
          <ECEHeader title="Active applications for this location" />
        </v-col>
      </v-row>

      <v-row v-if="activeApplications.length > 0" align="stretch">
        <v-col
          v-for="(application, index) in activeApplications"
          :key="application.id ?? index"
          cols="12"
          sm="6"
          class="d-flex"
        >
          <ProgramApplicationCard
            :program-application="application"
            @refresh-application-list="fetchApplications"
          />
        </v-col>
      </v-row>

      <v-row v-else>
        <v-col cols="12">
          <p>No active applications for this location.</p>
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
import ECEHeader from "@/components/ECEHeader.vue";
import CampusInformationCard from "@/components/CampusInformationCard.vue";
import ProgramProfilesList from "@/components/ProgramProfilesList.vue";
import ProgramApplicationCard from "@/components/program-application/ProgramApplicationCard.vue";
import { getEducationInstitution } from "@/api/education-institution";
import { getPrograms } from "@/api/program";
import { getProgramApplications } from "@/api/program-application";
import { useLoadingStore } from "@/store/loading";
import type { Campus, Components } from "@/types/openapi";

const ACTIVE_APPLICATION_STATUSES: Components.Schemas.ApplicationStatus[] = [
  "Draft",
  "Submitted",
  "ReviewAnalysis",
  "InterimRecognition",
  "OnGoingRecognition",
];

export default defineComponent({
  name: "Campus",
  components: {
    PageContainer,
    Loading,
    Breadcrumb,
    ECEHeader,
    CampusInformationCard,
    ProgramProfilesList,
    ProgramApplicationCard,
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
      applications: [] as Components.Schemas.ProgramApplication[],
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
    activeApplications(): Components.Schemas.ProgramApplication[] {
      return this.applications.filter((app) =>
        app.programCampuses?.some((c) => c.campusId === this.campusId),
      );
    },
  },
  async mounted() {
    await Promise.all([
      this.fetchCampus(),
      this.fetchPrograms(),
      this.fetchApplications(),
    ]);
  },
  methods: {
    async fetchCampus() {
      const institution = await getEducationInstitution();
      this.campus =
        institution?.campuses?.find((c) => c.id === this.campusId) ?? null;
    },
    async fetchPrograms() {
      const response = await getPrograms("", [
        "Approved",
        "ChangeRequestInProgress",
        "UnderReview",
        "Draft",
      ]);
      this.programs = response.data?.programs ?? [];
    },
    async fetchApplications() {
      const response = await getProgramApplications(
        { page: 1, pageSize: 100 },
        "",
        ACTIVE_APPLICATION_STATUSES,
      );
      this.applications = response.data?.applications ?? [];
    },
  },
});
</script>
