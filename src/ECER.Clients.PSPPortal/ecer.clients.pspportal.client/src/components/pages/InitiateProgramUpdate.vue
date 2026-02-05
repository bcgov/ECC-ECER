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

    <div>
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
                Make updates to your program profile that do not affect program requirements or competencies (for example, start date, course name, etc.). 
                You can indicate the date on which your changes come into effect.
              </p>
              <br />
              <p>Review your program profile by clicking the button below and update any of the following:</p>
              <br />
              <ul class="ml-8">
                  <li>Program name</li>
                  <li>Program start date</li>
                  <li>Course code</li>
                  <li>Course name</li>
                  <li>Course hours (Note: total hours and competency requirements for all areas of instruction must be met)</li>
              </ul>           
            </v-col>
        </v-row>

        <Callout class="mt-3" type="warning">
          <div class="d-flex flex-column ga-3">
              <h3>Need to make a change to a program?</h3>
              <p>If your update will affect program requirements or competencies (for example, adding or removing courses), please 
                <a @click="submitChangeRequest()">submit a change request</a> instead.</p>
              <p></p>
              <p>Learn more about program changes</p>
          </div>
        </Callout>
        <br /><br />
        <div v-if="program?.status != 'ChangeRequestInProgress'">
          <v-btn  rounded="lg" 
                  color="primary" 
                  @click="initiateUpdate">Continue to program profile</v-btn>
        </div>
        <div v-else>
          <div
            v-if="showProgressMeter"
            class="mt-8"
          >
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
      newProgramId: "" as string,
      loading: true,
      updateInProgress: false,
      pollInterval: 0 as any,
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
      return this.loadingStore.isLoading("program_get") || this.loading;
    },
    programTitle(): string {
      return this.program?.name + " - " + this.displayedTypes;
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
    showProgressMeter(): boolean {    
        return this.updateInProgress;
    },
  },
  async mounted() {
    await this.loadProgram();
    this.loading = false;
  },
  methods: {
    async loadProgram() {
      this.loading = true;
      try {
        const { data: response } = await getPrograms(this.programId, [
          "Approved",
          "ChangeRequestInProgress",
        ]);
        const program =
          response?.programs && response.programs.length > 0
            ? response.programs[0]
            : null;
        this.program = program || null;
      } catch (error) {
        console.error("Error loading program:", error);
        this.program = null;
      } finally {
        this.loading = false;
      }
    },
    async fetchNewProgram() {
      if (this.newProgramId) {
        const { data: response } = await getPrograms(this.newProgramId);
        if (response?.programs && response.programs[0]) {
          this.newProgram = response.programs[0];
        }
      }
    },
    async initiateUpdate() {
      if (this.program != null){
        this.updateInProgress = true;
        const response = await initiateProgramChange(this.program);
        if (response.error) {
          console.error("Failed to initiate program change:", response.error);
        }
        this.loadProgram();
        if (response.data != null) {
          this.newProgramId = response.data;
          this.fetchNewProgram();
          /* Poll the backend until the ready for review flag is set */
          this.pollInterval = setInterval(() => {
            this.fetchNewProgram();
            if (this.newProgram?.readyForReview) {
              /* Ready for review flag has been set. Stop polling. */
              this.updateInProgress = false;
              clearInterval(this.pollInterval);
              this.router.replace("/program/" + this.newProgramId);
            }
          }, IntervalTime.INTERVAL_10_SECONDS);
          setTimeout(() => {
            clearInterval(this.pollInterval);
          }, IntervalTime.INTERVAL_10_SECONDS * 10);
        }
      }
    },
    submitChangeRequest() {
      this.router.push({
        name: 'newMessage',
          params: {
            initialCategory: 'ProgramChangeRequest',
          },
      });
    }
  },
});
</script>
