<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>
    <v-row>
      <v-col class="ml-1" cols="12">
        <h1>Check your transfer eligibility</h1>
        <p class="mt-5">
          Answer a few questions to see if you are eligible to apply to transfer
          your certification from another province or territory to B.C.
        </p>
        <h2 class="mt-5">Out-of-province registrant information</h2>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <p class="mt-8">
              In which Canadian province or territory are you certified?
            </p>
            <v-select
              id="province"
              class="pt-2"
              :items="
                configStore.provinceList.filter(
                  (p) =>
                    p.provinceName !== 'British Columbia' &&
                    p.provinceName !== 'Other',
                )
              "
              variant="outlined"
              label=""
              v-model="province"
              item-title="provinceName"
              item-value="provinceId"
              :rules="[
                Rules.required(
                  'Select your province or territory',
                  'provinceId',
                ),
              ]"
              @update:modelValue="onProvinceChange"
              return-object
            ></v-select>
          </v-col>
        </v-row>
        <v-row>
          <v-col
            v-if="outOfProvinceCertificationTypes.length > 0"
            md="8"
            lg="6"
            xl="4"
          >
            <p class="mt-8">What certification do you have?</p>
            <v-select
              id="outOfProvinceCertification"
              class="pt-2"
              variant="outlined"
              label=""
              :items="outOfProvinceCertificationTypes"
              v-model="outOfProvinceCertification"
              item-title="transferringCertificate.certificationType"
              item-value="transferringCertificate.id"
              :rules="[
                Rules.required(
                  'Select your out of province certification type',
                  'transferringCertificate',
                ),
              ]"
              @update:modelValue="onCertificationChange"
              return-object
            ></v-select>
          </v-col>
          <v-col
            v-if="
              outOfProvinceCertificationTypes.length === 0 &&
              province &&
              !outOfProvinceCertificationTypesLoading
            "
            class="ml-1"
            cols="12"
          >
            <Callout
              type="warning"
              title="The province or territory you hold certification in is not eligible to be transferred to B.C."
            >
              <p>
                You need to
                <router-link to="/application/certification">
                  apply for certification
                </router-link>
                through a different pathway.
              </p>
            </Callout>
          </v-col>
        </v-row>
        <v-row
          v-if="
            highestCertificationType &&
            (filteredCertificationOptions?.length ?? 0) > 1 &&
            !certificationStore.holdsEceFiveYearCertification
          "
        >
          <v-col class="ml-1" cols="12">
            <h2 class="mt-5">Work experience</h2>
            <p class="mt-5">
              Your Canadian work experience determines your eligibility to apply
              for a transfer for either an ECE One Year or ECE Five Year
              certification in B.C.
            </p>
            <p class="mt-5">Your work experience:</p>
            <ul class="mt-3 ml-8">
              <li>
                Includes any work or volunteer hours completed within the last 5
                years
              </li>
              <li>
                Cannot include your practicum hours (hours that were a part of
                your education)
              </li>
              <li>Must be observed by Canadian-certified ECE</li>
            </ul>
            <p class="mt-5">
              Do you have 500 hours of Canadian work experience?
            </p>
            <v-radio-group
              class="mt-3"
              id="programConfirmationRadio"
              v-model="has500HoursWorkExperience"
              :rules="[Rules.required()]"
              color="primary"
            >
              <v-radio label="Yes" value="true"></v-radio>
              <v-radio label="No" value="false"></v-radio>
            </v-radio-group>
          </v-col>
        </v-row>
        <!-- User has an active five year certificate we show different options to see if they can upgrade -->
        <v-row
          v-else-if="
            highestCertificationType &&
            activeFiveYearCertification &&
            filteredCertificationOptions?.length !== 1
          "
        >
          <v-col class="ml-1" cols="12">
            <h2 class="mt-5">Certification level</h2>
            <p class="mt-5">
              Your certification can apply to transfer to either an ECE One Year
              or to add {{ textForUpgradeOptionsWithActiveFiveYear }} to your
              ECE Five Year certification in B.C.
            </p>
            <p class="mt-5">
              Which certification would you like to transfer with this
              application?
            </p>
            <v-radio-group
              class="mt-3"
              id="upgradeOptionsRadio"
              v-model="upgradeMyFiveYearCertification"
              :rules="[Rules.requiredRadio()]"
              color="primary"
            >
              <v-radio
                :label="labelForUpgradeOptionRadioFiveYearOptions"
                :value="true"
              ></v-radio>
              <v-radio
                label="Apply for ECE One Year certification"
                :value="false"
              ></v-radio>
            </v-radio-group>
          </v-col>
        </v-row>
        <!-- Section for callouts once we have enough information to make a pathway decision -->
        <v-row>
          <v-col>
            <Callout
              v-if="filteredCertificationOptions?.length === 0"
              type="warning"
              title="You are not eligible to transfer this certification"
            >
              <p class="mt-3">
                Your certification from another province transfers to a
                certification level you have already held in B.C. You can renew
                or
                <router-link to="/application/certification">
                  apply for certification
                </router-link>
                on your dashboard through a different pathway.
              </p>
            </Callout>
            <Callout
              v-if="highestCertificationType === CertificationType.Assistant"
              type="warning"
              title="You can apply to transfer your certification to ECE Assistant certification in B.C."
            >
              <p>
                You will be able to work alongside ECEs and/or Infant and
                Toddler Educators in licensed child care programs for children
                birth to 5 years of age.
              </p>
            </Callout>
            <Callout
              v-if="
                highestCertificationType === CertificationType.OneYear ||
                has500HoursWorkExperience === 'false' ||
                upgradeMyFiveYearCertification === false
              "
              type="warning"
              title="You can apply to transfer your certification to ECE One Year certification in B.C."
            >
              <p class="mt-3">You will be able to work:</p>
              <ul class="mt-2 mb-8 ml-8">
                <li>
                  alone and/or as the primary educator in licensed child care
                  programs for children 3 to 5 years of age, and/or
                </li>
                <li>
                  alongside an Infant and Toddler Educator in licensed programs
                  of children under 36 months
                </li>
              </ul>
              <p>
                Once you get 500 hours of work experience observed by a
                Canadian-certified ECE, you can apply to upgrade to an ECE Five
                Year with Infant and Toddler Educator (ITE) and Special Needs
                Educator (SNE).
              </p>
            </Callout>
            <!-- Five year variations -->
            <Callout
              v-if="
                (has500HoursWorkExperience === 'true' ||
                  upgradeMyFiveYearCertification === true ||
                  filteredCertificationOptions?.length === 1) &&
                highestCertificationType ===
                  CertificationType.FiveYearCertificate
              "
              type="warning"
              title="You can apply to transfer your certification to ECE Five Year certification in B.C."
            >
              <p class="mt-3">You will be able to work:</p>
              <ul class="mt-2 ml-8">
                <li>
                  alone and/or as the primary educator in licensed child care
                  programs for children 3 to 5 years of age, and/or
                </li>
                <li>
                  alongside an Infant and Toddler Educator in licensed programs
                  of children under 36 months
                </li>
              </ul>
            </Callout>
            <Callout
              v-else-if="
                (has500HoursWorkExperience === 'true' ||
                  filteredCertificationOptions?.length === 1) &&
                highestCertificationType ===
                  CertificationType.FiveYearCertificateITE_SNE &&
                !certificationStore.holdsEceFiveYearCertification
              "
              type="warning"
              title="You can apply to transfer your certification to ECE Five Year certification with Infant and Toddler Educator (ITE) and Special Needs Educator (SNE) in B.C."
            >
              <p strong class="mt-3">
                An ECE Five Year certification allows you to work in licensed
                child care programs:
              </p>
              <ul class="mt-2 mb-8 ml-8">
                <li>
                  alone and/or as the primary educator in licensed child care
                  programs for children 3 to 5 years of age, and/or
                </li>
                <li>
                  alongside an Infant and Toddler Educator in licensed programs
                  of children under 36 months
                </li>
              </ul>
              <p strong class="mt-3">
                Specialized certifications allow you to work in licensed child
                care programs:
              </p>
              <ul class="mt-2 ml-8">
                <li>
                  alone and/or as the primary educator with children birth to 5
                  years (ITE)
                </li>
                <li>
                  alone and/or as the primary educator in inclusive settings
                  with children 3-5 years of age (SNE)
                </li>
              </ul>
            </Callout>
            <Callout
              v-if="
                (upgradeMyFiveYearCertification === true ||
                  filteredCertificationOptions?.length === 1) &&
                highestCertificationType ===
                  CertificationType.FiveYearCertificateITE_SNE &&
                remainingPostBasic === 'ITE'
              "
              type="warning"
              title="You can apply to transfer your certification to B.C. to add Infant and Toddler Educator (ITE)"
            >
              This certification allows you to to work alone and/or as the
              primary educator in licensed child care programs for children
              birth to 5 years of age. It will also renew your ECE Five Year
              certificate. It is valid for 5 years.
            </Callout>
            <Callout
              v-if="
                (upgradeMyFiveYearCertification === true ||
                  filteredCertificationOptions?.length === 1) &&
                highestCertificationType ===
                  CertificationType.FiveYearCertificateITE_SNE &&
                remainingPostBasic === 'SNE'
              "
              type="warning"
              title="You can apply to transfer your certification to B.C. to add Special Needs Educator (SNE)"
            >
              This certification allows you to to work alone and/or as the
              primary educator in licensed child care programs for children
              birth to 5 years of age. It will also renew your ECE Five Year
              certificate. It is valid for 5 years.
            </Callout>
            <Callout
              v-if="
                (upgradeMyFiveYearCertification === true ||
                  filteredCertificationOptions?.length === 1) &&
                highestCertificationType ===
                  CertificationType.FiveYearCertificateITE_SNE &&
                remainingPostBasic === 'ITE_AND_SNE'
              "
              type="warning"
              title="You can apply to transfer your certification to B.C. to add Infant and Toddler Educator (ITE) and Special Needs Educator (SNE)"
            >
              <strong>
                Specialized certifications allow you to work in licensed child
                care programs:
              </strong>
              <br />
              <ul class="ml-10">
                <li>
                  alone and/or as the primary educator with children birth to 5
                  years (ITE)
                </li>
                <li>
                  alone and/or as the primary educator in inclusive settings
                  with children 3-5 years of age (SNE)
                </li>
              </ul>
            </Callout>
          </v-col>
        </v-row>
        <v-row>
          <v-col class="ml-1" cols="12">
            <v-btn
              id="btnViewRequirements"
              v-if="showRequirementsButton"
              @click="handleRequirementsClick"
              rounded="lg"
              color="primary"
            >
              View requirements
            </v-btn>
          </v-col>
        </v-row>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { getCertificationComparisonList } from "@/api/configuration";
import Breadcrumb from "@/components/Breadcrumb.vue";
import PageContainer from "@/components/PageContainer.vue";
import {
  getHighestCertificationType,
  CertificationType,
} from "@/utils/functions";
import { useConfigStore } from "@/store/config";
import { useApplicationStore } from "@/store/application";
import { useCertificationStore } from "@/store/certification";
import { useRouter } from "vue-router";
import * as Rules from "@/utils/formRules";
import type { Components, Province, ComparisonRecord } from "@/types/openapi";
import Callout from "@/components/Callout.vue";
import { useUserStore } from "@/store/user";
import { sortBy } from "lodash";

interface EceTransferData {
  province: Province | undefined;
  outOfProvinceCertification: ComparisonRecord | undefined;
}
export default {
  name: "Transfer",
  components: { Breadcrumb, PageContainer, Callout },
  setup() {
    const configStore = useConfigStore();
    const router = useRouter();
    const applicationStore = useApplicationStore();
    const userStore = useUserStore();
    const certificationStore = useCertificationStore();
    return {
      configStore,
      router,
      applicationStore,
      userStore,
      certificationStore,
    };
  },
  methods: {
    async onProvinceChange() {
      this.outOfProvinceCertificationTypesLoading = true;
      this.outOfProvinceCertification = undefined;
      this.has500HoursWorkExperience = undefined;
      this.upgradeMyFiveYearCertification = undefined;
      this.highestCertificationType = undefined;
      this.filteredCertificationOptions = undefined;
      if (this.transferData.province?.provinceId) {
        this.outOfProvinceCertificationTypes =
          sortBy(
            await getCertificationComparisonList(
              this.transferData.province?.provinceId,
            ),
            ["transferringCertificate.certificationType"],
          ) || [];
        this.outOfProvinceCertificationTypesLoading = false;
      } else {
        this.outOfProvinceCertificationTypes = [];
        this.outOfProvinceCertificationTypesLoading = false;
      }
    },
    async onCertificationChange() {
      this.highestCertificationType = undefined;
      this.has500HoursWorkExperience = undefined;
      this.upgradeMyFiveYearCertification = undefined;

      if (this.transferData.outOfProvinceCertification?.options) {
        //start filtering based on a user's existing certifications
        this.filteredCertificationOptions =
          this.transferData.outOfProvinceCertification?.options.filter(
            (option) => {
              // Five year certification level restrictions
              if (
                this.activeFiveYearCertification &&
                option.bcCertificate === CertificationType.FiveYearCertificate
              ) {
                return false;
              }

              if (
                this.activeFullyCertifiedFiveYearCertification &&
                option.bcCertificate ===
                  CertificationType.FiveYearCertificateITE_SNE
              ) {
                return false;
              }

              if (
                this.certificationStore.hasExpiredCertificationsOfType(
                  "ECE 5 YR",
                ) &&
                (option.bcCertificate ===
                  CertificationType.FiveYearCertificate ||
                  option.bcCertificate ===
                    CertificationType.FiveYearCertificateITE_SNE)
              ) {
                return false;
              }

              //one year rules
              //active one year certification should renew instead
              if (
                this.certificationStore.activeEceOneYearCertification &&
                option.bcCertificate === CertificationType.OneYear
              ) {
                return false;
              }

              //not allowed to apply for a new one year certificate without a 5 year cert reset
              if (
                this.certificationStore
                  .countMultipleEceOneYearCertificationsSinceLatest5YearIfExist >
                  0 &&
                option.bcCertificate === CertificationType.OneYear
              ) {
                return false;
              }

              if (
                this.certificationStore.activeEceFiveYearCertification &&
                option.bcCertificate === CertificationType.OneYear
              ) {
                return false;
              }

              //assistant rules
              if (
                this.certificationStore.hasCertificationsOfType("Assistant") &&
                option.bcCertificate === CertificationType.Assistant
              ) {
                return false;
              }

              return true;
            },
          );

        this.highestCertificationType = getHighestCertificationType(
          this.filteredCertificationOptions,
        );
      }
    },
    handleRequirementsClick() {
      // Set the labor mobility certificate information that we have now
      this.applicationStore.$patch({
        draftApplication: {
          certificationTypes: this.selfAssessmentOutcome,
          applicationType: "LabourMobility",
          stage: "CertificateInformation",
          labourMobilityCertificateInformation: {
            labourMobilityProvince: this.transferData.province,
            existingCertificationType:
              this.transferData.outOfProvinceCertification
                ?.transferringCertificate?.certificationType ?? undefined,
            certificateComparisonId:
              this.getCertificationComparisonIdBasedOnSelfAssessmentOutcome(),
          },
        },
      });
      this.userStore.isUnder19
        ? this.router.push({ name: "consent-required" })
        : this.router.push({ name: "application-requirements" });
    },
    getCertificationComparisonIdBasedOnSelfAssessmentOutcome():
      | string
      | undefined
      | null {
      //this function will leverage the logic from the selfAssessmentOutcome to generate the certificateComparisonId based on the options provided to registrant
      if (this.selfAssessmentOutcome.length === 0) {
        console.warn("No self-assessment outcome available.");
        return undefined;
      }
      //convert selfAssessmentOutcome to bcCertificate CertificationType
      let certificationType = "";

      if (
        this.selfAssessmentOutcome.includes("FiveYears") &&
        this.selfAssessmentOutcome.includes("Ite") &&
        this.selfAssessmentOutcome.includes("Sne")
      ) {
        certificationType = CertificationType.FiveYearCertificateITE_SNE;
      } else if (
        this.selfAssessmentOutcome.includes("Ite") ||
        this.selfAssessmentOutcome.includes("Sne")
      ) {
        certificationType = CertificationType.FiveYearCertificateITE_SNE;
      } else if (this.selfAssessmentOutcome.includes("FiveYears")) {
        certificationType = CertificationType.FiveYearCertificate;
      } else if (this.selfAssessmentOutcome.includes("OneYear")) {
        certificationType = CertificationType.OneYear;
      } else if (this.selfAssessmentOutcome.includes("EceAssistant")) {
        certificationType = CertificationType.Assistant;
      } else {
        console.warn(
          "Unsupported self-assessment outcome:",
          this.selfAssessmentOutcome,
        );
      }

      // find the certification comparison id based on the certification type
      return this.transferData.outOfProvinceCertification?.options?.find(
        (option) => option.bcCertificate === certificationType,
      )?.id;
    },
  },
  computed: {
    activeFiveYearCertification(): Components.Schemas.Certification | null {
      return this.certificationStore.activeEceFiveYearCertification;
    },
    activeFullyCertifiedFiveYearCertification(): boolean | undefined {
      if (!this.activeFiveYearCertification) {
        return false;
      }

      return (
        this.activeFiveYearCertification.levels?.some(
          (level) => level.type === "ITE",
        ) &&
        this.activeFiveYearCertification.levels?.some(
          (level) => level.type === "SNE",
        )
      );
    },
    remainingPostBasic(): "ITE" | "SNE" | "ITE_AND_SNE" | undefined {
      if (!this.activeFiveYearCertification) {
        return undefined;
      }

      if (
        this.activeFiveYearCertification.levels?.some(
          (level) => level.type === "ITE",
        ) &&
        this.activeFiveYearCertification.levels?.some(
          (level) => level.type === "SNE",
        )
      ) {
        console.warn("user is fully certified why do they need to do LM?");
        return undefined;
      } else if (
        !this.activeFiveYearCertification.levels?.some(
          (level) => level.type === "ITE",
        ) &&
        !this.activeFiveYearCertification.levels?.some(
          (level) => level.type === "SNE",
        )
      ) {
        return "ITE_AND_SNE";
      } else if (
        !this.activeFiveYearCertification.levels?.some(
          (level) => level.type === "ITE",
        )
      ) {
        return "ITE";
      } else if (
        !this.activeFiveYearCertification.levels?.some(
          (level) => level.type === "SNE",
        )
      ) {
        return "SNE";
      }
    },
    transferData(): EceTransferData {
      return {
        province: this.province,
        outOfProvinceCertification: this.outOfProvinceCertification,
      };
    },
    selfAssessmentOutcome(): Components.Schemas.CertificationType[] {
      if (this.highestCertificationType === CertificationType.Assistant) {
        return ["EceAssistant"];
      }
      if (
        this.highestCertificationType === CertificationType.OneYear ||
        (this.highestCertificationType ===
          CertificationType.FiveYearCertificate &&
          (this.has500HoursWorkExperience === "false" ||
            this.upgradeMyFiveYearCertification === false)) ||
        (this.highestCertificationType ===
          CertificationType.FiveYearCertificateITE_SNE &&
          (this.has500HoursWorkExperience === "false" ||
            this.upgradeMyFiveYearCertification === false))
      ) {
        return ["OneYear"];
      }
      if (
        this.highestCertificationType ===
          CertificationType.FiveYearCertificate &&
        (this.has500HoursWorkExperience === "true" ||
          (this.filteredCertificationOptions?.length === 1 &&
            !this.certificationStore.holdsEceFiveYearCertification))
      ) {
        return ["FiveYears"];
      }
      if (
        this.highestCertificationType ===
          CertificationType.FiveYearCertificateITE_SNE &&
        (this.has500HoursWorkExperience === "true" ||
          (this.filteredCertificationOptions?.length === 1 &&
            !this.certificationStore.holdsEceFiveYearCertification))
      ) {
        return ["FiveYears", "Ite", "Sne"];
      }
      //flow for those that have an active 5 year certificate and want to upgrade
      if (
        this.highestCertificationType ===
          CertificationType.FiveYearCertificateITE_SNE &&
        this.remainingPostBasic
      ) {
        if (this.remainingPostBasic === "ITE") {
          return ["Ite"];
        }
        if (this.remainingPostBasic === "SNE") {
          return ["Sne"];
        }
        if (this.remainingPostBasic === "ITE_AND_SNE") {
          return ["Ite", "Sne"];
        }
      }
      console.warn(
        "Unable to assign specific certificate type. This should not happen.",
      );
      return [];
    },
    showRequirementsButton() {
      return (
        this.highestCertificationType === CertificationType.Assistant ||
        this.highestCertificationType === CertificationType.OneYear ||
        this.has500HoursWorkExperience ||
        this.upgradeMyFiveYearCertification !== undefined ||
        this.filteredCertificationOptions?.length === 1
      );
    },
    labelForUpgradeOptionRadioFiveYearOptions(): string {
      switch (this.remainingPostBasic) {
        case "ITE":
          return "Apply to add Infant and Toddler Educator certification";

        case "SNE":
          return "Apply to add Special Needs Educator certification";

        case "ITE_AND_SNE":
          return "Apply to add Infant and Toddler Educator and Special Needs Educator certifications";
        default:
          console.warn(
            "Unexpected remainingPostBasic value:",
            this.remainingPostBasic,
          );
          return "unhandled value";
      }
    },
    textForUpgradeOptionsWithActiveFiveYear(): string {
      switch (this.remainingPostBasic) {
        case "ITE":
          return "Infant and Toddler Educator certification";

        case "SNE":
          return "Special Needs Educator certification";

        case "ITE_AND_SNE":
          return "Infant and Toddler Educator and Special Needs Educator certifications";
        default:
          console.warn(
            "Unexpected remainingPostBasic value:",
            this.remainingPostBasic,
          );
          return "unhandled value";
      }
    },
  },
  data: () => ({
    outOfProvinceCertificationTypesLoading: false,
    has500HoursWorkExperience: undefined,
    highestCertificationType: undefined as CertificationType | undefined,
    filteredCertificationOptions: undefined as
      | Components.Schemas.CertificationComparison[]
      | undefined,
    upgradeMyFiveYearCertification: undefined as boolean | undefined,
    outOfProvinceCertificationTypes:
      [] as Components.Schemas.ComparisonRecord[],
    Rules,
    province: undefined,
    outOfProvinceCertification: undefined,
    CertificationType,
  }),
};
</script>
