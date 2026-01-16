<template>
  <PageContainer>
    <Loading />
  </PageContainer>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import { getPortalInvitation } from "@/api/portal-invitation";
import { useUserStore } from "@/store/user";
import { PortalInviteType } from "@/utils/constant";
import { useRoute, useRouter } from "vue-router";
import PageContainer from "@/components/PageContainer.vue";
import Loading from "@/components/Loading.vue";

const router = useRouter();
const route = useRoute();
const userStore = useUserStore();

onMounted(async () => {
  const { data, error } = await getPortalInvitation(route.params.token as string);

  if (error) {
      if (error.detail === "Portal Invitation Wrong Status"){
          router.replace("/invalid-invitation");
      } else {
          router.push("/access-denied");
      }
      return;
  }

  if (data?.portalInvitation?.inviteType === PortalInviteType.PSIProgramRepresentative) {
    const programRepresentativeId = data?.portalInvitation?.pspProgramRepresentativeId;
    if (programRepresentativeId) {
      userStore.setInvitedProgramRepresentativeId(programRepresentativeId);
      userStore.setInvitationToken(route.params.token as string);
      router.push("/login");
    } else {
      router.push("/access-denied");
    }
  }
});
</script>
