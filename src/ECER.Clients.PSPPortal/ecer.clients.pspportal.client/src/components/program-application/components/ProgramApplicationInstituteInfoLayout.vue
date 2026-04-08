<template>
  <PageContainer>
    <Loading v-if="isLoading" />
    <template v-else>
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

      <slot
        name="application-summary"
        :program-type="programType"
        :institution="userStore.educationInstitution"
        :program-application-object="programApplicationObject"
      >
        <v-row>
          <v-col cols="12" sm="4" xl="3">Institution name</v-col>
          <v-col class="font-weight-bold" cols="12" sm="8" xl="9">
            {{ userStore.educationInstitution?.name }}
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" sm="4" xl="3">Campus</v-col>
          <v-col class="font-weight-bold" cols="12" sm="8" xl="9">
            {{ programApplicationObject?.programCampuses?.[0]?.name }}
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" sm="4" xl="3">Program profile</v-col>
          <v-col class="font-weight-bold" cols="12" sm="8" xl="9">
            {{ programApplicationObject?.programProfileName }}
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" sm="4" xl="3">Provincial certification types</v-col>
          <v-col class="font-weight-bold" cols="12" sm="8" xl="9">
            {{ programType }}
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" sm="4" md="4" xl="3">Delivery method</v-col>
          <v-col class="font-weight-bold" cols="12" sm="8" xl="9">
            {{ programApplicationObject?.deliveryType }}
          </v-col>
        </v-row>
      </slot>

      <v-form ref="instituteInfoForm">
        <template v-if="showSection('campus-section')">
          <slot name="campus-section">
            <v-row>
              <v-col cols="12">
                <slot name="campus-section-title">
                  <h2>Campus</h2>
                </slot>
              </v-col>
            </v-row>
            <v-row>
              <v-col cols="12">
                <slot name="campus-section-subtitle">
                  <p>
                    Select where this program will be offered. A first-time
                    application for a basic early childhood education program is
                    restricted to one campus.
                  </p>
                </slot>
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
                    userStore.educationInstitution?.institutionType ===
                      'Private' &&
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
                    :label="campus.generatedName || '-'"
                    density="compact"
                    hide-details
                    @update:model-value="
                      (checked) => onCampusChange(campus.id ?? null, checked)
                    "
                    :disabled="
                      userStore.educationInstitution?.institutionType ===
                        'Private' &&
                      programApplicationObject !== null &&
                      programApplicationObject.programCampuses !== null &&
                      programApplicationObject.programCampuses !== undefined &&
                      programApplicationObject.programCampuses.length >= 1 &&
                      campus.id !== null &&
                      campus.id !== undefined &&
                      programApplicationObject.programCampuses.some(
                        (camp) => camp.campusId !== campus.id,
                      )
                    "
                  />
                </v-col>
              </v-row>
            </v-input>
            <div class="no-campus text-grey-dark pb-3" v-else>
              No campus available
            </div>
          </slot>
        </template>

        <v-row>
          <v-col cols="12">
            <h2>Contact person</h2>
          </v-col>
        </v-row>

        <v-row>
          <v-col cols="12">
            <p>
              Select the individual contact for this campus or location. This
              user must have access to the ECE Post-Secondary Programs portal.
            </p>
            <br />
            <!-- prettier-ignore -->
            <p>
              Note: If the correct user is not listed here, please invite them
              to this portal under
              "<router-link
                class="user-link"
                :to="{
                  name: 'manage-users',
                  params: {
                    educationInstitutionName:
                      userStore.educationInstitution?.name,
                  },
                }"
              >
                Manage users
              </router-link>".
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

        <slot name="program-info-prefix" />

        <v-row>
          <v-col cols="12">
            <div class="d-flex flex-column ga-3">
              <EceTextField
                v-model="programName"
                label="Program name"
                :rules="[Rules.required('Enter a program name')]"
              ></EceTextField>
            </div>
          </v-col>
        </v-row>

        <template v-if="showSection('program-length-field')">
          <slot name="program-length-field">
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
          </slot>
        </template>

        <template v-if="showSection('delivery-section')">
          <slot name="delivery-section">
            <template v-if="showDeliverySection()">
              <v-row class="mb-n8">
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

              <v-row class="mb-n8">
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
            </template>
          </slot>
        </template>

        <slot
          name="satellite-fields"
          :satellite-start-date="satelliteStartDate"
          :on-update-satellite-start-date="
            (v: string | null) => (satelliteStartDate = v)
          "
          :satellite-end-date="satelliteEndDate"
          :on-update-satellite-end-date="
            (v: string | null) => (satelliteEndDate = v)
          "
          :end-date-validation="dateBeforeRule"
          :required-validation="Rules.required"
        />

        <v-row>
          <v-col cols="12">
            <h2>Enrollment and admission</h2>
          </v-col>
        </v-row>

        <template v-if="showSection('enrollment-section')">
          <slot name="enrollment-section">
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
          </slot>
        </template>

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

        <!-- Delivery Hours Section for Online/Hybrid -->
        <slot
          name="delivery-hours-section-for-online-hybrid"
          :program-application-object="programApplicationObject"
          :in-person-hours-percentage="inPersonHoursPercentage"
          :online-delivery-hours-percentage="onlineDeliveryHoursPercentage"
          :on-update-in-person-hours-percentage="
            (v: number | null) => (inPersonHoursPercentage = v)
          "
          :on-update-online-delivery-hours-percentage="
            (v: number | null) => (onlineDeliveryHoursPercentage = v)
          "
        />

        <v-row>
          <v-col>
            <v-btn
              rounded="lg"
              :loading="isSaving"
              color="primary"
              @click="saveAndContinue"
            >
              Continue
            </v-btn>
          </v-col>
        </v-row>
      </v-form>
    </template>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useUserStore } from "@/store/user";
import { getUsers } from "@/api/manage-users";
import type { PspUserListItem, Components } from "@/types/openapi";
import PageContainer from "@/components/PageContainer.vue";
import { cloneDeep } from "lodash";
import type { VForm } from "vuetify/components";
import * as Rules from "@/utils/formRules";
import EceTextField from "@/components/inputs/EceTextField.vue";
import {
  mapProgramType,
  updateProgramApplication,
  getProgramApplicationById,
} from "@/api/program-application";
import type { NextStepPayload } from "@/components/program-application/ProgramApplication.vue";
import Loading from "@/components/Loading.vue";
import type { ProgramApplicationContact } from "@/types/helperFunctions";
import EceDateInput from "@/components/inputs/EceDateInput.vue";
import { dateBeforeRule } from "@/utils/formRules";

export default defineComponent({
  name: "ProgramApplicationInstituteInfoLayout",
  components: { EceDateInput, PageContainer, EceTextField, Loading },
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
  },
  setup() {
    const userStore = useUserStore();
    return {
      userStore,
    };
  },
  emits: { next: (_payload: NextStepPayload) => true },
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
          this.programApplicationObject.programRepresentativeId =
            value === "" ? null : value;
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
          this.programApplicationObject.programLength =
            value === "" ? null : value;
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
          this.programApplicationObject.minimumEnrollment =
            value === "" ? null : value;
        }
      },
    },
    maximumEnrollment: {
      get() {
        return this.programApplicationObject?.maximumEnrollment ?? null;
      },
      set(value: string | null) {
        if (this.programApplicationObject) {
          this.programApplicationObject.maximumEnrollment =
            value === "" ? null : value;
        }
      },
    },
    inPersonHoursPercentage: {
      get() {
        return this.programApplicationObject?.inPersonHoursPercentage ?? null;
      },
      set(value: string | null) {
        if (this.programApplicationObject) {
          this.programApplicationObject.inPersonHoursPercentage =
            value === "" || value === null ? null : parseFloat(value);
        }
      },
    },
    onlineDeliveryHoursPercentage: {
      get() {
        return (
          this.programApplicationObject?.onlineDeliveryHoursPercentage ?? null
        );
      },
      set(value: string | null) {
        if (this.programApplicationObject) {
          this.programApplicationObject.onlineDeliveryHoursPercentage =
            value === "" || value === null ? null : parseFloat(value);
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
    satelliteStartDate: {
      get(): string | null {
        return (
          this.programApplicationObject?.programCampuses?.[0]?.startDate ?? null
        );
      },
      set(value: string | null) {
        if (this.programApplicationObject?.programCampuses?.[0]) {
          this.programApplicationObject.programCampuses[0].startDate =
            value ?? null;
        }
      },
    },
    satelliteEndDate: {
      get(): string | null {
        return (
          this.programApplicationObject?.programCampuses?.[0]?.endDate ?? null
        );
      },
      set(value: string | null) {
        if (this.programApplicationObject?.programCampuses?.[0]) {
          this.programApplicationObject.programCampuses[0].endDate =
            value ?? null;
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
      isLoading: true,
      isSaving: false,
    };
  },
  async mounted() {
    await this.loadUsers();
    await this.fetchApplication();

    this.programCampus =
      this.programApplicationObject?.programCampuses?.map(
        (campus) => campus.campusId,
      ) ?? [];
    this.isLoading = false;
  },
  methods: {
    dateBeforeRule,
    async fetchApplication() {
      const result = await getProgramApplicationById(this.programApplicationId);
      if (result.error || result.data == null) {
        console.error("Failed to retrieve program application:", result.error);
      } else {
        this.programApplicationObject = cloneDeep(result.data);
      }
    },
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
          name: `${currentUser.profile?.firstName} ${currentUser.profile?.lastName}`,
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
      this.isSaving = true;
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
          } else {
            this.isSaving = false;
            this.$emit("next", {});
          }
        }
      } finally {
        this.isSaving = false;
      }
    },
    //This is so we can pass an empty slot to override default slot
    //Vue doesn't do it by default
    showSection(name: string): boolean {
      return !this.$slots[name] || (this.$slots[name]?.() ?? []).length > 0;
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
            this.programApplicationObject.programCampuses.some(
              (campus) => campus.campusId === campusId,
            )
          ) {
            this.programApplicationObject.programCampuses =
              this.programApplicationObject.programCampuses?.filter(
                (campus) => campus.campusId !== campusId,
              ) ?? [];
          } else {
            this.programApplicationObject.programCampuses.push({
              id: null,
              campusId: campusId,
            });
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
