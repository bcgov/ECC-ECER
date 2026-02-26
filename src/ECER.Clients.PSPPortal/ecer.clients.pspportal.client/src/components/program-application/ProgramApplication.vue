<template>
  <ProgramApplicationHeader
    :programApplicationName="programApplication.programApplicationName"
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
        ></ComponentGroupNavigation>

        <v-navigation-drawer temporary v-model="drawer" width="350">
          <ComponentGroupNavigation
            :programApplicationId="programApplicationId"
            :program-types="programApplication.programTypes"
            :application-status="programApplication.status"
            :application-type="programApplication.programApplicationType"
          ></ComponentGroupNavigation>
        </v-navigation-drawer>
      </v-col>
      <v-col>
        <router-view />
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
      type: Object,
      required: true,
    },
    componentGroupId: {
      type: String,
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
      drawer: false,
    };
  },
  mounted() {},
  beforeUnmount() {},
  methods: {},
});
</script>
