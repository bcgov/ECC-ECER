<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>

    <Loading v-if="isLoading" />

    <div v-else-if="educationInstitution">
      <v-row>
        <v-col cols="12">
          <h1>Institution info</h1>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <EducationInstitutionCard
            :education-institution="educationInstitution"
            :hide-view-button="true"
          />
        </v-col>
      </v-row>

      <!-- Campus Locations -->
      <v-row class="mt-6">
        <v-col cols="12">
          <h2>Campus locations</h2>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <div class="d-flex flex-column ga-3">
            <p>
              A campus may be permanently associated with an institution or may
              be a temporary location, such as a location for a satellite
              program.
            </p>
            <p>Add a campus here if:</p>
            <ul class="ml-8">
              <li>
                Your institution has another location where an early childhood
                education program is offered, or
              </li>
              <li>
                If you are applying to offer an early childhood education
                program at another campus that is not listed here
              </li>
            </ul>
            <p>
              As part of the application process for a new program, you will
              need to have the applicable campus or location listed here.
            </p>
          </div>
        </v-col>
      </v-row>

      <v-row class="mt-2">
        <v-col cols="auto">
          <v-btn
            color="primary"
            size="large"
            @click="
              router.push({
                name: 'add-campus',
                params: { institutionId },
              })
            "
          >
            Add a campus
          </v-btn>
        </v-col>
      </v-row>

      <v-row v-if="campuses.length > 0" class="mt-4" align="stretch">
        <v-col
          v-for="(campus, index) in campuses"
          :key="campus.id ?? campus.generatedName ?? index"
          cols="12"
          sm="6"
          md="4"
          class="d-flex"
        >
          <CampusCard :campus="campus" @view="onViewCampus(campus)" />
        </v-col>
      </v-row>

      <!-- Satellite Locations -->
      <v-row class="mt-8">
        <v-col cols="12">
          <h2>Satellite locations</h2>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <div class="d-flex flex-column ga-3">
            <p>
              This process is for institutions with existing early childhood
              education programs that are recognized by the ECE Registry to seek
              approval to add a
              <strong>temporary offering</strong>
              of a program at a location in partnership with another
              organization, or at a location outside of the institution.
            </p>
            <!-- prettier-ignore -->
            <p>
              The satellite program offering runs in addition to existing
              programming and has a
              <strong>fixed end date</strong>.
            </p>
            <p>
              Institutions must hold ongoing recognition prior to submitting a
              request to offer a satellite program.
            </p>
          </div>
        </v-col>
      </v-row>

      <v-row class="mt-2">
        <v-col cols="auto">
          <v-btn
            color="primary"
            size="large"
            @click="
              router.push({
                name: 'add-satellite-location',
                params: { institutionId },
              })
            "
          >
            Add a satellite location
          </v-btn>
        </v-col>
      </v-row>

      <v-row v-if="satelliteLocations.length > 0" class="mt-4" align="stretch">
        <v-col
          v-for="(location, index) in satelliteLocations"
          :key="location.id ?? location.generatedName ?? index"
          cols="12"
          sm="6"
          md="4"
          class="d-flex"
        >
          <CampusCard :campus="location" @view="onViewCampus(location)" />
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
import EducationInstitutionCard from "@/components/EducationInstitutionCard.vue";
import CampusCard from "@/components/CampusCard.vue";
import { getEducationInstitution } from "@/api/education-institution";
import { useLoadingStore } from "@/store/loading";
import { useRouter } from "vue-router";
import type { EducationInstitution, Campus } from "@/types/openapi";

export default defineComponent({
  name: "EducationInstitution",
  components: {
    PageContainer,
    Loading,
    Breadcrumb,
    EducationInstitutionCard,
    CampusCard,
  },
  props: {
    institutionId: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      educationInstitution: null as EducationInstitution | null,
    };
  },
  async setup() {
    const loadingStore = useLoadingStore();
    const router = useRouter();
    return { loadingStore, router };
  },
  async mounted() {
    this.educationInstitution = await getEducationInstitution();
  },
  computed: {
    isLoading(): boolean {
      return this.loadingStore.isLoading("education_institution_get");
    },
    campuses(): Campus[] {
      return (this.educationInstitution?.campuses ?? []).filter(
        (c) => !c.isSatelliteOrTemporaryLocation,
      ) as Campus[];
    },
    satelliteLocations(): Campus[] {
      return (this.educationInstitution?.campuses ?? []).filter(
        (c) => c.isSatelliteOrTemporaryLocation,
      ) as Campus[];
    },
  },
  methods: {
    onViewCampus(campus: Campus) {
      this.router.push({
        name: "campus",
        params: { institutionId: this.institutionId, campusId: campus.id },
      });
    },
  },
});
</script>
