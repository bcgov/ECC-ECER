<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>
    <v-row>
      <v-col class="ml-1" cols="12">
        <h1>Edit Profile</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col class="mt-4" cols="12">
        <ProfileForm />
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import Breadcrumb from "@/components/Breadcrumb.vue";
import PageContainer from "@/components/PageContainer.vue";
import ProfileForm from "@/components/ProfileForm.vue";

export default {
  name: "EditProfile",
  components: { ProfileForm, Breadcrumb, PageContainer },
};
</script>
