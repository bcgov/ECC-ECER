<template>
  <v-container>
    <Breadcrumb />
    <h1>Employment experience references</h1>
    <br />
    <h2>Provide proof of 2 years of independent ECE practice.</h2>
    <br />
    <p>Your employment experience must:</p>
    <br />
    <ul class="ml-10">
      <li>Have been completed within the last 5 years</li>
      <li>Not overlap (any overlapping employment experience will only be counted once)</li>
      <li>Have been completed while holding valid ECE certification</li>
    </ul>
    <br />
    <p>If your employment experience was completed at multiple locations:</p>
    <br />
    <ul class="ml-10">
      <li>Provide a reference from each person</li>
      <li>You may enter up to 6 references</li>
    </ul>
  </v-container>
</template>
<script>
import { defineComponent } from "vue";
import { useRouter } from "vue-router";
import { useDisplay } from "vuetify";

import Breadcrumb from "@/components/Breadcrumb.vue";

export default defineComponent({
  name: "ManageWorkExperienceReferenceList",
  components: { Breadcrumb },
  props: {
    icraEligibilityId: {
      type: String,
      required: true,
    },
  },
  setup: async (props) => {
    const { smAndUp } = useDisplay();
    const router = useRouter();

    return {
      smAndUp,

      router,
    };
  },
  computed: {},
  methods: {},
});
</script>
