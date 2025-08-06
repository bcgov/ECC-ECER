<template>
  <v-container>
    <Breadcrumb />
    <div class="d-flex flex-column ga-3 mb-10">
      <h1 class="mb-5">Work experience references</h1>
      <h2>{{ totalRequiredWorkExperienceHours }} hours of work experience is required.</h2>
      <p>Your hours:</p>
      <ul class="ml-10">
        <div v-if="applicationType === 'Renewal'">
          <li v-if="certificationStore.certificateStatus(applicationStatus?.fromCertificate) === 'Active'">
            Have been completed within the term of your current certificate (between the
            {{ formatDate(certificationStore.certificationEffectiveDate(applicationStatus?.fromCertificate) || "", "LLL d, yyyy") }} and
            {{ formatDate(certificationStore.certificationExpiryDate(applicationStatus?.fromCertificate) || "", "LLL d, yyyy") }})
          </li>
          <li v-if="certificationStore.certificateStatus(applicationStatus?.fromCertificate) === 'Expired'">Have been completed within the last 5 years</li>
        </div>
        <div v-else>
          <li>Must have been completed after you started your education and within the last 5 years</li>
        </div>

        <li>Cannot include hours worked as part of your education on your practicum or work placement</li>
        <li>Can be work or volunteer hours</li>
      </ul>
      <p>If you worked at multiple locations, add a reference for each location.</p>
    </div>
    <div v-for="(reference, index) in applicationStatus?.workExperienceReferencesStatus" :key="index">
      <ManageWorkExperienceReferenceListItem
        :reference="reference"
        :application-status="applicationStatus?.status ?? 'Submitted'"
        :go-to="
          () =>
            router.push({
              name: 'viewWorkExperienceReference',
              params: { applicationId: route.params.applicationId, referenceId: reference.id?.toString() },
            })
        "
      />
    </div>
    <v-card elevation="0" rounded="0" class="border-t">
      <v-card-text>
        <v-row class="d-flex" :class="[smAndUp ? 'justify-space-between align-center' : 'flex-column']">
          <v-col cols="4">
            <div>
              <p><b>Total hours</b></p>
            </div>
          </v-col>

          <v-col cols="12" sm="4" :align="smAndUp ? 'center' : ''">
            <p>
              <b>{{ totalHours }} hours</b>
            </p>
          </v-col>
          <v-spacer></v-spacer>
        </v-row>
      </v-card-text>
    </v-card>
    <div v-if="totalHours < totalRequiredWorkExperienceHours" class="mt-10">
      <p>You need {{ totalRequiredWorkExperienceHours - totalHours }} more hours of work experience.</p>
      <v-btn
        prepend-icon="mdi-plus"
        class="mt-10"
        color="primary"
        @click.prevent="router.push({ name: 'addWorkExperienceReference', params: { applicationId: applicationId } })"
      >
        Add reference
      </v-btn>
    </div>
    <Callout v-if="totalHours >= totalRequiredWorkExperienceHours && applicationStatus?.status != 'Submitted'" title="Please wait" type="warning" class="mt-10">
      No additional work references needed. You’ve provided the required hours. We will contact you shortly. We’re either waiting on a response from your
      reference or have not had a chance to assess the reference’s response yet.
    </Callout>
    <Callout
      v-if="totalHours >= totalRequiredWorkExperienceHours && applicationStatus?.status == 'Submitted'"
      title="Waiting for references to respond"
      type="warning"
      class="mt-10"
    >
      No additional work references needed. You’ve provided the required hours. We’re waiting on a response from your one or more of your references.
    </Callout>
    <div class="mt-10">
      <a href="#" @click.prevent="router.push({ name: 'manageApplication', params: { applicationId: applicationId } })">Back to application summary</a>
    </div>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useDisplay } from "vuetify";

import { getApplicationStatus } from "@/api/application";
import Callout from "@/components/Callout.vue";
import ManageWorkExperienceReferenceListItem from "@/components/ManageWorkExperienceReferenceListItem.vue";
import { useCertificationStore } from "@/store/certification";
import { WorkExperienceType } from "@/utils/constant";
import { formatDate } from "@/utils/format";
import Breadcrumb from "@/components/Breadcrumb.vue";

export default defineComponent({
  name: "ManageWorkExperienceReferenceList",
  components: {
    ManageWorkExperienceReferenceListItem,
    Callout,
    Breadcrumb,
  },
  props: {
    applicationId: {
      type: String,
      required: true,
    },
  },
  setup: async () => {
    const { smAndUp } = useDisplay();
    const router = useRouter();
    const route = useRoute();
    const certificationStore = useCertificationStore();
    const applicationStatus = (await getApplicationStatus(route.params.applicationId.toString()))?.data;

    return {
      applicationStatus,
      smAndUp,
      certificationStore,
      router,
      route,
    };
  },
  computed: {
    totalHours(): number {
      return (
        this.applicationStatus?.workExperienceReferencesStatus?.reduce((acc, reference) => {
          switch (reference.status) {
            case "ApplicationSubmitted":
              return acc + (reference.totalNumberofHoursAnticipated ?? 0);
            case "Approved":
              return acc + (reference.totalNumberofHoursApproved ?? 0);
            case "Rejected":
              return acc;
            case "InProgress":
            case "UnderReview":
            case "WaitingforResponse":
            case "Submitted":
              return acc + (reference.totalNumberofHoursObserved ?? 0);
            default:
              return acc;
          }
        }, 0) || 0
      );
    },
    totalRequiredWorkExperienceHours(): number {
      return this.applicationStatus?.workExperienceReferencesStatus?.every((reference) => reference.type === WorkExperienceType.IS_400_Hours) ? 400 : 500;
    },
    applicationType() {
      return this.applicationStatus?.applicationType;
    },
  },
  methods: {
    formatDate,
  },
});
</script>
