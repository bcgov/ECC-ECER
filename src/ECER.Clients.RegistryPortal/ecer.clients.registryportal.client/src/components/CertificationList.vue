<template>
  <v-row>
    <v-col cols="12" v-for="(cert, index) in certifications" :key="index">
      <CertificationCard :certification="cert" :has-application="applicationStore.hasApplication" />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { Components } from "@/types/openapi";
import CertificationCard from "@/components/CertificationCard.vue";
import { useApplicationStore } from "@/store/application";

export default defineComponent({
  name: "CertificationList",
  components: {
    CertificationCard,
  },
  props: {
    certifications: {
      type: Array as PropType<Components.Schemas.Certification[]>,
      required: true,
    },
  },
  setup() {
    const applicationStore = useApplicationStore();
    return { applicationStore };
  },
});
</script>
