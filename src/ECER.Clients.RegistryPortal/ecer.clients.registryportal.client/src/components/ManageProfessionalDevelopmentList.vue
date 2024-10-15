<template>
  <v-container>
    <v-breadcrumbs class="pl-0" :items="items" color="primary">
      <template #divider>/</template>
    </v-breadcrumbs>

    <div class="d-flex flex-column ga-3 mb-10">
      <h1 class="mt-5">Professional development</h1>
      <div role="doc-subtitle">
        <b>
          {{ totalRequiredProfessionalDevelopmentHours }} hours of professional development relevant to early childhood education is required.
          <span v-if="totalHours < totalRequiredProfessionalDevelopmentHours">
            You need to add {{ totalRequiredProfessionalDevelopmentHours - totalHours }} more hours.
          </span>
        </b>
      </div>
      <p>Your hours:</p>
      <ul class="ml-10">
        <li v-if="latestCertification?.statusCode === 'Active'">
          Must have been completed within the term of your current certificate (between the
          {{ formatDate(latestCertification.effectiveDate!, "LLL d, yyyy") }} and {{ formatDate(latestCertification.expiryDate!, "LLL d, yyyy") }})
        </li>
        <li v-if="latestCertification?.statusCode === 'Expired'">Must have been completed within the last 5 years</li>
        <li>Can be work or volunteer hours</li>
        <li>Cannot include hours worked as part of your education on your practicum or work placement</li>
      </ul>
    </div>
    <div v-for="(professionalDevelopment, index) in applicationStatus?.professionalDevelopmentsStatus" :key="index">
      <ManageProfessionalDevelopmentListItem :professional-development="professionalDevelopment" />
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
    <div v-if="totalHours < totalRequiredProfessionalDevelopmentHours" class="mt-10">
      <p>You need {{ totalRequiredProfessionalDevelopmentHours - totalHours }} more hours of professional development.</p>
      <v-btn
        prepend-icon="mdi-plus"
        class="mt-10"
        color="primary"
        @click.prevent="router.push({ name: 'addProfessionalDevelopment', params: { applicationId: applicationId } })"
      >
        Add course or workshop
      </v-btn>
    </div>
    <Callout v-if="totalHours >= totalRequiredProfessionalDevelopmentHours" title="Please wait" type="warning" class="mt-10">
      No additional courses or workshops are needed. You’ve provided the required hours. We will contact you shortly after we've reviewed the new professional
      development courses or workshops added.
    </Callout>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useDisplay } from "vuetify";

import { getApplicationStatus } from "@/api/application";
import Callout from "@/components/Callout.vue";
import ManageProfessionalDevelopmentListItem from "@/components/ManageProfessionalDevelopmentListItem.vue";
import { useCertificationStore } from "@/store/certification";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "ManageProfessionalDevelopmentList",
  components: {
    ManageProfessionalDevelopmentListItem,
    Callout,
  },
  props: {
    applicationId: {
      type: String,
      required: true,
    },
  },
  setup: async () => {
    const { smAndUp } = useDisplay();
    const route = useRoute();
    const certificationStore = useCertificationStore();
    const applicationStatus = (await getApplicationStatus(route.params.applicationId.toString()))?.data;
    const latestCertification = certificationStore.latestCertification;
    const router = useRouter();

    return {
      applicationStatus,
      smAndUp,
      latestCertification,
      router,
    };
  },
  data() {
    return {
      items: [
        {
          title: "Home",
          disabled: false,
          href: "/",
        },
        {
          title: "Application",
          disabled: false,
          href: `/manage-application/${this.applicationId}`,
        },
        {
          title: "Professional development",
          disabled: true,
          href: `/manage-application/${this.applicationId}/professional-development`,
        },
      ],
    };
  },
  computed: {
    totalHours(): number {
      return (
        this.applicationStatus?.professionalDevelopmentsStatus
          ?.filter((item) => item.status !== "Rejected")
          .reduce((total, item) => total + (item.numberOfHours || 0), 0) || 0
      );
    },
    totalRequiredProfessionalDevelopmentHours(): number {
      return 40;
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
