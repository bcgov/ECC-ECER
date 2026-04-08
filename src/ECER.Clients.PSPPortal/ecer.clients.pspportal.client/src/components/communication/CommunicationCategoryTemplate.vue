<template>
  <template v-if="category === 'ProgramChangeRequest'">
    <v-row class="mt-5">
      <v-col cols="12">
        <p class="mb-3">
          Notify the ECE Registry about changes to your program that affect
          program requirements or competencies. Include:
        </p>
        <ul class="ml-6 mb-3">
          <li>A description of the change</li>
          <li>Its effective date</li>
          <li>
            Relevant supporting documentation (for example, course outlines)
          </li>
        </ul>
        <p class="mb-3">
          The ECE Registry will review your request and follow up with you for
          additional information if needed.
        </p>
        <v-expansion-panels>
          <v-expansion-panel>
            <v-expansion-panel-title>
              <h3>Learn more about program changes</h3>
            </v-expansion-panel-title>
            <v-expansion-panel-text>
              <p class="mb-3">
                Program changes are divided into two categories:
              </p>
              <ol class="ml-10">
                <li>
                  Changes that do not require ECE Registry approval, which
                  include:
                  <ul style="list-style-type: disc">
                    <li>
                      Renaming course codes and course names without changes to
                      content
                    </li>
                    <li>
                      Reducing or increasing course hours while remaining within
                      the minimum hours for each area of instruction and not
                      altering the already approved competencies or learning
                      objectives for each area of instruction
                    </li>
                  </ul>
                </li>
                <li class="mt-3">
                  Changes that require ECE Registry approval, which include:
                  <ul style="list-style-type: disc">
                    <li>
                      Any changes that might alter the ECE Registry approved
                      program coursework that meets the minimum provincial
                      requirements for certification
                    </li>
                    <li>
                      Updating the course description or learning objectives
                      that might directly impact the student's ability to
                      demonstrate any of the required occupational standards set
                      out in the BC Child Care Sector Occupational Competencies
                    </li>
                    <li>
                      Removing a course if the required competencies are not
                      already covered in that area of instruction in the
                      approved program profile
                    </li>
                    <li>Adding a course to the approved program profile</li>
                  </ul>
                </li>
              </ol>
            </v-expansion-panel-text>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <Callout class="mt-3" type="warning">
          <h3>Need to make a change to a program profile?</h3>
          <p>
            For updates that do not affect requirements or competencies,
            <router-link :to="{ name: 'program-profiles' }">
              update your program profile
            </router-link>
            instead.
          </p>
        </Callout>
      </v-col>
    </v-row>
  </template>

  <v-row v-else-if="category === 'PracticumInquiry'" class="mt-5">
    <v-col cols="12">
      <p class="mb-3">Ask the ECE Registry about:</p>
      <ul class="ml-6 mb-3">
        <li>Practicum requirements</li>
        <li>Alternative setting practicum placements</li>
        <li>Making changes to your practicum policies</li>
        <li>Any other inquiries related to practicum</li>
      </ul>
      <p class="mb-3">
        Please include any relevant documentation (for example, practicum
        handbook for students, practicum handbook for instructors).
      </p>
      <p>The ECE Registry will review your inquiry and follow up with you.</p>
    </v-col>
  </v-row>

  <v-row v-else-if="category === 'ECEProgramApplicationInquiry'" class="mt-5">
    <v-col cols="12">
      <p class="mb-3">
        Ask the ECE Registry about your program application currently under
        review or inquire about submitting a new early childhood education
        program application and what options are available to you.
      </p>
      <p>The ECE Registry will review your inquiry and follow up with you.</p>
    </v-col>
  </v-row>

  <v-row
    v-else-if="category === 'ECEProgramApplicationRequirements'"
    class="mt-5"
  >
    <v-col cols="12">
      <p class="mb-3">
        Ask the ECE Registry about early childhood education program
        requirements (for example, administrative guidelines and policies,
        curriculum development and the BC Child Care Sector Occupational
        Competencies, online program delivery).
      </p>
      <p class="mb-3">Please include any relevant documentation.</p>
      <p>The ECE Registry will review your inquiry and follow up with you.</p>
    </v-col>
  </v-row>

  <v-row v-else-if="category === 'ProgramProfileInquiry'" class="mt-5">
    <v-col cols="12">
      <p class="mb-3">
        Ask the ECE Registry about a program profile (for example, questions
        about revising a current program profile or planned changes for an
        upcoming program profile).
      </p>
      <p class="mb-3">
        Please include the program profile(s) academic year you are inquiring
        about.
      </p>
      <p class="mb-3">
        For changes to a current program profile that do not impact program
        requirements or competencies, please submit a
        <router-link :to="{ name: 'program-profiles' }">
          program profile update
        </router-link>
        instead of submitting an inquiry.
      </p>
      <p>The ECE Registry will review your inquiry and follow up with you.</p>
    </v-col>
  </v-row>

  <v-row v-else-if="category === 'IndividualEducationPlanIEP'" class="mt-5">
    <v-col cols="12">
      <p class="mb-3">
        Ask the ECE Registry about developing an Individualized Education Plan
        (IEP) for a student who requires an accommodation (for example, support
        with a practicum placement in a remote area).
      </p>
      <p class="mb-3">Please include any relevant documentation.</p>
      <p>The ECE Registry will review your inquiry and follow up with you.</p>
    </v-col>
  </v-row>

  <v-row v-else-if="category === 'MeetingRequest'" class="mt-5">
    <v-col cols="12">
      <p class="mb-3">
        Request a meeting with a representative from the Post-Secondary Programs
        Team.
      </p>
      <p class="mb-3">
        Please provide a brief description of what you would like to discuss and
        questions you can share ahead of time.
      </p>
      <p>
        The ECE Registry will review your meeting request and follow up with
        you.
      </p>
    </v-col>
  </v-row>

  <v-row v-else-if="category === 'Other'" class="mt-5">
    <v-col cols="12">
      <p class="mb-3">
        The ECE Registry will review your message and follow up with you as soon
        as possible.
      </p>
      <p>
        Please note: If there are concerns about the character, skill or conduct
        of a certified ECE or ECE Assistant, please contact the ECE Registry's
        Investigation team. For more information about the Registry's
        Investigations Team and process, please visit
        <a
          href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/complaints"
          target="_blank"
          rel="noopener noreferrer"
        >
          Make a Complaint - Province of British Columbia
        </a>
        .
      </p>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import type { PropType } from "vue";
import { defineComponent } from "vue";
import Callout from "@/components/common/Callout.vue";
import type { InitiableCommunicationCategory } from "@/utils/communicationCategory";

export default defineComponent({
  name: "CommunicationCategoryTemplate",
  components: {
    Callout,
  },
  props: {
    category: {
      type: String as PropType<InitiableCommunicationCategory | null>,
      default: null,
    },
  },
});
</script>
