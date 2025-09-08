import { defineStore } from "pinia";

import type { Components } from "@/types/openapi";
import type { ApplicationStage, IcraEligibilityStage, ReferenceStage, RenewStage, Step, Wizard } from "@/types/wizard";
import { AddressType } from "@/utils/constant";

import { useOidcStore } from "./oidc";
import { useUserStore } from "./user";

export interface WizardData {
  [key: string]: any;
}

export interface WizardState {
  step: number;
  wizardConfig: Wizard;
  wizardData: WizardData;
  listComponentMode: "add" | "list";
}

export const useWizardStore = defineStore("wizard", {
  state: (): WizardState => ({
    step: 1,
    wizardData: {} as WizardData,
    wizardConfig: {} as Wizard,
    listComponentMode: "list",
  }),
  persist: true,
  getters: {
    steps(state): Step[] {
      return Object.values(state.wizardConfig.steps);
    },
    currentStep(state): Step {
      const currentStep = this.steps[state.step - 1];
      if (!currentStep) throw new Error("No current step found");
      return currentStep;
    },
    currentStepId(state): string {
      const stepId = this.steps[state.step - 1]?.id;
      if (!stepId) throw new Error("No current step id found");
      return stepId;
    },
    currentStepStage(state): ApplicationStage | ReferenceStage | RenewStage | IcraEligibilityStage {
      const stage = this.steps[state.step - 1]?.stage;
      if (!stage) throw new Error("No current step stage found");
      return stage;
    },
    hasStep() {
      return (step: ApplicationStage | ReferenceStage | RenewStage | IcraEligibilityStage) => {
        return this.steps.some((s) => s.stage === step);
      };
    },
  },
  actions: {
    async initializeWizard(wizard: Wizard, draftApplication: Components.Schemas.DraftApplication): Promise<void> {
      const userStore = useUserStore();
      const oidcStore = useOidcStore();

      const oidcUserInfo = await oidcStore.oidcUserInfo();
      const oidcAddress = await oidcStore.oidcAddress();

      this.wizardConfig = wizard;

      // set step to the index of steps where the stage matches the draft application stage
      this.step = Object.values(wizard.steps).findIndex((step) => step.stage === draftApplication.stage) + 1;

      const transcriptsDict = {} as { [id: string]: Components.Schemas.Transcript };
      const workReferencesDict = {} as { [id: string]: Components.Schemas.WorkExperienceReference };

      // Convert array to dictionary with keys as "1", "2", ..., "n"
      if (draftApplication.transcripts) {
        draftApplication.transcripts.forEach((transcript, index) => {
          const id = (index + 1).toString();
          transcriptsDict[id] = transcript;
        });
      }
      if (draftApplication.workExperienceReferences) {
        draftApplication.workExperienceReferences.forEach((reference, index) => {
          const id = (index + 1).toString();
          workReferencesDict[id] = reference;
        });
      }

      this.wizardData = {
        // Contact Information step data
        ...(wizard?.steps?.profile?.form?.inputs?.legalLastName?.id && {
          [wizard.steps.profile.form.inputs.legalLastName.id]: userStore.userProfile?.lastName || oidcUserInfo?.lastName,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.legalFirstName?.id && {
          [wizard.steps.profile.form.inputs.legalFirstName.id]: userStore.userProfile?.firstName || oidcUserInfo?.firstName,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.legalMiddleName?.id && {
          [wizard.steps.profile.form.inputs.legalMiddleName.id]: userStore.userProfile?.middleName,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.preferredName?.id && {
          [wizard.steps.profile.form.inputs.preferredName.id]: userStore.userProfile?.preferredName,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.dateOfBirth?.id && {
          [wizard.steps.profile.form.inputs.dateOfBirth.id]: userStore.userProfile?.dateOfBirth || oidcUserInfo?.dateOfBirth,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.addresses?.id && {
          [wizard.steps.profile.form.inputs.addresses.id]: {
            [AddressType.RESIDENTIAL]: userStore.userProfile?.residentialAddress || oidcAddress,
            [AddressType.MAILING]: userStore.userProfile?.mailingAddress || oidcAddress,
          },
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.primaryContactNumber?.id && {
          [wizard.steps.profile.form.inputs.primaryContactNumber.id]: userStore.userProfile?.phone || oidcUserInfo?.phone,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.alternateContactNumber?.id && {
          [wizard.steps.profile.form.inputs.alternateContactNumber.id]: userStore.userProfile?.alternateContactPhone,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.email?.id && {
          [wizard.steps.profile.form.inputs.email.id]: userStore.userProfile?.email || oidcUserInfo?.email,
        }),

        // Certificate Information step data
        ...(wizard.steps?.certificateInformation?.form?.inputs?.certificateInformation?.id && {
          [wizard.steps.certificateInformation.form.inputs.certificateInformation.id]: draftApplication?.labourMobilityCertificateInformation,
        }),

        // Education step data
        ...(wizard.steps?.education?.form?.inputs?.educationList?.id && { [wizard.steps?.education?.form?.inputs?.educationList?.id]: transcriptsDict }),

        // one year explanation letter
        ...(wizard.steps?.oneYearRenewalExplanation?.form?.inputs?.oneYearRenewalExplanation?.id && {
          [wizard.steps?.oneYearRenewalExplanation?.form?.inputs?.oneYearRenewalExplanation?.id]: draftApplication?.oneYearRenewalExplanationChoice || null,
        }),
        ...(wizard.steps?.oneYearRenewalExplanation?.form?.inputs?.renewalExplanationOther?.id && {
          [wizard.steps?.oneYearRenewalExplanation?.form?.inputs?.renewalExplanationOther?.id]: draftApplication?.renewalExplanationOther || "",
        }),

        // five year explanation letter
        ...(wizard.steps?.fiveYearRenewalExplanation?.form?.inputs?.fiveYearRenewalExplanation?.id && {
          [wizard.steps?.fiveYearRenewalExplanation?.form?.inputs?.fiveYearRenewalExplanation?.id]: draftApplication?.fiveYearRenewalExplanationChoice || null,
        }),
        ...(wizard.steps?.fiveYearRenewalExplanation?.form?.inputs?.renewalExplanationOther?.id && {
          [wizard.steps?.fiveYearRenewalExplanation?.form?.inputs?.renewalExplanationOther?.id]: draftApplication?.renewalExplanationOther || "",
        }),

        // Character References step data
        ...(wizard.steps?.characterReferences?.form?.inputs?.characterReferences?.id && {
          [wizard.steps?.characterReferences?.form?.inputs?.characterReferences?.id]: draftApplication?.characterReferences?.[0] || [],
        }),

        // Professional Development
        ...(wizard.steps?.professionalDevelopments?.form?.inputs?.professionalDevelopments?.id && {
          [wizard.steps?.professionalDevelopments?.form?.inputs?.professionalDevelopments?.id]: draftApplication?.professionalDevelopments || [],
        }),

        // wizard data may not have referenceList depending on the certification type. So we need to hardcode the value.
        referenceList: workReferencesDict,
      };
    },
    async initializeWizardForIcraEligibility(wizard: Wizard, draftIcraEligibility: Components.Schemas.ICRAEligibility) {
      const userStore = useUserStore();
      const oidcStore = useOidcStore();

      const oidcUserInfo = await oidcStore.oidcUserInfo();
      const oidcAddress = await oidcStore.oidcAddress();

      this.$reset();
      this.wizardConfig = wizard;

      this.setWizardData({
        // Contact Information step data
        ...(wizard?.steps?.profile?.form?.inputs?.legalLastName?.id && {
          [wizard.steps.profile.form.inputs.legalLastName.id]: userStore.userProfile?.lastName || oidcUserInfo?.lastName,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.legalFirstName?.id && {
          [wizard.steps.profile.form.inputs.legalFirstName.id]: userStore.userProfile?.firstName || oidcUserInfo?.firstName,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.legalMiddleName?.id && {
          [wizard.steps.profile.form.inputs.legalMiddleName.id]: userStore.userProfile?.middleName,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.preferredName?.id && {
          [wizard.steps.profile.form.inputs.preferredName.id]: userStore.userProfile?.preferredName,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.dateOfBirth?.id && {
          [wizard.steps.profile.form.inputs.dateOfBirth.id]: userStore.userProfile?.dateOfBirth || oidcUserInfo?.dateOfBirth,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.addresses?.id && {
          [wizard.steps.profile.form.inputs.addresses.id]: {
            [AddressType.RESIDENTIAL]: userStore.userProfile?.residentialAddress || oidcAddress,
            [AddressType.MAILING]: userStore.userProfile?.mailingAddress || oidcAddress,
          },
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.primaryContactNumber?.id && {
          [wizard.steps.profile.form.inputs.primaryContactNumber.id]: userStore.userProfile?.phone || oidcUserInfo?.phone,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.alternateContactNumber?.id && {
          [wizard.steps.profile.form.inputs.alternateContactNumber.id]: userStore.userProfile?.alternateContactPhone,
        }),
        ...(wizard?.steps?.profile?.form?.inputs?.email?.id && {
          [wizard.steps.profile.form.inputs.email.id]: userStore.userProfile?.email || oidcUserInfo?.email,
        }),

        // TODO: Add other steps data
      });
    },
    initializeWizardForCharacterReference(wizard: Wizard, portalInvitation: Components.Schemas.PortalInvitation) {
      this.$reset();
      this.wizardConfig = wizard;

      this.setWizardData({
        applicantFirstName: portalInvitation.applicantFirstName,
        applicantLastName: portalInvitation.applicantLastName,
        referenceFirstName: portalInvitation.referenceFirstName,
        referenceLastName: portalInvitation.referenceLastName,
        inviteType: portalInvitation.inviteType,
        certificationTypes: portalInvitation.certificationTypes,
        ...(wizard?.steps?.review?.form?.inputs?.confirmProvidedInformationIsRight?.id && {
          [wizard.steps.review.form.inputs.confirmProvidedInformationIsRight.id]: false,
        }),
        ...(wizard?.steps?.contactInformation?.form?.inputs?.referenceContactInformation?.id && {
          [wizard.steps.contactInformation.form.inputs.referenceContactInformation.id]: {} as Components.Schemas.ReferenceContactInformation,
        }),
        ...(wizard?.steps?.referenceEvaluation?.form?.inputs?.characterReferenceEvaluation?.id && {
          [wizard.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]: {} as Components.Schemas.CharacterReferenceEvaluation,
        }),
        ...(wizard?.steps?.review?.form?.inputs?.recaptchaToken?.id && {
          [wizard.steps.review.form.inputs.recaptchaToken.id]: "",
        }),
      });
    },
    initializeWizardForWorkExReference(wizard: Wizard, portalInvitation: Components.Schemas.PortalInvitation) {
      this.$reset();
      this.wizardConfig = wizard;

      this.setWizardData({
        applicantFirstName: portalInvitation.applicantFirstName,
        applicantLastName: portalInvitation.applicantLastName,
        referenceFirstName: portalInvitation.referenceFirstName,
        referenceLastName: portalInvitation.referenceLastName,
        inviteType: portalInvitation.inviteType,
        certificationTypes: portalInvitation.certificationTypes,
        workExperienceReferenceHours: portalInvitation.workExperienceReferenceHours,
        workExperienceType: portalInvitation.workExperienceType,
        ...(wizard?.steps?.review?.form?.inputs?.confirmProvidedInformationIsRight?.id && {
          [wizard.steps.review.form.inputs.confirmProvidedInformationIsRight.id]: false,
        }),
        ...(wizard?.steps?.contactInformation?.form?.inputs?.referenceContactInformation?.id && {
          [wizard.steps.contactInformation.form.inputs.referenceContactInformation.id]: {} as Components.Schemas.ReferenceContactInformation,
        }),
        ...(wizard?.steps?.workExperienceEvaluation?.form?.inputs?.workExperienceEvaluation?.id && {
          [wizard.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id]: {} as Components.Schemas.WorkExperienceReferenceDetails,
        }),
        ...(wizard?.steps?.assessment?.form?.inputs?.workExperienceAssessment?.id && {
          [wizard.steps.assessment.form.inputs.workExperienceAssessment.id]: {} as Components.Schemas.WorkExperienceReferenceCompetenciesAssessment,
        }),
        ...(wizard?.steps?.review?.form?.inputs?.recaptchaToken?.id && {
          [wizard.steps.review.form.inputs.recaptchaToken.id]: "",
        }),
      });
    },
    initializeWizardFor400HoursWorkExReference(wizard: Wizard, portalInvitation: Components.Schemas.PortalInvitation) {
      this.$reset();
      this.wizardConfig = wizard;

      this.setWizardData({
        applicantFirstName: portalInvitation.applicantFirstName,
        applicantLastName: portalInvitation.applicantLastName,
        referenceFirstName: portalInvitation.referenceFirstName,
        referenceLastName: portalInvitation.referenceLastName,
        inviteType: portalInvitation.inviteType,
        certificationTypes: portalInvitation.certificationTypes,
        workExperienceReferenceHours: portalInvitation.workExperienceReferenceHours,
        workExperienceType: portalInvitation.workExperienceType,
        latestCertification: portalInvitation.latestCertification,
        ...(wizard?.steps?.review?.form?.inputs?.confirmProvidedInformationIsRight?.id && {
          [wizard.steps.review.form.inputs.confirmProvidedInformationIsRight.id]: false,
        }),
        ...(wizard?.steps?.contactInformation?.form?.inputs?.referenceContactInformation?.id && {
          [wizard.steps.contactInformation.form.inputs.referenceContactInformation.id]: {} as Components.Schemas.ReferenceContactInformation,
        }),
        ...(wizard?.steps?.workExperience400HoursEvaluation?.form?.inputs?.workExperience400HoursEvaluation?.id && {
          [wizard.steps.workExperience400HoursEvaluation.form.inputs.workExperience400HoursEvaluation.id]:
            {} as Components.Schemas.WorkExperienceReferenceDetails,
        }),
        ...(wizard?.steps?.review?.form?.inputs?.recaptchaToken?.id && {
          [wizard.steps.review.form.inputs.recaptchaToken.id]: "",
        }),
      });
    },
    setWizardData(wizardData: WizardData): void {
      this.wizardData = { ...this.wizardData, ...wizardData };
    },
    setCurrentStep(stage: ApplicationStage | ReferenceStage | IcraEligibilityStage): void {
      const item = Object.values(this.wizardConfig.steps).findIndex((step) => step.stage === stage) + 1;
      this.step = item;
      window.scrollTo({
        top: 0,
        behavior: "smooth",
      });
    },
    incrementStep(): void {
      if (this.step < Object.keys(this.wizardConfig.steps).length) {
        this.step += 1;
        window.scrollTo(0, 0);
      }
    },
    decrementStep(): void {
      if (this.step > 1) {
        this.step -= 1;
        window.scrollTo(0, 0);
      }
    },
    setStep(step: number): void {
      this.step = step;
      window.scrollTo(0, 0);
    },
  },
});
