<template>
  <v-row class="ma-4">
    <v-col cols="12">
      <h1>Profile</h1>
      <p>Edit your profile information, we use this information for your application process.</p>
    </v-col>
    <ProfileForm />
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ProfileForm from "@/components/ProfileForm.vue";

export default defineComponent({
  name: "Profile",
  components: { ProfileForm },
});
</script>
