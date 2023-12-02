<template>
  <main>
    <p>User: {{ userStore.userInfo }}</p>

    <Applications />
  </main>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import Applications from "@/components/Applications.vue";
import { useUserStore } from "@/store/user";

export default defineComponent({
  name: "Home",
  components: {
    Applications,
  },
  setup() {
    const userStore = useUserStore();

    return { userStore };
  },
});
</script>
