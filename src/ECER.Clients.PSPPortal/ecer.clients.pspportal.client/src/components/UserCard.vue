<template>
  <Card
    :color="user.accessToPortal === 'Disabled' || user.accessToPortal == null ? 'surface-border-default' : 'surface'"
    min-height="225px"
    class="d-flex flex-column justify-space-between"
  >
    <div class="flex-grow-1">
      <v-row>
        <v-col cols="12">
          <h2>{{ displayName }}</h2>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12">
          <p class="font-weight-bold">{{ jobTitle }}</p>
          <p>{{ email }}</p>
        </v-col>
      </v-row>
    </div>

    <!-- Active users: Primary contact badge or action buttons -->
    <v-row v-if="user.accessToPortal === 'Active'">
      <v-col cols="12" class="align-self-end">
        <span v-if="isPrimary" class="d-flex align-center ga-2">
          <v-icon color="success" icon="mdi-check-decagram" size="small"></v-icon>
          <p class="text-success small">Primary contact</p>
        </span>
        <div v-else class="d-flex ga-2">
          <v-btn color="primary" size="small" @click="$emit('set-primary', user.id)" :loading="isLoading">
            <v-icon class="mr-2">mdi-check-decagram</v-icon>
            Set user as primary
          </v-btn>
          <v-btn
            variant="outlined"
            color="primary"
            size="small"
            @click="$emit('remove-access', user.id)"
            :disabled="user.id === currentUserId"
            :loading="isLoading"
          >
            <v-icon class="mr-2">mdi-minus-circle-outline</v-icon>
            Remove access
          </v-btn>
        </div>
      </v-col>
    </v-row>

    <!-- Pending invitations: Resend invitation button -->
    <v-row v-if="user.accessToPortal === 'Invited'">
      <v-col cols="12" class="align-self-end">
        <v-btn color="primary" size="small" variant="outlined" @click="$emit('resend-invitation', user.id)" :loading="isLoading">
          <v-icon class="mr-2">mdi-email-arrow-right-outline</v-icon>
          Resend invitation
        </v-btn>
      </v-col>
    </v-row>

    <!-- Inactive users: Reactivate button -->
    <v-row v-if="user.accessToPortal === 'Disabled' || user.accessToPortal == null">
      <v-col cols="12" class="align-self-end">
        <v-btn color="primary" size="small" @click="$emit('reactivate', user.id)" :loading="isLoading">
          <v-icon class="mr-2">mdi-account-plus-outline</v-icon>
          Reactivate user
        </v-btn>
      </v-col>
    </v-row>
  </Card>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import Card from "@/components/Card.vue";
import { useLoadingStore } from "@/store/loading";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "UserCard",
  components: { Card },
  props: {
    user: {
      type: Object as PropType<Components.Schemas.PspUserListItem>,
      required: true,
    },
    currentUserId: {
      type: String,
      required: true,
    },
  },
  setup() {
    const loadingStore = useLoadingStore();
    return {
      loadingStore,
    };
  },
  emits: {
    "set-primary": (userId: string | null | undefined) => true,
    "remove-access": (userId: string | null | undefined) => true,
    "resend-invitation": (userId: string | null | undefined) => true,
    reactivate: (userId: string | null | undefined) => true,
  },
  computed: {
    displayName(): string {
      const profile = this.user.profile;
      if (!profile) return "Unknown User";

      const firstName = profile.firstName || "";
      const lastName = profile.lastName || "";
      return `${firstName} ${lastName}`.trim() || "Unknown User";
    },
    jobTitle(): string {
      return this.user.profile?.jobTitle || "No job title set";
    },
    email(): string {
      return this.user.profile?.email || "No email address set";
    },
    isPrimary(): boolean {
      return this.user.profile?.role === "Primary";
    },
    isLoading(): boolean {
      return (
        this.loadingStore.isLoading("psp_user_manage_get") ||
        this.loadingStore.isLoading("psp_user_manage_deactivate_post") ||
        this.loadingStore.isLoading("psp_user_manage_reactivate_post") ||
        this.loadingStore.isLoading("psp_user_manage_set_primary_post")
      );
    },
  },
});
</script>
