<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <h1>Edit campus</h1>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <p>
          You can edit details for this campus to update the current location
          and contact information.
        </p>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <!-- prettier-ignore -->
        <Alert type="warning" icon="mdi-alert-circle-outline" :closable="false">
          <p>
            <b>
              Do not edit this campus to represent a different campus or
              location. This can cause application and reporting errors.
            </b>
          </p>
          <br />
          To add a new campus or satellite location, go to
          <router-link
            :to="{
              name: 'education-institution',
              params: { institutionId },
            }"
          >
            View locations
          </router-link>.
        </Alert>
      </v-col>
    </v-row>

    <Loading v-if="isLoading" />

    <template v-else>
      <CampusForm
        ref="campusFormRef"
        :psp-users="pspUsers"
        :institution-name="institutionName"
        :is-private="isPrivate"
        :initial-data="campus ?? undefined"
      />

      <v-row class="mt-8">
        <v-col cols="auto">
          <div class="d-flex flex-row ga-2">
            <v-btn
              rounded="lg"
              color="primary"
              :loading="isSaving"
              @click="save"
            >
              Save
            </v-btn>
            <v-btn
              rounded="lg"
              color="primary"
              variant="outlined"
              :loading="isSaving"
              @click="cancel"
            >
              Cancel
            </v-btn>
          </div>
        </v-col>
      </v-row>
    </template>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import Loading from "@/components/Loading.vue";
import CampusForm from "@/components/CampusForm.vue";
import Alert from "@/components/Alert.vue";
import {
  getEducationInstitution,
  updateCampus,
} from "@/api/education-institution";
import { getUsers } from "@/api/manage-users";
import { useAlertStore } from "@/store/alert";
import { useRouter } from "vue-router";
import type { Components, PspUserListItem } from "@/types/openapi";
import type { PspUserItem } from "@/components/inputs/EcePspUser.vue";

export default defineComponent({
  name: "EditCampus",
  components: { PageContainer, Breadcrumb, Loading, CampusForm, Alert },
  props: {
    institutionId: {
      type: String,
      required: true,
    },
    campusId: {
      type: String,
      required: true,
    },
  },
  setup() {
    const alertStore = useAlertStore();
    const router = useRouter();
    return { alertStore, router };
  },
  data() {
    return {
      institution: null as Components.Schemas.EducationInstitution | null,
      campus: null as Components.Schemas.Campus | null,
      pspUsers: [] as PspUserItem[],
      isLoading: true,
      isSaving: false,
    };
  },
  computed: {
    isPrivate(): boolean {
      return this.institution?.institutionType === "Private";
    },
    institutionName(): string {
      return this.institution?.name ?? "";
    },
  },
  async mounted() {
    const [institution, userList] = await Promise.all([
      getEducationInstitution(),
      getUsers(),
    ]);

    if (!institution) {
      this.alertStore.setFailureAlert(
        "Unable to load institution. Please try again.",
      );
      await this.router.push({
        name: "education-institution",
        params: { institutionId: this.institutionId },
      });
      return;
    }

    this.institution = institution;
    this.campus =
      institution.campuses?.find((c) => c.id === this.campusId) ?? null;

    if (!this.campus) {
      this.alertStore.setFailureAlert("Campus not found.");
      await this.router.push({
        name: "education-institution",
        params: { institutionId: this.institutionId },
      });
      return;
    }

    this.pspUsers = (userList ?? []).map((user: PspUserListItem) => ({
      id: user.id ?? "",
      name: `${user.profile?.firstName ?? ""} ${user.profile?.lastName ?? ""}`.trim(),
    }));

    this.isLoading = false;
  },
  methods: {
    async save() {
      const campusFormRef = this.$refs.campusFormRef as InstanceType<
        typeof CampusForm
      >;
      const { valid } = await campusFormRef.validate();

      if (!valid) {
        this.alertStore.setFailureAlert(
          "You must enter all required fields in the valid format.",
        );
        return;
      }

      this.isSaving = true;
      try {
        const { campus } = campusFormRef.getData();
        const response = await updateCampus(this.campusId, campus);
        if (response?.data === "") {
          this.alertStore.setSuccessAlert(
            "Campus has been successfully updated.",
          );
          await this.router.push({
            name: "campus",
            params: {
              institutionId: this.institutionId,
              campusId: this.campusId,
            },
          });
        } else if (response?.error?.status == 400) {
          let message = response?.error?.title
            ? response?.error?.title
            : "Failed to update campus. Please try again.";
          this.alertStore.setFailureAlert(message);
        } else {
          this.alertStore.setFailureAlert(
            "Failed to update campus. Please try again.",
          );
        }
      } finally {
        this.isSaving = false;
      }
    },
    cancel() {
      this.router.push({
        name: "campus",
        params: { institutionId: this.institutionId, campusId: this.campusId },
      });
    },
  },
});
</script>
