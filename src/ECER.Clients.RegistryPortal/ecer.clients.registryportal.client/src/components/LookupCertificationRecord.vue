<template>
  <v-sheet class="bg-primary">
    <v-container><h1>Validate an ECE certificate</h1></v-container>
  </v-sheet>
  <v-container>
    <!-- prettier-ignore -->
    <a href="#" @click.prevent="router.push({ name: 'lookup-certification' })">
      <v-icon size='x-large'>mdi-chevron-left</v-icon>Back to search
    </a>
    <div>TODO ECER-3210</div>
    {{ lookupCertificationStore.certificationRecord }}
  </v-container>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import { useRouter } from "vue-router";

import { useLookupCertificationStore } from "@/store/lookupCertification";

export default defineComponent({
  name: "LookupCertification",

  props: {
    test: {
      // TODO create type for prop
      type: String as PropType<any>,
      required: true,
    },
  },
  setup() {
    const lookupCertificationStore = useLookupCertificationStore();
    const router = useRouter();
    return { lookupCertificationStore, router };
  },
  methods: {},
});
</script>
