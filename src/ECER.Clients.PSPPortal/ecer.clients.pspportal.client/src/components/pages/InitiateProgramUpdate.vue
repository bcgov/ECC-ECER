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
      </v-col>
    </v-row>

    <Loading v-if="isLoading"></Loading>

    <div>
      <div>
        <v-row>
            <v-col>
                <h2>{{program?.programName}} - {{displayedTypes}}</h2>             
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="12">
              <p>
                Make updates to your program profile that do not affect program requirements or competencies (for example, start date, course name, etc.). 
                You can indicate the date on which your changes come into effect.
              </p>

                <p>Review your program profile by clicking the button below and update any of the following:</p>
                <ul class="ml-8">
                    <li>Program name</li>
                    <li>Program start date</li>
                    <li>Course code</li>
                    <li>Course name</li>
                    <li>Course hours (Note: Total hours and competency requirements for all areas of instruction must be met)</li>
                </ul>           
            </v-col>
        </v-row>

        <Callout class="mt-3" type="warning">
          <div class="d-flex flex-column ga-3">
              <p>Need to make a change to a program?</p>
              <p>If your update will affect program requirements or competencies (for example, adding or removing courses), please submit a change request instead.</p>
              <p></p>
              <p>Learn more about program changes</p>
          </div>
        </Callout>

        <div v-if="program?.status != 'ChangeRequestInProgress'">
          <v-btn  rounded="lg" 
                  color="primary" 
                  @click="initiateUpdate">Start program profile update</v-btn>
        </div>
        <div
          v-if="program?.status === 'ChangeRequestInProgress' && !isReadyForReview()"
          class="mt-8"
        >
          <v-progress-circular
            class="mb-2"
            color="primary"
            indeterminate
          ></v-progress-circular>
          <h4>
            Your request has been initiated. Please wait a few minutes while we prepare it for review. 
            When ready, it will appear here and will also be available in your dashboard.
          </h4>
        </div>        

      </div>
    </div>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Loading from "@/components/Loading.vue";
import { useLoadingStore } from "@/store/loading";
import { useRouter } from "vue-router";
import type { Components } from "@/types/openapi";
import ECEHeader from "@/components/ECEHeader.vue";
import Callout from "@/components/common/Callout.vue";
import { getPrograms, initiateProgramChange } from "@/api/program";

const PAGE_SIZE = 0;
const INTERVAL_TIME = 30000;

export default defineComponent({
  name: "ProgramProfiles",
  components: {
    ECEHeader,
    PageContainer,
    Loading,
    Callout
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
      updateInitiated: false,
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
    displayedTypes(): string {
      let types = "";
      if (this.program?.programTypes != null && this.program?.programTypes?.length > 0){
        const typeCount = this.program?.programTypes?.length;
        this.program?.programTypes?.forEach((type, index) => {
          types = types + type;
          if (index < typeCount-1){
            types = types + ", ";
          }
        });
      }
      return types;
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
          "Draft",
          "Denied",
          "Approved",
          "UnderReview",
          "ChangeRequestInProgress",
          "Inactive",
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
      if (this.newProgramId){
        const { data: response } = await getPrograms(this.newProgramId);
        if(response?.programs && response.programs[0]){
          this.newProgram = response.programs[0];
          if(this.newProgram.readyForReview){
            /* Ready for review flag has been set. Stop polling. */
            clearInterval(this.pollInterval)
            this.router.replace("/program/" + this.newProgramId);
          }
        }
      }
    },
    async initiateUpdate() {
      if (this.program != null){
        const response = await initiateProgramChange(this.program);
        if (response.error) {
          console.error("Failed to initiate program change:", response.error);
        }
        this.loadProgram();
        if(response.data != null){
          this.newProgramId = response.data;       
          this.fetchNewProgram();
          if (this.newProgram?.readyForReview){
            this.router.replace("/program/" + this.newProgramId);
          }else{
            /* Poll the backend until the ready for review flag is set */
            this.pollInterval = setInterval(() => {this.fetchNewProgram()}, INTERVAL_TIME);
            setTimeout(() => {clearInterval(this.pollInterval)}, INTERVAL_TIME * 10);          }
        }
      }
    },
    isReadyForReview(): boolean {
      if (this.newProgram == null || 
          !this.newProgram?.readyForReview)
      {        
        return false;
      }
      else
      {
        return true;
      }
    },
  },
});
</script>
