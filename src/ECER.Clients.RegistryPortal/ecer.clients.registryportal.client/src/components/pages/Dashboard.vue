<template>
  <v-app>
    <v-navigation-drawer permanent class="bg-white">
      <v-list>
        <v-list-item prepend-icon="mdi-account-edit " title="Sandra Adams" subtitle="sandra_a88@gmail.com">
          <template #subtitle>
            <v-list-item-title><p class="small">test@test.com</p></v-list-item-title>
            <v-list-item-title
              ><p class="small">{{ formatPhoneNumber("1231231234") }}</p></v-list-item-title
            >
          </template>
        </v-list-item>
      </v-list>

      <v-divider></v-divider>

      <v-list density="compact" base-color="black" color="links" nav>
        <v-list-item prepend-icon="mdi-folder" title="My Certifications" value="certifications"> </v-list-item>
        <v-list-item prepend-icon="mdi-bell" title="Messages" value="messages"></v-list-item>
        <v-list-item prepend-icon="mdi-account-edit" title="Profile" value="profile"></v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <v-container fluid>
        <v-row>
          <v-col sm="12" md="6">
            <v-card flat color="white">
              <h3>Welcome Jane Doe</h3>
              <p class="small">Complete and submit your application for certification in early childhood education.</p>
              <v-btn color="primary">Apply Now</v-btn>
            </v-card>
          </v-col>
          <v-col sm="12" md="6">
            <v-card flat color="white">
              <p class="small">Not sure which certificate to apply for? Fill out a quick self-assessment to see your certification options​​​​​​​.</p>
              <v-btn color="warning">Check Elegibility</v-btn>
            </v-card>
          </v-col>
          <v-col cols="12" class="order-first order-md-last order-lg-last order-xl-last">
            <v-card flat color="white"> <router-view></router-view> </v-card
          ></v-col>
        </v-row>
      </v-container>
    </v-main>
  </v-app>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { formatPhoneNumber } from "@/utils/format";

export default defineComponent({
  name: "Dashboard",

  methods: {
    formatPhoneNumber,
  },
});
</script>

<style lang="scss" scoped>
.v-list-item > div > i {
  opacity: 1 !important;
  color: blue !important;
}
</style>
