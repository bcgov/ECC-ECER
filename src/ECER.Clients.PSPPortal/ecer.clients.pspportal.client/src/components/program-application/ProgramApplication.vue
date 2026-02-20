<template>
  <ProgramApplicationHeader
    :programApplicationName="programApplicationName"
  ></ProgramApplicationHeader>
  <v-row>
    <v-col cols="3">
      <ComponentGroupNavigation
        :programApplicationId="programApplicationId"
      ></ComponentGroupNavigation>
    </v-col>
    <v-col>
      <router-view />
    </v-col>
  </v-row>
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
    programApplicationName: {
      type: String,
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
    return {};
  },
  mounted() {},
  beforeUnmount() {},
  methods: {},
});
</script>
<style scoped></style>
