<template>
  <v-sheet :rounded="0" flat color="primary" width="100%">
    <PageContainer :margin-top="false" class="py-13">
      <h1 class="text-white">Register page WIP</h1>
    </PageContainer>
  </v-sheet>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute } from "vue-router";

import PageContainer from "@/components/PageContainer.vue";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useDisplay } from "vuetify";

export default defineComponent({
  name: "Register",
  components: { PageContainer },
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const route = useRoute();
    const { smAndDown } = useDisplay();

    return { userStore, oidcStore, route, smAndDown };
  },
  methods: {},
});
</script>
