<template>
    <div></div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { getPortalInvitation } from '@/api/portal-invitation';
import { useUserStore } from '@/store/user';
import { PortalInviteType } from '@/utils/constant';
import { useRoute, useRouter } from 'vue-router';

const router = useRouter();
const route = useRoute();
const userStore = useUserStore();

onMounted(async () => {
    const { data, error } = await getPortalInvitation(route.params.token as string);

    if (error) {
        console.log("error", error);
        router.push("/invalid-invitation");
        return;
    }

    if (data?.portalInvitation?.inviteType === PortalInviteType.PSIProgramRepresentative) {
        const programRepresentativeId = data?.portalInvitation?.pspProgramRepresentativeId;
        console.log("programRepresentativeId", programRepresentativeId);
        if (programRepresentativeId) {
            userStore.setInvitedProgramRepresentativeId(programRepresentativeId);
            router.push("/login");
        } else {
            router.push("/invalid-invitation");
        }
    }
});
</script>
