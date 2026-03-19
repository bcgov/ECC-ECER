<template>
  <ProgramApplicationHeader
    :programApplicationName="programApplication.programApplicationName || ''"
  ></ProgramApplicationHeader>
  <v-container>
    <v-breadcrumbs class="pl-0" :items="breadcrumbItems" color="primary">
      <template #divider>/</template>
    </v-breadcrumbs>
    <v-row>
      <v-col cols="auto">
        <div v-if="$vuetify.display.smAndDown">
          <v-btn id="btnToggleMenu" icon @click="drawer = true">
            <v-icon>mdi-menu</v-icon>
          </v-btn>
        </div>
        <ComponentGroupNavigation
          v-else
          :programApplicationId="programApplicationId"
          :program-types="programApplication.programTypes"
          :application-status="programApplication.status"
          :application-type="programApplication.programApplicationType"
          :component-groups="componentGroups"
          :is-rfai="isRFAI"
        ></ComponentGroupNavigation>

        <v-navigation-drawer temporary v-model="drawer" width="350">
          <ComponentGroupNavigation
            :programApplicationId="programApplicationId"
            :program-types="programApplication.programTypes"
            :application-status="programApplication.status"
            :application-type="programApplication.programApplicationType"
            :component-groups="componentGroups"
            :is-rfai="isRFAI"
          ></ComponentGroupNavigation>
        </v-navigation-drawer>
      </v-col>
      <v-col>
        <router-view
          :application-type="programApplication.programApplicationType"
          @next="handleNext"
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";
import PageContainer from "@/components/PageContainer.vue";
import ComponentGroupNavigation from "@/components/common/ComponentGroupNavigation.vue";
import ProgramApplicationHeader from "./ProgramApplicationHeader.vue";
import type { Components } from "@/types/openapi";
import { getNavigationMetadata } from "@/api/program-application";

interface ApplicationStep {
  name: string;
  componentGroupId?: string;
  programType?: string;
}

export interface NextStepPayload {
  currentComponentGroupId?: string;
  programType?: string;
}

export default defineComponent({
  name: "ProgramApplication",
  components: {
    PageContainer,
    ComponentGroupNavigation,
    ProgramApplicationHeader,
  },
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
    programApplication: {
      type: Object as () => Components.Schemas.ProgramApplication,
      required: true,
    },
    componentGroupId: {
      type: String,
      required: false,
    },
    programType: {
      type: String as () => Components.Schemas.ProgramTypes,
      required: false,
    },
  },
  setup() {
    const { mobile } = useDisplay();

    return {
      mobile,
    };
  },
  data() {
    return {
      componentGroups: [] as Components.Schemas.NavigationMetadata[],
      drawer: false,
    };
  },
  async mounted() {
    await this.getComponentGroups();
    this.loadInitialStep();
  },
  beforeUnmount() {},
  computed: {
    orderedSteps(): ApplicationStep[] {
      const programTypeSteps =
        this.programApplication.programTypes?.map((programType) => ({
          name: "program-application-program-profile-area-of-instruction",
          programType: programType,
        })) ?? [];
      return [
        { name: "program-application-component-info" },
        { name: "program-application-institute-info" },
        ...this.componentGroups
          .filter((d) => d.navigationType === "Component")
          .map((g) => ({
            name: "program-application-component",
            componentGroupId: g.id ?? "",
          })),
        ...programTypeSteps,
        { name: "program-application-review-response" },
        {
          name: "program-application-program-profile-area-of-instruction-review",
        },
        {
          name: "submit-application",
        },
      ];
    },
    breadcrumbItems() {
      let programApplicationTypeDisplay = "";
      switch (this.programApplication.programApplicationType) {
        case "NewBasicECEPostBasicProgram":
          programApplicationTypeDisplay =
            "Application for a basic or post-basic program";
          break;
        case "NewCampusatRecognizedPrivateInstitution":
          programApplicationTypeDisplay = "Application for new campus";
          break;
        case "SatelliteProgram":
          programApplicationTypeDisplay = "Application for satellite program";
          break;
        case "AddOnlineorHybridDeliveryMethod":
          programApplicationTypeDisplay =
            "Application for adding an online or hybrid delivery method";
          break;
        case "CurriculumRevisionsatRecognizedInstitution":
        case "WorkIntegratedLearningProgram":
        default:
          programApplicationTypeDisplay =
            "unmapped application type " +
            this.programApplication.programApplicationType;
      }
      return [
        { title: "Home", disabled: false, href: "/" },
        {
          title: "All applications",
          disabled: false,
          href: "/program-applications",
        },
        { title: `${programApplicationTypeDisplay}`, disabled: true },
      ];
    },
    isRFAI() {
      return (
        this.programApplication?.status !== undefined &&
        (this.programApplication?.status === "InterimRecognition" ||
          this.programApplication?.status === "ReviewAnalysis") &&
        this.programApplication?.statusReasonDetail === "RFAIrequested"
      );
    },
  },
  methods: {
    async getComponentGroups() {
      const response = await getNavigationMetadata(this.programApplicationId);
      this.componentGroups = (response.data ?? []).sort(
        (a, b) => (a.displayOrder ?? 0) - (b.displayOrder ?? 0),
      );
    },
    async handleNext({
      currentComponentGroupId,
      programType,
    }: NextStepPayload) {
      await this.getComponentGroups();
      const currentIndex = this.orderedSteps.findIndex(
        (s) =>
          s.name === this.$route.name &&
          s.componentGroupId === currentComponentGroupId &&
          s.programType === programType,
      );
      const next = this.orderedSteps[currentIndex + 1];
      if (!next) {
        console.log(
          "An error occurred while trying to navigate. Next step not found.",
        );
        return;
      }
      this.$router.push({
        name: next.name,
        params: {
          programApplicationId: this.programApplicationId,
          ...(next.componentGroupId && {
            componentGroupId: next.componentGroupId,
          }),
          ...(next.programType && {
            programType: next.programType,
          }),
        },
      });
    },
    loadInitialStep() {
      if (this.isRFAI) {
        this.$router.push({
          name: "program-application-review-response",
          params: { programApplicationId: this.programApplication.id },
        });
      } else {
        this.$router.push({
          name: "program-application-component-info",
          params: { programApplicationId: this.programApplicationId },
        });
      }
    },
  },
});
</script>
