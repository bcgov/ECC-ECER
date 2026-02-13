<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <h1>Update a program profile</h1>
        <br />
      </v-col>
    </v-row>

    <Loading v-if="isLoading"></Loading>

    <div v-else>
      <div>
        <v-row>
          <v-col>
            <div>
              <svg width="36" height="4">
                <rect width="36" height="4" fill="#FFC72C" />
              </svg>
              <h2>{{ programTitle }}</h2>
            </div>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <p>
              Make updates to your program profile that do not affect program
              requirements or competencies. You can indicate the date on which
              your changes come into effect.
            </p>
            <br />
            <p>
              Review your program profile by clicking on the button below and
              update any of the following:
            </p>
            <br />
            <ul class="ml-8">
              <li>Program name</li>
              <li>Program start date</li>
              <li>Course code</li>
              <li>Course name</li>
              <li>
                Course hours (Note: total hours and competency requirements for
                all areas of instruction must be met)
              </li>
            </ul>
          </v-col>
        </v-row>
        <br />
        <Callout class="mt-3" type="warning">
          <div class="d-flex flex-column ga-3">
            <h3>Need to make a change to a program?</h3>
            <p>
              If your update will affect program requirements or competencies,
              please
              <a @click="submitChangeRequest()">
                <u>submit a change request</u>
              </a>
              instead.
            </p>
            <p></p>
            <p></p>
            <v-expansion-panels>
              <v-expansion-panel>
                <v-expansion-panel-title>
                  <h3>Learn more about program changes</h3>
                </v-expansion-panel-title>
                <v-expansion-panel-text>
                  Program changes are divided into two categories:
                  <br />
                  <br />
                  <ol class="ml-10">
                    <li>
                      Changes that do not require ECE Registry approval,
                      which include: 
                      <ul style="list-style-type: disc">
                        <li>
                          Renaming course codes and course names without changes
                          to content 
                        </li>
                        <li>
                          Reducing or increasing course hours
                          while remaining within the minimum hours for each area
                          of instruction and not altering the already approved
                          competencies or learning objectives for each area of
                          instruction  
                        </li>
                      </ul>
                    </li>
                    <br />
                    <li>
                      Changes that require ECE Registry approval,
                      which include: 
                      <ul style="list-style-type: disc">
                        <li>
                          Any changes that might alter the ECE Registry approved
                          program coursework that meets the minimum provincial
                          requirements for certification 
                        </li>
                        <li>
                          Updating the course description or
                          learning objectives that might directly impact the
                          student’s ability to demonstrate any of the required
                          occupational standards set out in the BC Child Care
                          Sector Occupational Competencies   
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
          </div>
        </Callout>
        <br />
        <br />
        <div>
          <v-btn
            rounded="lg"
            :loading="updateInProgress"
            :disabled="disableButton"
            color="primary"
            @click="initiateUpdate"
          >
            Continue to program profile
          </v-btn>
        </div>
        <div>
          <div v-if="updateInProgress" class="mt-8">
            <v-progress-circular
              class="mb-2"
              color="primary"
              indeterminate
            ></v-progress-circular>
            <h4>
              Your request has been initiated. Please wait a few minutes while
              we prepare it for review. When ready, it will appear here and will
              also be available in your dashboard.
            </h4>
          </div>
        </div>
      </div>
    </div>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Loading from "@/components/Loading.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import { useLoadingStore } from "@/store/loading";
import { useRouter } from "vue-router";
import type { Components } from "@/types/openapi";
import ECEHeader from "@/components/ECEHeader.vue";
import Callout from "@/components/common/Callout.vue";
import { getPrograms, initiateProgramChange } from "@/api/program";
import { IntervalTime } from "@/utils/constant";

export default defineComponent({
  name: "ProgramProfiles",
  components: {
    ECEHeader,
    PageContainer,
    Breadcrumb,
    Loading,
    Callout,
  },
  setup() {
    const loadingStore = useLoadingStore();
    const router = useRouter();
    return { loadingStore, router };
  },
  data() {
    return {
      program: null as Components.Schemas.Program | null,
      newProgram: null as Components.Schemas.Program | null,
      updateInProgress: false,
      pollInterval: null as ReturnType<typeof setInterval> | null,
    };
  },
  props: {
    programId: {
      type: String,
      required: true,
    },
  },
  computed: {
    isLoading(): boolean {
      return (
        this.loadingStore.isLoading("program_get") && !this.updateInProgress
      );
    },
    programTitle(): string {
      if (this.program?.name) {
        return this.program?.name + " - " + this.displayedTypes;
      } else {
        return "";
      }
    },
    displayedTypes(): string {
      let types = "";
      if (
        this.program?.programTypes != null &&
        this.program?.programTypes?.length > 0
      ) {
        const typeCount = this.program?.programTypes?.length;
        this.program?.programTypes?.forEach((type, index) => {
          types = types + type;
          if (index < typeCount - 1) {
            types = types + ", ";
          }
        });
      }
      return types;
    },
    disableButton(): boolean {
      return this.program?.status === "ChangeRequestInProgress";
    },
  },
  async mounted() {
    await this.loadProgram();

    /* Check if an update request currently exists and start polling if needed */
    await this.fetchNewProgram();

    /* If newProgram is already ready, navigate to it */
    if (this.newProgram?.readyForReview && this.newProgram?.id) {
      this.router.replace("/program/" + this.newProgram.id);
      return;
    }

    /* Start polling if there's an update in progress (either we have a newProgram not ready, 
       or the original program status indicates a change request is in progress) */
    const hasUnreadyNewProgram =
      this.newProgram && !this.newProgram.readyForReview;
    const changeInProgress = this.program?.status === "ChangeRequestInProgress";

    if (hasUnreadyNewProgram || changeInProgress) {
      this.startPolling();
    }
  },
  beforeUnmount() {
    this.stopPolling();
  },
  methods: {
    async loadProgram() {
      try {
        const { data: response } = await getPrograms(this.programId, [
          "Approved",
          "ChangeRequestInProgress",
        ]);
        this.program = response?.programs?.[0] ?? null;
      } catch (error) {
        console.error("Error loading program:", error);
        this.program = null;
      }
    },
    async fetchNewProgram() {
      try {
        const { data: response } = await getPrograms(undefined, undefined, {
          fromProgramId: this.programId,
        });
        this.newProgram = response?.programs?.[0] ?? null;
      } catch (error) {
        console.error("Error fetching new program:", error);
      }
    },
    async initiateUpdate() {
      if (!this.program) return;

      this.updateInProgress = true;
      const response = await initiateProgramChange(this.program);

      if (response.error) {
        console.error("Failed to initiate program change:", response.error);
        this.updateInProgress = false;
        return;
      }

      await this.fetchNewProgram();
      this.startPolling();
    },
    stopPolling() {
      if (this.pollInterval) {
        clearInterval(this.pollInterval);
        this.pollInterval = null;
      }
      this.updateInProgress = false;
    },
    startPolling() {
      this.updateInProgress = true;

      this.pollInterval = setInterval(async () => {
        await this.fetchNewProgram();
        if (this.newProgram?.readyForReview && this.newProgram?.id) {
          this.stopPolling();
          this.router.replace("/program/" + this.newProgram.id);
        }
      }, IntervalTime.INTERVAL_10_SECONDS);
    },
    submitChangeRequest() {
      this.router.push({
        name: "new-message",
        params: {
          initialCategory: "ProgramChangeRequest",
        },
      });
    },
  },
});
</script>
