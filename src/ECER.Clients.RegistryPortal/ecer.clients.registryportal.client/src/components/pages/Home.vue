<template>
  <main>
    <button type="button" @click="handleTokenRefresh">
      Refresh token with BCeID
    </button>
    <p>User: {{ profile?.display_name }}</p>

    <Applications />
  </main>
</template>

<script lang="ts">
import { mapState } from "pinia";
import { defineComponent } from "vue";

import Applications from "@/components/Applications.vue";
import { useUserStore } from "@/store/user";

export default defineComponent({
  name: "Home",
  setup() {
    const userStore = useUserStore();
    return { userStore };
  },
  components: {
    Applications,
  },
  computed: {
    ...mapState(useUserStore, ["profile"]),
  },
  methods: {
    handleTokenRefresh: async function () {
      const user = await this.userStore.signinSilent();
      if (user?.profile) {
        this.userStore.setProfile(user?.profile);
      }
    },
  },
});
</script>
