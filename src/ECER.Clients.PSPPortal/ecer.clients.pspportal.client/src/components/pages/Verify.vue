<template>
    <div></div>
</template>

<script lang="ts">
import { getPortalInvitation } from '@/api/portal-invitation';
import { useUserStore } from '@/store/user';
import { PortalInviteType } from '@/utils/constant';
import { useRoute, useRouter } from 'vue-router';


export default {
    async setup() {
        const router = useRouter();
        const route = useRoute();
        const userStore = useUserStore();
        const { data, error } = await getPortalInvitation(route.params.token as string);

        if (error) {
            console.log("error", error);
            router.push("/invalid-invitation");
        }

        if (data?.portalInvitation?.inviteType === PortalInviteType.PSIProgramRepresentative) {
            // store the program representative id in the store and let the user continue to authentication flow
            const programRepresentativeId = data?.portalInvitation?.pspProgramRepresentativeId;
            if (programRepresentativeId) {
                userStore.setInvitedProgramRepresentativeId(programRepresentativeId);
                router.push("/login");
            } else {
                router.push("/invalid-invitation");
            }
        }
    },
};
</script>
