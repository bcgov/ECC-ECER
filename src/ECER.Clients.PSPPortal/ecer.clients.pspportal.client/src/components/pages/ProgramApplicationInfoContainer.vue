<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>
    <ProgramApplicationInfo />
    <v-row>
      <v-col>
        <v-btn
          rounded="lg"
          color="primary"
          @click="router.push('/program-application-begin')"
        >
          Continue
        </v-btn>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Callout from "@/components/common/Callout.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import { useRouter } from "vue-router";
import ProgramApplicationInfo from "@/components/common/ProgramApplicationInfo.vue";

export default defineComponent({
  name: "ProgramApplicationInfoContainer",
  components: {
    ProgramApplicationInfo,
    Breadcrumb,
    ECEHeader,
    Callout,
    PageContainer,
  },
  setup() {
    const router = useRouter();

    return {
      router,
    };
  },
});
</script>
