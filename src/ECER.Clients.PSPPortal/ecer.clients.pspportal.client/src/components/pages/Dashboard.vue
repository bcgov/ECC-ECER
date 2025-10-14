<template>
  <PageContainer :margin-top="false">
    <h1>Hello World</h1>
    <p>Welcome to the dashboard!</p>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "Dashboard",
  components: {
    PageContainer,
  },
  setup() {
    const oidcStore = useOidcStore();
    const userStore = useUserStore();
    const router = useRouter();
    return { oidcStore, router, userStore };
  },
  async mounted() {
    let user;
    try {
      user = await this.oidcStore.getUser();
      if (!user) {
        user = await this.oidcStore.signinCallback();
        this.router.replace("/");
      }
    } catch (error) { }

    if (!user) {
      globalThis.location.href = "/login";
      return; //stops the rest of the component from loading. Prevents 401 calls for the methods below
    }
  },
});
</script>
