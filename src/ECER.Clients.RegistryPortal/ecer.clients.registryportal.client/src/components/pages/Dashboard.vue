<template>
  <v-app>
    <v-navigation-drawer v-model="drawer" floating class="bg-white">
      <v-list>
        <v-list-item prepend-icon="mdi-account-edit " :title="userStore.fullName" subtitle="sandra_a88@gmail.com">
          <template #subtitle>
            <v-list-item-title>
              <p class="small">{{ userStore.email }}</p>
            </v-list-item-title>
            <v-list-item-title>
              <p class="small">{{ formatPhoneNumber(userStore.phoneNumber) }}</p>
            </v-list-item-title>
          </template>
        </v-list-item>
      </v-list>

      <v-divider></v-divider>

      <v-list density="compact" base-color="black" color="links" nav>
        <v-list-item
          v-for="item in navigationOptions"
          :key="item.path"
          :active="$router.currentRoute.value.path.includes(item.path)"
          :title="item.name"
          @click="$router.push(item.path)"
        >
          <template #prepend>
            <v-badge v-if="item.badge" color="error" :content="item.badge">
              <v-icon>{{ item.icon }}</v-icon>
            </v-badge>
            <v-icon v-else>{{ item.icon }}</v-icon>
          </template>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-app-bar v-if="$vuetify.display.mobile" height="40" elevation="0" color="warning">
      <v-icon class="ms-6" size="large" color="grey-dark" icon="mdi-menu" @click="drawer = !drawer"></v-icon>
    </v-app-bar>

    <v-main>
      <v-container fluid>
        <v-row>
          <v-col cols="12" md="8" lg="8" xl="8">
            <v-card class="rounded-lg fill-height" flat color="white">
              <v-card-item class="ma-4">
                <h3>Welcome {{ userStore.fullName }}</h3>
                <p class="small">Complete and submit your application for certification in early childhood education.</p>
              </v-card-item>
              <v-card-actions class="ma-4">
                <v-btn variant="flat" rounded="lg" color="primary" @click="$router.push('/application')">Start New Application</v-btn>
              </v-card-actions>
            </v-card>
          </v-col>
          <v-col cols="12" md="4" lg="4" xl="4">
            <v-card class="rounded-lg fill-height" flat color="white">
              <v-card-item class="ma-4">
                <p class="small">Not sure which certificate to apply for? Fill out a quick self-assessment to see your certification options.</p>
              </v-card-item>
              <v-card-actions class="ma-4">
                <v-btn variant="flat" rounded="lg" color="warning">Check Eligibility</v-btn>
              </v-card-actions>
            </v-card>
          </v-col>
          <v-col cols="12" class="order-first order-lg-last order-xl-last">
            <v-card class="rounded-lg" flat color="white">
              <v-card-item>
                <router-view></router-view>
              </v-card-item>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-main>
  </v-app>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useMessageStore } from "@/store/message";
import { useUserStore } from "@/store/user";
import { formatPhoneNumber } from "@/utils/format";

export default defineComponent({
  name: "Dashboard",
  setup() {
    const userStore = useUserStore();
    const messageStore = useMessageStore();

    const navigationOptions = [
      { name: "My Certifications", path: "/my-certifications", icon: "mdi-folder" },
      {
        name: `Messages${messageStore.messageCount > 0 ? ` (${messageStore.messageCount})` : ""}`,
        path: "/messages",
        icon: "mdi-bell",
        badge: messageStore.messageCount,
      },
      { name: "Profile", path: "/profile", icon: "mdi-account-edit" },
    ];

    return { userStore, navigationOptions };
  },
  data: () => ({
    drawer: null as boolean | null | undefined,
  }),
  methods: {
    formatPhoneNumber,
  },
});
</script>
