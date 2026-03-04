<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <h1>Institution and program information</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <h2>Application information</h2>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="3">Provincial Certification Type</v-col>
      <v-col class="font-weight-bold" cols="3">
        {{ programType }}
      </v-col>
    </v-row>
    <v-row class="mt-n3">
      <v-col cols="3">Delivery method</v-col>
      <v-col class="font-weight-bold" cols="3">
        {{ programApplicationStore.programApplication?.deliveryType }}
      </v-col>
    </v-row>
    <v-row class="mt-n3">
      <v-col cols="3">Institution</v-col>
      <v-col class="font-weight-bold" cols="3">
        {{ userStore.educationInstitution?.name }}
      </v-col>
    </v-row>

    <v-form ref="instituteInfoForm">
      <v-row>
        <v-col cols="12">
          <h2>Campus</h2>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12">
          <p>
            Select where this program will be offered. A first-time application
            for a basic early childhood education program is restricted to one
            campus.
          </p>
        </v-col>
      </v-row>

      <v-input
        v-if="
          userStore.educationInstitution?.campuses &&
          userStore.educationInstitution?.campuses.length > 0
        "
        v-model="programCampus"
        :rules="[
          (v) =>
            !(
              userStore.educationInstitution?.auspice === 'Private' &&
              v &&
              v.length > 1
            ) || 'Private institutions can only select a single campus',
        ]"
        class="pb-3"
      >
        <v-row dense>
          <v-col
            v-for="campus in userStore.educationInstitution?.campuses"
            cols="12"
          >
            <v-checkbox
              v-model="programCampus"
              :value="campus.id || null"
              :label="campus.name || '-'"
              density="compact"
              hide-details
              @update:model-value="
                (checked) => onCampusChange(campus.id ?? null, checked)
              "
              :disabled="
                userStore.educationInstitution?.auspice === 'Private' &&
                programApplicationObject !== null &&
                programApplicationObject.programCampuses !== null &&
                programApplicationObject.programCampuses !== undefined &&
                programApplicationObject.programCampuses.length >= 1 &&
                campus.id !== null &&
                campus.id !== undefined &&
                !programApplicationObject.programCampuses.find(
                  (camp) => camp.campusId === campus.id,
                )
              "
            />
          </v-col>
        </v-row>
      </v-input>
      <div class="no-campus text-grey-dark pb-3" v-else>
        No campus available
      </div>

      <v-row>
        <v-col cols="12">
          <h2>Contact person</h2>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <p>
            Select the individual responsible for this program application. This
            user must have access to the PSP Portal.
          </p>
          <p>
            Note: If the correct user is not listed here, please invite them to
            this portal under
            <router-link
              class="user-link"
              :to="{
                name: 'manage-users',
                params: {
                  educationInstitutionName:
                    userStore.educationInstitution?.name,
                },
              }"
            >
              "Manage users"
            </router-link>
            .
          </p>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="6">
          <p>Select a user from {{ userStore.educationInstitution?.name }}</p>
          <v-select
            v-model="programRepresentativeId"
            class="mt-2"
            variant="outlined"
            :items="contacts"
            item-title="name"
            item-value="id"
            :rules="[Rules.required('Required')]"
            hide-details="auto"
          ></v-select>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <h2>Program information</h2>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <p>
            To apply for a post-basic early childhood education program (ITE
            and/or SNE), an institution must either:
          </p>
          <ul class="ml-6">
            <li>
              Already have a recognized basic early childhood education program,
              or
            </li>
            <li>
              Apply for a basic early childhood education and post-basic early
              childhood education program concurrently.
            </li>
          </ul>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <div class="d-flex flex-column ga-3">
            <EceTextField
              v-model="programName"
              label="Program information"
              :rules="[Rules.required('Enter a program name')]"
            ></EceTextField>
          </div>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="3">
          <div class="d-flex flex-column ga-3">
            <EceTextField
              v-model="programLength"
              label="Program length"
              :rules="[validateHours]"
            ></EceTextField>
          </div>
        </v-col>
        <v-col cols="1" class="mt-6 align-content-center">
          <div class="d-flex flex-column">Months</div>
        </v-col>
      </v-row>

      <v-row class="mb-n8" v-if="showDeliverySection()">
        <v-col cols="12">
          <p class="pb-3">Online method of instruction</p>
          <v-input
            v-model="methodOfInstruction"
            label="Online method of instruction"
            class="pb-3"
          >
            <v-row dense>
              <v-col cols="12">
                <v-checkbox
                  v-model="methodOfInstruction"
                  label="Synchronous - Instructor and students meet in real time"
                  value="Synchronous"
                  density="compact"
                  hide-details
                />
              </v-col>
              <v-col cols="12">
                <v-checkbox
                  v-model="methodOfInstruction"
                  label="Asynchronous - Students complete instructional hours and activities on their own time"
                  value="Asynchronous"
                  density="compact"
                  hide-details
                />
              </v-col>
            </v-row>
          </v-input>
        </v-col>
      </v-row>

      <v-row class="mb-n8" v-if="showDeliverySection()">
        <v-col cols="12">
          <p class="pb-3">
            Delivery method for practicum instructor supervision
          </p>
          <v-input
            v-model="deliveryMethodforInstructor"
            label="Delivery method for practicum instructor supervision"
            class="pb-3"
          >
            <v-row dense>
              <v-col cols="12">
                <v-checkbox
                  v-model="deliveryMethodforInstructor"
                  label="In-person site visits"
                  value="Inpersonsitevisits"
                  density="compact"
                  hide-details
                />
              </v-col>
              <v-col cols="12">
                <v-checkbox
                  v-model="deliveryMethodforInstructor"
                  label="Virtual site visits"
                  value="Virtualsitevisits"
                  density="compact"
                  hide-details
                />
              </v-col>
            </v-row>
          </v-input>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <h2>Enrollment and admission</h2>
        </v-col>
      </v-row>

      <v-row class="mb-n8">
        <v-col cols="12">
          <p class="pb-3">Program enrollment options</p>
          <v-input
            v-model="enrollmentOptions"
            label="Program enrollment options"
            class="pb-3"
          >
            <v-row dense>
              <v-col cols="12">
                <v-checkbox
                  v-model="enrollmentOptions"
                  label="Part-time"
                  value="PartTime"
                  density="compact"
                  hide-details
                />
              </v-col>
              <v-col cols="12">
                <v-checkbox
                  v-model="enrollmentOptions"
                  label="Full-time"
                  value="FullTime"
                  density="compact"
                  hide-details
                />
              </v-col>
            </v-row>
          </v-input>
        </v-col>
      </v-row>

      <v-row class="mb-n8">
        <v-col cols="12">
          <p class="pb-3">Admission options - select all that apply</p>
          <v-input
            v-model="admissionOptions"
            label="Admission options - select all that apply"
            class="pb-3"
          >
            <v-row dense>
              <v-col cols="12">
                <v-checkbox
                  v-model="admissionOptions"
                  label="All courses restricted to early childhood education students"
                  value="Allcoursesrestrictedtoearlychildhoodeducationstudents"
                  density="compact"
                  hide-details
                />
              </v-col>
              <v-col cols="12">
                <v-checkbox
                  v-model="admissionOptions"
                  label="One or more courses open to any students in the institution"
                  value="Oneormorecoursesopentoanystudentsintheinstitution"
                  density="compact"
                  hide-details
                />
              </v-col>
              <v-col cols="12">
                <v-checkbox
                  v-model="admissionOptions"
                  label="Cohort enrollment - students start together and graduate together"
                  value="Cohortenrollmentstudentsstarttogetherandgraduatetogether"
                  density="compact"
                  hide-details
                />
              </v-col>
              <v-col cols="12">
                <v-checkbox
                  v-model="admissionOptions"
                  label="Continuous enrollment - students can enrol at any time"
                  value="Continuousenrollmentstudentscanenrolatanytime"
                  density="compact"
                  hide-details
                />
              </v-col>
              <v-col cols="12">
                <v-checkbox
                  v-model="admissionOptions"
                  label="Other"
                  value="Other"
                  density="compact"
                  hide-details
                />
              </v-col>
            </v-row>
          </v-input>
        </v-col>
      </v-row>

      <v-row v-if="admissionOptions?.includes('Other')">
        <v-col cols="12">
          <EceTextField
            v-model="otherAdmissionOptions"
            label="List or describe the other admission options"
            hide-details="auto"
          />
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <p>Anticipated student enrollment numbers</p>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="4">
          <div class="d-flex flex-column ga-3">
            <EceTextField
              v-model="minimumEnrollment"
              label="Minimum enrollment per course"
              :rules="[validateHours]"
            ></EceTextField>
          </div>
        </v-col>
        <v-col cols="4">
          <div class="d-flex flex-column ga-3">
            <EceTextField
              v-model="maximumEnrollment"
              label="Maximum enrollment per course"
              :rules="[validateHours]"
            ></EceTextField>
          </div>
        </v-col>
      </v-row>

      <v-row>
        <v-col>
          <v-btn
            rounded="lg"
            :loading="isLoading"
            color="primary"
            @click="saveAndContinue"
          >
            Save and Continue
          </v-btn>
        </v-col>
      </v-row>
    </v-form>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useUserStore } from "@/store/user";
import { getUsers } from "@/api/manage-users";
import type { PspUserListItem } from "@/types/openapi";
import PageContainer from "@/components/PageContainer.vue";
import { useProgramApplicationStore } from "@/store/programApplication";
import { cloneDeep } from "lodash";
import type { Components } from "@/types/openapi";
import type { VForm } from "vuetify/components";
import * as Rules from "@/utils/formRules";
import EceTextField from "@/components/inputs/EceTextField.vue";
import {
  mapProgramType,
  updateProgramApplication,
} from "@/api/program-application";

export default defineComponent({
  name: "ProgramApplicationInstituteInfo",
  components: { PageContainer, EceTextField },
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
  },
  setup() {
    const userStore = useUserStore();
    const programApplicationStore = useProgramApplicationStore();
    return {
      userStore,
      programApplicationStore,
    };
  },
  computed: {
    validateHours() {
      return (v: string) => {
        if (!v) return true;
        const numValue = Number.parseFloat(v);
        if (Number.isNaN(numValue)) {
          return "Enter a valid number";
        }
        if (numValue < 0) {
          return "Must be positive";
        }
        return Rules.numberToDecimalPlaces(2)(v);
      };
    },
    programRepresentativeId: {
      get() {
        return this.programApplicationObject?.programRepresentativeId ?? null;
      },
      set(value: string | null) {
        if (this.programApplicationObject) {
          this.programApplicationObject.programRepresentativeId = value;
        }
      },
    },
    otherAdmissionOptions: {
      get() {
        return this.programApplicationObject?.otherAdmissionOptions ?? null;
      },
      set(value: string | null) {
        if (this.programApplicationObject) {
          this.programApplicationObject.otherAdmissionOptions = value;
        }
      },
    },
    programName: {
      get() {
        return this.programApplicationObject?.programApplicationName ?? null;
      },
      set(value: string | null) {
        if (this.programApplicationObject) {
          this.programApplicationObject.programApplicationName = value;
        }
      },
    },
    programLength: {
      get() {
        return this.programApplicationObject?.programLength ?? null;
      },
      set(value: string | null) {
        if (this.programApplicationObject) {
          this.programApplicationObject.programLength = value;
        }
      },
    },
    methodOfInstruction: {
      get() {
        return this.programApplicationObject?.onlineMethodOfInstruction ?? null;
      },
      set(value: [] | null) {
        if (this.programApplicationObject) {
          this.programApplicationObject.onlineMethodOfInstruction = value;
        }
      },
    },
    deliveryMethodforInstructor: {
      get() {
        return this.programApplicationObject?.deliveryMethod ?? null;
      },
      set(value: [] | null) {
        if (this.programApplicationObject) {
          this.programApplicationObject.deliveryMethod = value;
        }
      },
    },
    enrollmentOptions: {
      get() {
        return this.programApplicationObject?.enrollmentOptions ?? null;
      },
      set(value: [] | null) {
        if (this.programApplicationObject) {
          this.programApplicationObject.enrollmentOptions = value;
        }
      },
    },
    minimumEnrollment: {
      get() {
        return this.programApplicationObject?.minimumEnrollment ?? null;
      },
      set(value: string | null) {
        if (this.programApplicationObject) {
          this.programApplicationObject.minimumEnrollment = value;
        }
      },
    },
    maximumEnrollment: {
      get() {
        return this.programApplicationObject?.maximumEnrollment ?? null;
      },
      set(value: string | null) {
        if (this.programApplicationObject) {
          this.programApplicationObject.maximumEnrollment = value;
        }
      },
    },
    admissionOptions: {
      get() {
        return this.programApplicationObject?.admissionOptions ?? null;
      },
      set(value: [] | null) {
        if (this.programApplicationObject) {
          this.programApplicationObject.admissionOptions = value;
        }
      },
    },
    programType(): string {
      const types = this.programApplicationObject?.programTypes;
      if (!types?.length) return "—";
      return types.map(mapProgramType).join(", ");
    },
  },
  data() {
    return {
      users: [] as PspUserListItem[],
      programApplicationObject:
        null as Components.Schemas.ProgramApplication | null,
      contacts: [] as ProgramApplicationContact[],
      Rules,
      programCampus: [] as (string | null | undefined)[],
      isLoading: false,
    };
  },
  async mounted() {
    await this.loadUsers();
    this.programApplicationObject = cloneDeep(
      this.programApplicationStore.programApplication,
    );
    this.programCampus =
      this.programApplicationObject?.programCampuses?.map(
        (campus) => campus.campusId,
      ) ?? [];
  },
  methods: {
    async loadUsers() {
      this.users = (await getUsers()) ?? [];
      let currentRepId = this.programApplicationObject?.programRepresentativeId;
      const currentUser = this.users.find(
        (user: PspUserListItem) => user.id === currentRepId,
      );
      const otherUsers = this.users.filter(
        (user: PspUserListItem) => user.id !== currentRepId,
      );

      const sortedOtherUsers = otherUsers.sort(
        (a: PspUserListItem, b: PspUserListItem) => {
          const nameA = `${a.profile?.firstName} ${a.profile?.lastName}`.trim();
          const nameB = `${b.profile?.firstName} ${b.profile?.lastName}`.trim();
          return nameA.localeCompare(nameB);
        },
      );

      if (currentUser) {
        this.contacts.push({
          id: currentUser.id,
          name: `${currentUser.profile?.firstName} ${currentUser.profile?.lastName} (You)`,
        });
      }

      sortedOtherUsers.forEach((user: PspUserListItem) => {
        this.contacts.push({
          id: user.id,
          name: `${user.profile?.firstName} ${user.profile?.lastName}`,
        });
      });
      this.programRepresentativeId = currentUser?.id || null;
    },
    async saveAndContinue() {
      const { valid } = await (
        this.$refs.instituteInfoForm as VForm
      ).validate();

      if (!valid) return;
      this.isLoading = true;
      try {
        if (this.programApplicationObject !== null) {
          const response = await updateProgramApplication(
            this.programApplicationObject,
          );
          if (response.error) {
            console.error(
              "Failed to update program application:",
              response.error,
            );
          }
        }
      } finally {
        this.isLoading = false;
      }
    },
    showDeliverySection(): boolean {
      return this.programApplicationObject?.deliveryType !== "Inperson";
    },
    onCampusChange(
      campusId: string | null,
      checked: (string | null | undefined)[] | null,
    ) {
      if (checked) {
        if (
          this.programApplicationObject !== null &&
          !this.programApplicationObject.programCampuses
        ) {
          this.programApplicationObject.programCampuses = [];
        }

        if (
          this.programApplicationObject !== null &&
          this.programApplicationObject.programCampuses
        ) {
          if (
            !this.programApplicationObject.programCampuses.find(
              (campus) => campus.campusId === campusId,
            )
          ) {
            this.programApplicationObject.programCampuses.push({
              id: null,
              campusId: campusId,
            });
          } else {
            this.programApplicationObject.programCampuses =
              this.programApplicationObject.programCampuses?.filter(
                (campus) => campus.campusId !== campusId,
              ) ?? [];
          }
        }
      }
    },
  },
});
</script>
<style scoped>
.user-link {
  color: #1a5a96 !important;
  text-decoration: underline;
}

.no-campus {
  font-style: italic;
}
</style>
