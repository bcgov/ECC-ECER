<template>
  <PageContainer>
    <v-row class="ma-4">
      <v-col cols="12">
        <h1>Profile</h1>
        <p>Edit your profile information, we use this information for your application process.</p>
      </v-col>
      <ProfileForm />
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PageContainer from "@/components/PageContainer.vue";
import ProfileForm from "@/components/ProfileForm.vue";

export default defineComponent({
  name: "Profile",
  components: { ProfileForm, PageContainer },
});
</script>
