<template>
  <PageContainer>
    <Loading v-if="isLoading" />
    <template v-else>
      <v-row class="justify-space-between mb-4">
        <v-col cols="auto">
          <h1>Review responses</h1>
        </v-col>
        <v-col cols="auto">
          <v-btn rounded="lg" variant="text" @click="printPage()">
            <v-icon
              color="secondary"
              icon="mdi-printer-outline"
              class="mr-2"
            ></v-icon>
            <a class="small">Print Preview</a>
          </v-btn>
        </v-col>
      </v-row>

      <v-row v-if="isRFAI" class="mb-4">
        <v-col cols="12">
          <Callout title="Additional information requested" type="warning">
            <p>
              Refer to your messages for details on resolving this request. You
              may edit the questions in your application requiring additional
              information.
            </p>
          </Callout>
        </v-col>
      </v-row>

      <v-row v-if="isRFAI" class="mb-4">
        <v-col class="d-flex" cols="12">
          <p class="align-self-center mr-4"><strong>SHOW:</strong></p>
          <v-btn-toggle
            v-model="filter"
            color="primary"
            mandatory
            @update:model-value="setView()"
          >
            <v-btn value="rfai">
              Responses requiring additional information
            </v-btn>
            <v-btn value="all">All Responses</v-btn>
          </v-btn-toggle>
        </v-col>
      </v-row>

      <v-row v-if="isRFAI && filter === 'rfai'" class="mb-4">
        <v-col class="d-flex" cols="12">
          <p>
            Show only responses where the registry is requestimg more
            information.
          </p>
        </v-col>
      </v-row>

      <v-card v-if="isRFAI" class="mb-4 pb-2" variant="outlined" rounded="lg">
        <v-card-title>
          <div class="d-flex align-center">
            <div>
              <h2 class="text-wrap">Application submitted</h2>
            </div>
          </div>
        </v-card-title>
        <v-card-text class="text-grey-dark">
          <v-row class="mb-n5">
            <v-col cols="4">
              <p class="small">Submitted by</p>
            </v-col>
            <v-col>
              <p class="small font-weight-bold">
                {{ programApplicationObject?.declarantName }}
              </p>
            </v-col>
          </v-row>

          <v-row class="mb-n5">
            <v-col cols="4">
              <p class="small">Submission date</p>
            </v-col>
            <v-col>
              <p class="small font-weight-bold">
                {{ programApplicationObject?.declarationDate }}
              </p>
            </v-col>
          </v-row>

          <v-row class="mb-n5">
            <v-col cols="4">
              <p class="small">Application status</p>
            </v-col>
            <v-col>
              <p class="small font-weight-bold">
                {{ statusText }}
              </p>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <v-card v-if="!isRFAI" class="mb-4 pb-2" variant="outlined" rounded="lg">
        <v-card-title>
          <div class="d-flex justify-space-between align-center">
            <div>
              <h2 class="text-wrap">Program overview</h2>
            </div>
            <div>
              <v-tooltip
                v-if="programApplicationObject?.status === 'Draft'"
                location="top"
              >
                <template #activator="{ props }">
                  <v-btn
                    icon="mdi-pencil"
                    v-bind="props"
                    :color="'primary'"
                    variant="plain"
                    :to="{
                      name: 'program-application-institute-info',
                      params: { programApplicationId: programApplicationId },
                    }"
                  />
                </template>
                <span>Edit institution and program information</span>
              </v-tooltip>
            </div>
          </div>
        </v-card-title>
        <!-- CONTENT -->
        <v-card-text class="text-grey-dark">
          <PostBasicProgramOverview
            :programApplicationObject="programApplicationObject"
            :contactPerson="contactPerson"
            v-if="
              programApplicationObject?.programApplicationType ===
              programApplicationType.NewBasicECEPostBasicProgram
            "
          />
          <NewCampusProgramOverview
            :programApplicationObject="programApplicationObject"
            :contactPerson="contactPerson"
            v-if="
              programApplicationObject?.programApplicationType ===
              programApplicationType.NewCampusatRecognizedPrivateInstitution
            "
          />
        </v-card-text>
      </v-card>

      <v-card
        v-for="[name, metaData] in componentAnswer"
        :key="name"
        class="mb-4 pb-2"
        variant="outlined"
        rounded="lg"
      >
        <v-card-title>
          <div class="d-flex justify-space-between align-center">
            <div>
              <h2 class="text-wrap">{{ name }}</h2>
            </div>
            <div v-if="allowEdit(metaData.components)">
              <v-tooltip
                v-if="programApplicationObject?.status === 'Draft'"
                location="top"
              >
                <template #activator="{ props }">
                  <v-btn
                    icon="mdi-pencil"
                    v-bind="props"
                    :color="'primary'"
                    variant="plain"
                    :to="
                      '/program-application/' +
                      programApplicationId +
                      '/component/' +
                      metaData.componentGroupId
                    "
                  />
                </template>
                <span>Edit {{ name }}</span>
              </v-tooltip>
            </div>
          </div>
        </v-card-title>
        <v-card-text
          v-for="component in metaData.components"
          class="text-grey-dark"
        >
          <v-row class="mb-n5">
            <v-col cols="4">
              <p class="small">{{ component.name }}</p>
            </v-col>
            <v-col>
              <v-chip
                v-if="isRFAI && component.rfaiRequired"
                color="warning"
                variant="flat"
                size="small"
              >
                <div>Additional information requested</div>
              </v-chip>
              <p class="small font-weight-bold">
                {{ component.answer }}
              </p>
            </v-col>
          </v-row>
        </v-card-text>
        <v-card-text class="text-grey-dark">
          <v-row class="mb-n5">
            <v-col cols="4">
              <p class="small">
                <v-icon>mdi-attachment</v-icon>
                Attached files
              </p>
            </v-col>
            <v-col>
              <p class="small font-weight-bold">
                {{ mapFileNames(metaData) }}
              </p>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <v-row class="d-print-none">
        <v-col>
          <v-btn rounded="lg" color="primary" @click="$emit('next', {})">
            Continue
          </v-btn>
        </v-col>
      </v-row>
    </template>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Loading from "@/components/Loading.vue";
import type {
  Components,
  PspUserListItem,
  ComponentGroupWithComponents,
} from "@/types/openapi";
import type { NextStepPayload } from "@/components/program-application/ProgramApplication.vue";
import {
  mapProgramType,
  getProgramApplicationById,
  getComponentGroupComponents,
  mapEnrollmentOptions,
  mapAdmissionOptions,
  mapDeliveryMethods,
  mapProgramStatus,
} from "@/api/program-application";
import { useUserStore } from "@/store/user";
import { getUsers } from "@/api/manage-users";
import Callout from "@/components/common/Callout.vue";
import PostBasicProgramOverview from "@/components/common/PostBasicProgramOverview.vue";
import NewCampusProgramOverview from "@/components/common/NewCampusProgramOverview.vue";
import { ProgramApplicationType } from "@/utils/constant";

interface ComponentGroupMetaData {
  componentGroupId?: string | null;
  components?: Components.Schemas.ProgramApplicationComponent[] | null;
}

export default defineComponent({
  name: "ProgramApplicationReviewResponses",
  components: {
    PageContainer,
    Loading,
    Callout,
    PostBasicProgramOverview,
    NewCampusProgramOverview,
  },
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
  },
  setup() {
    const userStore = useUserStore();
    return {
      userStore,
    };
  },
  emits: { next: (_payload: NextStepPayload) => true },
  computed: {
    programApplicationType() {
      return ProgramApplicationType;
    },
    statusText(): string {
      return mapProgramStatus(this.programApplicationObject?.status);
    },
    programType(): string {
      const types = this.programApplicationObject?.programTypes;
      if (!types?.length) return "—";
      return types.map(mapProgramType).join(", ");
    },
    mapEnrollmentOptions() {
      const types = this.programApplicationObject?.enrollmentOptions;
      if (!types?.length) return "—";
      return types.map(mapEnrollmentOptions).join(", ");
    },
    mapAdmissionOptions() {
      const types = this.programApplicationObject?.admissionOptions;
      if (!types?.length) return "—";
      return types.map(mapAdmissionOptions).join(", ");
    },
    mapDeliveryMethods() {
      const types = this.programApplicationObject?.deliveryMethod;
      if (!types?.length) return "—";
      return types.map(mapDeliveryMethods).join(", ");
    },
    campus(): string {
      if (this.programApplicationObject?.programCampuses) {
        let programCampusIds =
          this.programApplicationObject?.programCampuses.map((c) => {
            if (c.campusId !== null && c.campusId !== undefined) {
              return c.campusId;
            }
          });
        let campusObj = this.userStore.educationInstitution?.campuses?.filter(
          (c) => {
            if (c.id !== null && c.id !== undefined) {
              programCampusIds.includes(c.id);
            }
          },
        );
        if (!campusObj?.length) return "-";
        return campusObj.map((c) => c.generatedName).join(", ");
      }
      return "-";
    },
    isRFAI(): boolean {
      return (
        this.programApplicationObject?.status !== undefined &&
        this.activeStatus.includes(this.programApplicationObject?.status) &&
        (this.programApplicationObject?.status === "InterimRecognition" ||
          this.programApplicationObject?.status === "ReviewAnalysis") &&
        this.programApplicationObject?.statusReasonDetail === "RFAIrequested"
      );
    },
  },
  data() {
    return {
      programApplicationObject:
        null as Components.Schemas.ProgramApplication | null,
      contactPerson: "",
      componentGroup: [] as ComponentGroupWithComponents[] | null | undefined,
      componentAnswer: {} as Map<string, ComponentGroupMetaData> | null,
      isLoading: true,
      activeStatus: [
        "Approved",
        "Draft",
        "InterimRecognition",
        "OnGoingRecognition",
        "ReviewAnalysis",
        "Submitted",
      ] as Components.Schemas.ApplicationStatus[],
      filter: "rfai",
    };
  },
  async mounted() {
    this.isLoading = true;
    await this.fetchApplication();
    await this.loadComponents();
    this.isLoading = false;
  },
  methods: {
    printPage() {
      globalThis.print();
    },
    async fetchApplication() {
      const result = await getProgramApplicationById(this.programApplicationId);
      let users = (await getUsers()) ?? [];
      if (result.error || result.data == null) {
        console.error("Failed to retrieve program application:", result.error);
      } else {
        this.programApplicationObject = result.data;
        const contactPerson = users.find(
          (user: PspUserListItem) =>
            user.id === this.programApplicationObject?.programRepresentativeId,
        );
        this.contactPerson = contactPerson
          ? `${contactPerson.profile?.firstName} ${contactPerson.profile?.lastName}`.trim()
          : "-";
      }
    },
    showDeliverySection(): boolean {
      return this.programApplicationObject?.deliveryType !== "Inperson";
    },
    async loadComponents() {
      this.componentGroup = null;
      this.componentAnswer = null;
      const result = await getComponentGroupComponents(
        this.programApplicationId,
        undefined,
      );
      if (result.error) return;
      this.componentGroup = result.data as
        | ComponentGroupWithComponents[]
        | null
        | undefined;
      this.setView();
    },
    setView() {
      if (this.isRFAI) {
        if (this.filter === "rfai") {
          this.filterByRFAI();
        } else {
          this.allResponses();
        }
      } else {
        this.allResponses();
      }
    },
    mapFileNames(metaData: ComponentGroupMetaData) {
      return metaData.components?.flatMap((f) => f.files).join(", ");
    },
    filterByRFAI() {
      const map = new Map();

      this.componentGroup?.forEach((c) => {
        const groupName = c.name;
        const rfaiComponents = c.components?.filter((c) => c.rfaiRequired);
        if (rfaiComponents && rfaiComponents.length > 0) {
          if (map.has(groupName)) {
            map.get(groupName).components.push(rfaiComponents);
          } else {
            map.set(groupName, {
              componentGroupId: c.id,
              components: rfaiComponents,
            });
          }
        }
      });
      this.componentAnswer = map;
    },
    allResponses() {
      const map = new Map();

      this.componentGroup?.forEach((c) => {
        const groupName = c.name;
        if (map.has(groupName)) {
          map.get(groupName).components.push(c.components);
        } else {
          map.set(groupName, {
            componentGroupId: c.id,
            components: c.components,
          });
        }
      });
      this.componentAnswer = map;
    },
    allowEdit(
      components:
        | Components.Schemas.ProgramApplicationComponent[]
        | null
        | undefined,
    ) {
      const hasRfaiComponents = components?.filter((c) => c.rfaiRequired);
      return hasRfaiComponents && hasRfaiComponents.length > 0
        ? hasRfaiComponents && hasRfaiComponents.length > 0
        : !this.isRFAI;
    },
  },
});
</script>
