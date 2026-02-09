<template>
  <v-container>
    <Breadcrumb />
    <div class="d-flex flex-column ga-3">
      <h1 class="mt-5">Professional development</h1>
      <v-row>
        <v-col>
          <h3>Add course or workshop</h3>
          <br />
          <p>The course or workshop must:</p>
          <br />
          <ul class="ml-10">
            <li>Be relevant to the field of early childhood education</li>
            <li
              v-if="fromCertificate && fromCertificate.statusCode === 'Active'"
            >
              {{
                `Have been completed within the term of your current certificate (Between ${formatDate(fromCertificate?.effectiveDate || "", "LLLL d, yyyy")} to ${formatDate(fromCertificate?.expiryDate || "", "LLLL d, yyyy")})`
              }}
            </li>
            <li
              v-else-if="
                fromCertificate && fromCertificate.statusCode === 'Expired'
              "
            >
              Have been completed within the last 5 years
            </li>
          </ul>
        </v-col>
      </v-row>
      <v-form ref="professionalDevelopmentForm">
        <v-row>
          <v-col>
            <h3 class="mt-5">Course or workshop information</h3>
          </v-col>
        </v-row>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceTextField
              id="txtCourseName"
              v-model="professionalDevelopment.courseName"
              label="Name of course or workshop"
              maxlength="100"
              :rules="[Rules.required('Enter your course or workshop name')]"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceTextField
              id="txtOrganizationName"
              v-model="professionalDevelopment.organizationName"
              label="Name of the organization who offered this training"
              tooltip-text="Provide the name of the school, organization, or instructor that offered the training or issued the certificate. 
They should be able to confirm you completed the course or workshop."
              maxlength="300"
              :rules="[
                Rules.required(
                  'Enter the name of the organization that offered this training',
                ),
              ]"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceTextField
              id="txtCourseOrWorkshopLink"
              v-model="professionalDevelopment.courseorWorkshopLink"
              variant="outlined"
              label="Website with description of course or workshop (optional)"
              maxlength="500"
              :rules="[Rules.website()]"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col cmd="8" lg="6" xl="4">
            <EceTextField
              id="txtInstructorName"
              v-model="professionalDevelopment.instructorName"
              label="Name of instructor or facilitator (if known)"
              maxlength="100"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <h3>Dates and duration</h3>
          </v-col>
        </v-row>
        <v-row>
          <v-col sm="6" md="4" lg="2">
            <EceDateInput
              id="txtCourseStartDate"
              ref="startDateInput"
              v-model="professionalDevelopment.startDate"
              :rules="[
                Rules.required(
                  'Enter the start date of your course or workshop',
                ),
                Rules.futureDateNotAllowedRule(),
                Rules.conditionalWrapper(
                  !!(
                    fromCertificate && fromCertificate.statusCode === 'Active'
                  ),
                  Rules.dateBetweenRule(
                    fromCertificate?.effectiveDate || '',
                    fromCertificate?.expiryDate || '',
                    'The start date of your course or workshop must be within the term of your current certificate',
                  ),
                ),
                Rules.conditionalWrapper(
                  !!(
                    fromCertificate && fromCertificate.statusCode === 'Expired'
                  ),
                  Rules.dateRuleRange(
                    applicationStore.draftApplication.createdOn || '',
                    5,
                    'The start date of your course or workshop must be within the last five years',
                  ),
                ),
              ]"
              label="Start date"
              :max="today"
              @input="validateDates"
            ></EceDateInput>
          </v-col>
        </v-row>
        <v-row>
          <v-col sm="6" md="4" lg="2">
            <EceDateInput
              id="txtCourseEndDate"
              ref="endDateInput"
              v-model="professionalDevelopment.endDate"
              :rules="[
                Rules.required('Enter the end date of your course or workshop'),
                Rules.futureDateNotAllowedRule(),
                Rules.dateBeforeRule(professionalDevelopment.startDate || ''),
                Rules.conditionalWrapper(
                  !!(
                    fromCertificate && fromCertificate.statusCode === 'Active'
                  ),
                  Rules.dateBetweenRule(
                    fromCertificate?.effectiveDate || '',
                    fromCertificate?.expiryDate || '',
                    'The end date of your course or workshop must be within the term of your current certificate',
                  ),
                ),
                Rules.conditionalWrapper(
                  !!(
                    fromCertificate && fromCertificate.statusCode === 'Expired'
                  ),
                  Rules.dateRuleRange(
                    applicationStore.draftApplication.createdOn || '',
                    5,
                    'The end date of your course or workshop must be within the last five years',
                  ),
                ),
              ]"
              label="End date"
              :max="today"
              @input="validateDates"
            ></EceDateInput>
          </v-col>
        </v-row>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceTextField
              id="txtNumberOfHours"
              v-model="professionalDevelopment.numberOfHours"
              label="How many hours was this course or workshop?"
              :rules="[
                Rules.required('Enter your course or workshop hours'),
                Rules.numberToDecimalPlaces(),
              ]"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <h3>Proof of completion</h3>
            <br />
            <p>
              We may need to verify you took this course. You'll need to provide
              at least one option below
            </p>
            <br />
            <p>What can you provide? Choose all that apply</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-checkbox
              id="chkInstructorPhoneNumber"
              v-model="professionalDevelopment.selection"
              label="Phone number of host organization, instructor or facilitator"
              :hide-details="true"
              density="compact"
              value="phone"
              :rules="[Rules.atLeastOneOptionRequired('')]"
              @input="selectionChanged"
            ></v-checkbox>
            <v-row v-if="showPhoneNumberInput" class="ml-4 mb-2">
              <v-col md="8" lg="6" xl="4">
                <EceTextField
                  id="txtOrganizationPhoneNumber"
                  v-model="
                    professionalDevelopment.organizationContactInformation
                  "
                  label="Phone number"
                  :rules="[
                    Rules.required(
                      'Enter the phone number for your course or workshop contact',
                    ),
                    Rules.phoneNumber(
                      'Enter your reference\'s valid phone number',
                    ),
                  ]"
                ></EceTextField>
              </v-col>
            </v-row>
            <v-checkbox
              id="chkInstructorEmailAddress"
              v-model="professionalDevelopment.selection"
              label="Email address of host organization, instructor or facilitator"
              :hide-details="true"
              value="email"
              density="compact"
              :rules="[Rules.atLeastOneOptionRequired('')]"
              @input="selectionChanged"
            ></v-checkbox>
            <v-row v-if="showEmailInput" class="ml-4 mb-2">
              <v-col md="8" lg="6" xl="4">
                <EceTextField
                  id="txtOrganizationEmailAddress"
                  v-model="professionalDevelopment.organizationEmailAddress"
                  label="Email address"
                  :rules="[
                    Rules.required(
                      'Enter the email address of your course or workshop contact',
                    ),
                    Rules.email(),
                  ]"
                ></EceTextField>
              </v-col>
            </v-row>
            <v-checkbox
              id="chkCourseCompletionDocument"
              v-model="professionalDevelopment.selection"
              label="I can't provide contact information"
              hide-details="auto"
              value="file"
              density="compact"
              :rules="[Rules.atLeastOneOptionRequired()]"
              @input="selectionChanged"
            ></v-checkbox>
          </v-col>
        </v-row>
        <v-row v-if="showFileInput" class="ml-4">
          <v-col>
            <FileUploader
              :allow-multiple-files="false"
              :show-add-file-button="true"
              @update:files="handleFileUpdate"
            />
          </v-col>
        </v-row>
      </v-form>
    </div>
    <v-row class="mt-6">
      <v-col class="d-flex flex-row ga-3 flex-wrap">
        <v-btn
          id="btnAddProfessionalDevelopment"
          size="large"
          color="primary"
          :loading="
            loadingStore.isLoading(
              'application_professionaldevelopment_add_post',
            )
          "
          @click="handleAddProfessionalDevelopment"
        >
          Save course or workshop
        </v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { defineComponent } from "vue";
import { useRouter } from "vue-router";
import type { VForm, VInput } from "vuetify/components";
import EceTextField from "@/components/inputs/EceTextField.vue";
import EceDateInput from "@/components/inputs/EceDateInput.vue";
import { getApplicationStatus } from "@/api/application";
import { addProfessionalDevelopment } from "@/api/application";
import FileUploader from "@/components/FileUploader.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useCertificationStore } from "@/store/certification";
import { useLoadingStore } from "@/store/loading";
import type { Components } from "@/types/openapi";
import { formatDate } from "@/utils/format";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";
import Breadcrumb from "@/components/Breadcrumb.vue";
import type { FileItem } from "./UploadFileItem.vue";

interface ProfessionalDevelopmentData {
  selection: ("phone" | "email" | "file")[];
  areAttachedFilesValid: boolean;
  isFileUploadInProgress: boolean;
}

export default defineComponent({
  name: "AddProfessionalDevelopment",
  components: { FileUploader, EceTextField, EceDateInput, Breadcrumb },
  props: {
    applicationId: {
      type: String,
      required: true,
    },
  },
  setup: async (props) => {
    const applicationStore = useApplicationStore();
    const alertStore = useAlertStore();

    const loadingStore = useLoadingStore();
    const router = useRouter();
    const applicationStatus = (
      await getApplicationStatus(props.applicationId.toString())
    )?.data;
    const certificationStore = useCertificationStore();

    return {
      applicationStore,
      alertStore,
      loadingStore,
      router,
      applicationStatus,
      certificationStore,
      formatDate,
    };
  },
  data() {
    const professionalDevelopment: ProfessionalDevelopmentData &
      Components.Schemas.ProfessionalDevelopment = {
      selection: [],
      areAttachedFilesValid: true,
      isFileUploadInProgress: false,
    };

    return { professionalDevelopment, Rules };
  },
  computed: {
    applicationType() {
      return this.applicationStatus?.applicationType!;
    },
    today() {
      return formatDate(DateTime.now().toString());
    },
    showPhoneNumberInput() {
      return this.professionalDevelopment.selection.includes("phone");
    },
    showEmailInput() {
      return this.professionalDevelopment.selection.includes("email");
    },
    showFileInput() {
      return this.professionalDevelopment.selection.includes("file");
    },
    fromCertificate() {
      return this.certificationStore.getCertificationById(
        this.applicationStore.draftApplication.fromCertificate,
      );
    },
  },

  methods: {
    async handleAddProfessionalDevelopment() {
      // Validate the form
      const { valid } = await (
        this.$refs.professionalDevelopmentForm as VForm
      ).validate();
      if (this.professionalDevelopment.isFileUploadInProgress) {
        this.alertStore.setFailureAlert(
          "Uploading files in progress. Please wait until files are uploaded and try again.",
        );
      } else if (valid) {
        const { error } = await addProfessionalDevelopment(
          { application_id: this.applicationId },
          this.professionalDevelopment,
        );
        if (error) {
          this.alertStore.setFailureAlert(
            "Sorry, something went wrong and your changes could not be saved. Try again later.",
          );
        } else {
          this.alertStore.setSuccessAlert(
            "Professional development added sucessfully!",
          );
          this.router.push({
            name: "manageProfessionalDevelopment",
            params: { applicationId: this.applicationId },
          });
        }
      } else {
        this.alertStore.setFailureAlert(
          "You must enter all required fields in the valid format to continue.",
        );
      }
    },
    isNumber,
    selectionChanged() {
      if (!this.professionalDevelopment.selection.includes("phone")) {
        this.professionalDevelopment.organizationContactInformation = "";
      }

      if (!this.professionalDevelopment.selection.includes("email")) {
        this.professionalDevelopment.organizationEmailAddress = "";
      }

      if (!this.professionalDevelopment.selection.includes("file")) {
        //if user unchecks file we remove all files
        this.professionalDevelopment.newFiles = [];
      }
    },
    validateDates() {
      if (
        this.professionalDevelopment.startDate &&
        this.professionalDevelopment.endDate
      ) {
        (this.$refs.startDateInput as VInput).validate();
        (this.$refs.endDateInput as VInput).validate();
      }
    },
    handleFileUpdate(filesArray: FileItem[]) {
      this.professionalDevelopment.areAttachedFilesValid = true;
      this.professionalDevelopment.isFileUploadInProgress = false;
      this.professionalDevelopment.newFiles = []; // Reset attachments
      if (filesArray && filesArray.length > 0) {
        for (let i = 0; i < filesArray.length; i++) {
          const file = filesArray[i];

          if (!file) {
            console.warn("file is undefined");
            continue;
          }

          // Check for file errors
          if (file.fileErrors && file.fileErrors.length > 0) {
            this.professionalDevelopment.areAttachedFilesValid = false;
          }

          // Check if file is still uploading
          else if (file.progress < 101) {
            this.professionalDevelopment.isFileUploadInProgress = true;
          }

          // If file is valid and fully uploaded, add to attachments
          if (
            this.professionalDevelopment.areAttachedFilesValid &&
            !this.professionalDevelopment.isFileUploadInProgress
          ) {
            this.professionalDevelopment.newFiles?.push(file.fileId);
          }
        }
      }
    },
  },
});
</script>
