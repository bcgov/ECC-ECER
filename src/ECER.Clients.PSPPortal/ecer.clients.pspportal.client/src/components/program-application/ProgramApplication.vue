<template>
  <ProgramApplicationHeader
    :programApplicationName="programApplication.programApplicationName || ''"
  ></ProgramApplicationHeader>
  <v-container>
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
        ></ComponentGroupNavigation>

        <v-navigation-drawer temporary v-model="drawer" width="350">
          <ComponentGroupNavigation
            :programApplicationId="programApplicationId"
            :program-types="programApplication.programTypes"
            :application-status="programApplication.status"
            :application-type="programApplication.programApplicationType"
            :component-groups="componentGroups"
          ></ComponentGroupNavigation>
        </v-navigation-drawer>
      </v-col>
      <v-col lg="8" md="6" sm="auto">
        <router-view @next="handleNext" />
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
import { getComponentGroupMetadata } from "@/api/program-application";

interface ApplicationStep {
  name: string;
  componentGroupId?: string;
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
      componentGroups: [] as Components.Schemas.ComponentGroupMetadata[],
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
      return [
        { name: "program-application-component-info" },
        { name: "program-application-institute-info" },
        ...this.componentGroups.map((g) => ({
          name: "program-application-component",
          componentGroupId: g.id ?? "",
        })),
        //TODO { name: "some-future-route" }
      ];
    },
  },
  methods: {
    async getComponentGroups() {
      const response = await getComponentGroupMetadata(
        this.programApplicationId,
      );
      this.componentGroups = (response.data ?? []).sort(
        (a, b) => (a.displayOrder ?? 0) - (b.displayOrder ?? 0),
      );
    },
    async handleNext(currentComponentGroupId?: string) {
      await this.getComponentGroups();
      const currentIndex = this.orderedSteps.findIndex(
        (s) =>
          s.name === this.$route.name &&
          s.componentGroupId === currentComponentGroupId,
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
        },
      });
    },
    loadInitialStep() {
      this.$router.push({
        name: "program-application-component-info",
        params: { programApplicationId: this.programApplicationId },
      });
    },
  },
});
</script>
