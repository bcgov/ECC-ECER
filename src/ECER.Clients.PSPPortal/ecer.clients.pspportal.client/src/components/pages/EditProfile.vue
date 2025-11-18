<template>
    <PageContainer>
        <v-row>
            <v-col cols="12">
                <Breadcrumb />
            </v-col>
        </v-row>
        <v-row>
            <v-col class="ml-1" cols="12">
                <h1>My contact details</h1>
            </v-col>
        </v-row>
        <v-row>
            <v-col class="mt-4" cols="12">
                <EceForm ref="editProfileformRef" :form="profileForm" :form-data="formStore.formData"
                    @updated-form-data="formStore.setFormData" />
                <v-row class="mt-10">
                    <v-col>
                        <div class="d-flex flex-row justify-start ga-2">
                            <v-btn rounded="lg" color="primary"
                                :loading="loadingStore.isLoading('psp_user_profile_put')"
                                @click="saveProfile">Save</v-btn>
                            <v-btn rounded="lg" color="primary" variant="outlined"
                                :loading="loadingStore.isLoading('psp_user_profile_put')"
                                @click="router.back()">Cancel</v-btn>
                        </div>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
    </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { getPspUserProfile, updatePspUserProfile } from "@/api/psp-rep";
import Breadcrumb from "@/components/Breadcrumb.vue";
import EceForm from "@/components/Form.vue";
import PageContainer from "@/components/PageContainer.vue";
import profileForm from "@/config/profile-form";
import { useAlertStore } from "@/store/alert";
import { useFormStore } from "@/store/form";
import { useLoadingStore } from "@/store/loading";
import { useUserStore } from "@/store/user";
import { useRouter } from "vue-router";

export default defineComponent({
    name: "EditProfile",
    components: { EceForm, Breadcrumb, PageContainer },
    setup: async () => {
        const formStore = useFormStore();
        const userStore = useUserStore();
        const alertStore = useAlertStore();
        const loadingStore = useLoadingStore();
        const router = useRouter();


        const userProfile = await getPspUserProfile();
        if (userProfile !== null) {
            formStore.initializeForm({
                [profileForm?.components?.firstName?.id || ""]: userProfile.firstName,
                [profileForm?.components?.lastName?.id || ""]: userProfile.lastName,
                [profileForm?.components?.email?.id || ""]: userProfile.email,
                [profileForm?.components?.phoneNumber?.id || ""]: userProfile.phone,
                [profileForm?.components?.phoneNumberExtension?.id || ""]: userProfile.phoneExtension,
                [profileForm?.components?.jobTitle?.id || ""]: userProfile.jobTitle,
                [profileForm?.components?.preferredFirstName?.id || ""]: userProfile.preferredName,
            });
        }

        return { profileForm, formStore, alertStore, userStore, loadingStore, router };
    },
    methods: {
        async saveProfile() {
            const { valid } = await (this.$refs.editProfileformRef as typeof EceForm).$refs[profileForm.id].validate();

            if (!valid) {
                this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
            } else {
                const userUpdated = await updatePspUserProfile({
                    email: this.formStore.formData[profileForm?.components?.email?.id || ""],
                    phone: this.formStore.formData[profileForm?.components?.phoneNumber?.id || ""],
                    phoneExtension: this.formStore.formData[profileForm?.components?.phoneNumberExtension?.id || ""],
                    jobTitle: this.formStore.formData[profileForm?.components?.jobTitle?.id || ""],
                    preferredName: this.formStore.formData[profileForm?.components?.preferredFirstName?.id || ""],
                });

                if (userUpdated) {
                    this.alertStore.setSuccessAlert("You have successfully edited your profile information.");
                    this.userStore.updatePspUserProfile({
                        email: this.formStore.formData[profileForm?.components?.email?.id || ""],
                        phone: this.formStore.formData[profileForm?.components?.phoneNumber?.id || ""],
                        phoneExtension: this.formStore.formData[profileForm?.components?.phoneNumberExtension?.id || ""],
                        jobTitle: this.formStore.formData[profileForm?.components?.jobTitle?.id || ""],
                        preferredName: this.formStore.formData[profileForm?.components?.preferredFirstName?.id || ""],
                    });
                } else {
                    this.alertStore.setFailureAlert("Profile save failed");
                }
            }
        },
    },
});
</script>
