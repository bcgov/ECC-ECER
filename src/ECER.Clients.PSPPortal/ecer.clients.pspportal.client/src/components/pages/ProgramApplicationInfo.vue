<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <h1 class="mb-4">Application for a basic or post-basic program</h1>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <h2 class="mb-3">Application information</h2>
        <div class="d-flex flex-column ga-3">
          <p>
            Under the authority of the Community Care and Assisted Living Act and the Child Care Licensing Regulation (CCLR), the
            Director of the Early Childhood Educator Registry (Registry) sets standards for the certification of Early Childhood
            Educators (ECEs) in British Columbia. Recognized early childhood education programs are listed in Schedule D of the
            CCLR and have been evaluated and found to provide students with the knowledge, skills, and abilities required to meet
            this standard.
          </p>
          <p>
            This application solicits information to assess early childhood education programs against the standards as
            established in regulation, the
            <a href="https://www2.gov.bc.ca/assets/gov/education/early-learning/teach/ece/bc_occupational_competencies.pdf"
               class="text-primary text-decoration-underline"
               target="_blank"
            >B.C. Child Care Sector Occupational Competencies</a> (CCSOC), and the
            Linking Competencies document.
          </p>
          <div>
            <p>
              To apply for a post-basic program (Infant and Toddler Educator [ITE] and/or Special Needs Educator [SNE]), an
              institution must either:
            </p>
            <ul class="ml-10">
              <li>Already have a recognized early childhood education basic program, or</li>
              <li>Be applying for a basic and post-basic program concurrently</li>
            </ul>
          </div>
        </div>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <h3>First-time applications</h3>
        <ul class="ml-10">
          <li>A first-time application for a basic ECE program is restricted to one campus</li>
          <li>
            A first-time application for a basic ECE or post-basic ECE program is restricted to one delivery mode
            (for example, as a first-time applicant, if you are applying for an in-person basic ECE program, you would not be
            eligible to concurrently apply for an online program)
          </li>
        </ul>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <Callout title="Private institutions" type="warning">
          <p>
            Private institutions planning to offer early childhood education programs must first receive certification from the
            Ministry of Post-Secondary Education and Future Skills, Private Training Institutions Regulatory Unit.
          </p>
          <p>
            Once certification is received, the post-secondary institution may then apply to become a recognized early childhood
            education program. Visit
            <a href="http://www.privatetraininginstitutions.gov.bc.ca/institutions"
               class="text-primary text-decoration-underline"
               target="_blank"
            >Private Training Institutions</a> for more information.
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
              <li>When you continue, the application will be saved for you to complete</li>
            </ul>
          </div>
          <div>
            <h3>Completing your application</h3>
            <ul class="ml-10">
              <li>As your application will take some time to complete, you may save and exit at any point in the form to resume editing later</li>
              <li>You may complete the sections in any order</li>
              <li>You will be asked to review and make any edits to your responses prior to submitting your application to the ECE Registry</li>
            </ul>
          </div>
          <div>
            <h3>Program components</h3>
            <ul class="ml-10">
              <li>As the Registry's assessment is outcome-based, information must clearly outline plans and provide examples of the program design and how it will be implemented</li>
              <li>Clear, concise documentation is required for each component</li>
              <li>Some questions will ask you to upload documents to support your responses</li>
              <li class="ml-8 font-weight-bold">Note: Do not submit any documents containing personal details in this application (for example, staff resumes).</li>
            </ul>
          </div>
        </div>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <ECEHeader title="Assessment and recognition process" />
        <div class="d-flex flex-column ga-3 mt-4">
          <p>
            The Registry can begin your assessment after you submit your completed application package.
          </p>
          <p>
            Site visits may be required during the assessment to determine interim recognition and ongoing recognition.
          </p>
          <p>
            As early childhood education continues to grow and evolve, any program recognized under current
            competencies or standards will be required to upgrade if new standards are adopted. Recognized programs
            are subject to review.
          </p>

        </div>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <ECEHeader title="Related documents" />
        <div class="d-flex flex-column mt-4">
          <p>Child Care Sector Occupational Competencies</p>
          <p>Child Care Sector Occupational Competencies Assessment Tool</p>
          <p>Linking Competencies</p>
          <p class="mt-3">If you need additional documentation, please <router-link to="/messages/new">contact the ECE Registry</router-link>.</p>
        </div>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Callout from "@/components/common/Callout.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import PageContainer from "@/components/PageContainer.vue";

export default defineComponent({
  name: "ProgramApplicationInfo",
  components: { ECEHeader, Callout, PageContainer },
});
</script>

<style scoped>
</style>
