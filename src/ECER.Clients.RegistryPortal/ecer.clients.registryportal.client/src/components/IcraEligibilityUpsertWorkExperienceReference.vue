<template>
  <Loading v-if="loadingStore.isLoading('icra_status_get')" />
  <v-container v-else>
    <Breadcrumb />
    <h1>Adding or replacing</h1>
    {{ "icra eligibility id " + icraEligibilityId }}
    {{ "reference id " + referenceId }}
  </v-container>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";
import { useDisplay } from "vuetify";
import type { Components } from "@/types/openapi";
import { cleanPreferredName } from "@/utils/functions";

import { getIcraEligibilityStatus } from "@/api/icra";

import Breadcrumb from "@/components/Breadcrumb.vue";
import Loading from "./Loading.vue";

import { useLoadingStore } from "@/store/loading";

export default defineComponent({
  name: "IcraEligibilityUpsertWorkExperienceReference",
  components: { Breadcrumb, Loading },
  props: {
    icraEligibilityId: {
      type: String,
      required: true,
    },
    referenceId: {
      type: String,
      required: true,
    },
  },
  setup: async (props) => {
    const { smAndUp } = useDisplay();
    const loadingStore = useLoadingStore();
    const router = useRouter();

    return {
      smAndUp,
      router,
      loadingStore,
    };
  },
  data() {
    return {
      icraEligibilityStatus: {} as Components.Schemas.ICRAEligibilityStatus,
    };
  },
  async mounted() {
    this.icraEligibilityStatus = (await getIcraEligibilityStatus(this.icraEligibilityId))?.data || {};
  },
  computed: {},
  methods: {
    cleanPreferredName,
  },
});
</script>
