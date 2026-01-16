import type {
  CheckBoxWrapper,
  DropdownWrapper,
  RadioButtonWrapper,
} from "@/types/form";
import type { Components } from "@/types/openapi";

export enum ApplicationState {
  IN_PROGRESS = 0,
  COMPLETED = 1,
}

//routes for the frontend referencing the route name
export enum RouteName {
  IN_PROGRESS = "in-progress",
  COMPLETED = "completed",
}

export enum AlertNotificationType {
  ERROR = "error",
  WARN = "warning",
  SUCCESS = "success",
  INFO = "primary",
}

export enum AddressType {
  RESIDENTIAL = "residential",
  MAILING = "mailing",
}

export enum PortalInviteType {
  WORK_EXPERIENCE = "WorkExperienceReferenceforApplication",
  CHARACTER = "CharacterReference",
  PROGRAM_REPRESENTATIVE = "PSIProgramRepresentative",
  ICRA_WORK_EXPERIENCE = "WorkExperienceReferenceforICRA",
}

export enum WorkExperienceType {
  IS_400_Hours = "Is400Hours",
  IS_500_Hours = "Is500Hours",
  ICRA = "ICRA",
}

export enum ProvinceTerritoryType {
  BC = "British Columbia",
  OTHER = "Other",
}

export enum CertificationType {
  ITE = "Ite",
  SNE = "Sne",
  FIVE_YEAR = "FiveYears",
  ONE_YEAR = "OneYear",
  ECE_ASSISTANT = "EceAssistant",
}

export const referenceRelationshipDropdown: DropdownWrapper<Components.Schemas.ReferenceRelationship>[] =
  [
    { title: "Supervisor", value: "Supervisor" },
    { title: "Co Worker", value: "CoWorker" },
    { title: "Teacher", value: "Teacher" },
    {
      title: "Parent or Guardian of Child in Care",
      value: "ParentGuardianofChildinCare",
    },
    { title: "Other", value: "Other" },
  ];

export const childrenProgramTypeDropdown: DropdownWrapper<Components.Schemas.ChildrenProgramType>[] =
  [
    { title: "Group child care", value: "Groupchildcare" },
    { title: "Preschool", value: "Preschool" },
    { title: "Family child care", value: "Familychildcare" },
    { title: "Occasional child care", value: "Occasionalchildcare" },
    { title: "Multi-Age child care", value: "MultiAgechildcare" },
    { title: "In-Home Multi-Age child care", value: "InHomeMultiAgechildcare" },
    { title: "Child-minding", value: "Childminding" },
    { title: "Other", value: "Other" },
  ];

export const workHoursTypeRadio: RadioButtonWrapper<Components.Schemas.WorkHoursType>[] =
  [
    { label: "Full time", value: "FullTime" },
    { label: "Part time", value: "PartTime" },
  ];

export const workReferenceRelationshipRadio: RadioButtonWrapper<Components.Schemas.ReferenceRelationship>[] =
  [
    { label: "Supervisor", value: "Supervisor" },
    { label: "Co-worker", value: "CoWorker" },
    { label: "Other", value: "Other" },
  ];

export const workReference400HoursRelationshipRadio: RadioButtonWrapper<Components.Schemas.ReferenceRelationship>[] =
  [
    { label: "Supervisor", value: "Supervisor" },
    { label: "Co-worker", value: "CoWorker" },
    {
      label: "Parent or guardian of child they cared for",
      value: "ParentGuardianofChildinCare",
    },
  ];

export const likertScaleRadio: RadioButtonWrapper<Components.Schemas.LikertScale>[] =
  [
    { label: "Yes", value: "Yes" },
    { label: "No", value: "No" },
  ];

export const childcareAgeRangesCheckBox: CheckBoxWrapper<Components.Schemas.ChildcareAgeRanges>[] =
  [
    { label: "0 to 36 Months", value: "_036months" },
    { label: "3 to 5 Years", value: "_35years" },
    { label: "6 to 8 Years", value: "_68years" },
  ];

export const lengthOfAcquaintanceDropdown: DropdownWrapper<Components.Schemas.ReferenceKnownTime>[] =
  [
    { title: "less than 6 months", value: "Lessthan6months" },
    { title: "6 months to 1 year", value: "From6monthsto1year" },
    { title: "1 to 2 years", value: "From1to2years" },
    { title: "2 to 5 years", value: "From2to5years" },
    { title: "5 or more years", value: "Morethan5years" },
  ];

export const fiveYearRenewalInformationRadio: RadioButtonWrapper<Components.Schemas.FiveYearRenewalExplanations>[] =
  [
    {
      label: "I left the childcare field for personal reasons",
      value: "Ileftthechildcarefieldforpersonalreasons",
    },
    {
      label:
        "I was unable to complete the required hours of professional development",
      value: "Iwasunabletocompletetherequiredhoursofprofessionaldevelopment",
    },
    {
      label:
        "I was unable to find employment in the childcare field in my community",
      value: "Iwasunabletofindemploymentinthechildcarefieldinmycommunity",
    },
    {
      label:
        "My employment does not require certification as an ECE. For example nanny, teacher, college instructor, administrator, etc.",
      value:
        "MyemploymentdiddoesnotrequirecertificationasanECEforexamplenannyteachercollegeinstructoradministratoretc",
    },
    { label: "Other", value: "Other" },
  ];

export const renewalInformationRadio: RadioButtonWrapper<Components.Schemas.OneYearRenewalexplanations>[] =
  [
    {
      label: "I live and work in a community without other certified ECEs",
      value: "IliveandworkinacommunitywithoutothercertifiedECEs",
    },
    {
      label:
        "I was unable to find employment in the childcare field to complete the required number of hours",
      value:
        "Iwasunabletofindemploymentinthechildcarefieldtocompletetherequirednumberofhours",
    },
    {
      label:
        "I was unable to work due to the status of my visa or was unable to enter the country as expected",
      value:
        "Iwasunabletoworkduetothestatusofmyvisaorwasunabletoenterthecountryasexpected",
    },
    {
      label: "I was unable to work in the childcare field for personal reasons",
      value: "Iwasunabletoworkinthechildcarefieldforpersonalreasons",
    },
    { label: "Other", value: "Other" },
  ];

export const educationRecognitionRadio: RadioButtonWrapper<Components.Schemas.EducationRecognition>[] =
  [
    { label: "Recognized", value: "Recognized" },
    { label: "Not Recognized", value: "NotRecognized" },
  ];

export const educationOriginRadio: RadioButtonWrapper<Components.Schemas.EducationOrigin>[] =
  [
    { label: "In British Columbia", value: "InsideBC" },
    { label: "In another province or territory in Canada", value: "OutsideBC" },
    { label: "Outside of Canada", value: "OutsideofCanada" },
  ];
