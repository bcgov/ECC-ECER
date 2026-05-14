<template>
  <v-row>
    <v-col cols="12">
      <h1 class="mb-4">
        Application for new campus at recognized private institution
      </h1>
    </v-col>
  </v-row>

  <v-row>
    <v-col cols="12">
      <h2 class="mb-3">Application information</h2>
      <div class="d-flex flex-column ga-3">
        <p>
          Under the authority of the Community Care and Assisted Living Act and
          the Child Care Licensing Regulation (CCLR), the director of the Early
          Childhood Educator Registry (ECE Registry) sets standards for the
          certification of early childhood educators in British Columbia.
          Recognized early childhood education programs are listed in Schedule D
          of the CCLR and have been evaluated and found to provide students with
          the knowledge, skills, and abilities required to meet this standard.
        </p>
        <p>
          This application solicits information to assess early childhood
          education programs against the standards as established in regulation,
          the
          <a
            href="https://www2.gov.bc.ca/assets/download/3C37875A5D1944DBB4224F0AEA481E74"
            class="text-primary text-decoration-underline"
            target="_blank"
          >
            B.C. Child Care Sector Occupational Competencies
          </a>
          (CCSOC), and the Linking Competencies document.
        </p>

        <p>
          This application is for private institutions currently recognized by
          the Registry that are seeking approval to offer their early childhood
          education program at a new campus location.
        </p>

        <p>
          The application is based on curriculum previously approved by the
          Registry. Any changes to the approved curriculum must be submitted to
          the Registry for review and approval.
        </p>
      </div>
    </v-col>
  </v-row>

  <v-row v-if="institutionType === 'Private'">
    <v-col cols="12">
      <Callout title="Private institutions" type="warning">
        <br />
        <p>
          Private institutions require additional approval from their regulator
          before adding a temporary offering of an early childhood education
          program at a location in partnership with another organization, or at
          a location outside of the institution.
        </p>
        <br />
        <p>
          Contact the
          <a
            href="http://www.privatetraininginstitutions.gov.bc.ca/institutions"
            class="text-primary text-decoration-underline"
            target="_blank"
          >
            Private Training Institutions Regulatory Unit
          </a>
          for more information.
        </p>
      </Callout>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12">
      <ECEHeader title="What to expect" />
      <div class="d-flex flex-column ga-3 mt-4">
        <div>
          <h3>To begin your application</h3>
          <ul class="ml-10">
            <li>Provide the requested institution and program information</li>
            <li>
              When you continue, the application will be saved for you to
              complete
            </li>
          </ul>
        </div>
        <div>
          <h3>Completing your application</h3>
          <ul class="ml-10">
            <li>
              As your application will take some time to complete, you may save
              and exit at any point in the form to resume editing later
            </li>
            <li>You may complete the sections in any order</li>
            <li>
              You will be asked to review and make any edits to your responses
              prior to submitting your application to the ECE Registry
            </li>
          </ul>
        </div>
        <div>
          <h3>Program components</h3>
          <ul class="ml-10">
            <li>
              As the ECE Registry's assessment is outcome-based, information
              must clearly outline plans and provide examples of the program
              design and how it will be implemented
            </li>
            <li>Clear, concise documentation is required for each component</li>
            <li>
              Some questions will ask you to upload documents to support your
              responses
            </li>
          </ul>
        </div>
        <div class="mt-5">
          <span class="font-weight-bold">
            Note: Do not submit any documents containing personal details in
            this application (for example, staff resumes).
          </span>
        </div>
      </div>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12">
      <ECEHeader title="Assessment and recognition process" />
      <div class="d-flex flex-column ga-3 mt-4">
        <p>
          The ECE Registry can begin your assessment after you submit your
          completed application package.
        </p>
        <p>
          A recognition evaluation meeting will be required during the
          assessment to determine interim recognition and ongoing recognition.
        </p>
        <p>
          Two recognition evaluation meetings are required during the
          assessment: one to determine interim recognition to start the program,
          and a second to determine ongoing recognition.
        </p>
        <p>
          As early childhood education continues to grow and evolve, any program
          recognized under current competencies or standards will be required to
          upgrade if new standards are adopted. Recognized programs are subject
          to review.
        </p>
      </div>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12">
      <ECEHeader title="Related documents" />
      <div class="d-flex flex-column mt-4">
        <a
          href="https://www2.gov.bc.ca/assets/download/3C37875A5D1944DBB4224F0AEA481E74"
          target="_blank"
          class="text-primary text-decoration-underline"
        >
          B.C. Child Care Sector Occupational Competencies
        </a>
        <a
          href="https://www2.gov.bc.ca/assets/download/C7D2FE264133456082776F45D4C69500"
          target="_blank"
          class="text-primary text-decoration-underline"
        >
          B.C. Child Care Sector Occupational Competencies Assessment Tool
        </a>
        <!-- prettier-ignore -->
        <p class="mt-3">
          If you need additional documentation, please
          <router-link
            to="/messages/new"
            class="text-primary text-decoration-underline"
          >
            contact the ECE Registry
          </router-link>.
        </p>
      </div>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import type { PropType } from "vue";
import type { PsiInstitutionType } from "@/types/openapi";
import Callout from "@/components/common/Callout.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";

export default defineComponent({
  name: "NewCampusProgramApplicationInfo",
  components: { Breadcrumb, ECEHeader, Callout, PageContainer },
  props: {
    institutionType: {
      type: String as PropType<PsiInstitutionType | undefined>,
      required: true,
    },
  },
});
</script>
