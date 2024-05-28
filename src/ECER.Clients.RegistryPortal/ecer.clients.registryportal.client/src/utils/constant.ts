import type { CheckBoxWrapper, DropdownWrapper, RadioButtonWrapper } from "@/types/form";
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
  WORK_EXPERIENCE = "WorkExperienceReference",
  CHARACTER = "CharacterReference",
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

export const referenceRelationshipDropdown: DropdownWrapper<Components.Schemas.ReferenceRelationship>[] = [
  { title: "Supervisor", value: "Supervisor" },
  { title: "Co Worker", value: "CoWorker" },
  { title: "Teacher", value: "Teacher" },
  { title: "Parent or Guardian of Child in Care", value: "ParentGuardianofChildinCare" },
  { title: "Other", value: "Other" },
];

export const childrenProgramTypeDropdown: DropdownWrapper<Components.Schemas.ChildrenProgramType>[] = [
  { title: "Group child care", value: "Groupchildcare" },
  { title: "Preschool", value: "Preschool" },
  { title: "Family child care", value: "Familychildcare" },
  { title: "Occasional child care", value: "Occasionalchildcare" },
  { title: "Multi-Age child care", value: "MultiAgechildcare" },
  { title: "In-Home Multi-Age child care", value: "InHomeMultiAgechildcare" },
  { title: "Child-minding", value: "Childminding" },
  { title: "Other", value: "Other" },
];

export const workHoursTypeRadio: RadioButtonWrapper<Components.Schemas.WorkHoursType>[] = [
  { label: "Full time", value: "FullTime" },
  { label: "Part time", value: "PartTime" },
];

export const workReferenceRelationshipRadio: RadioButtonWrapper<Components.Schemas.ReferenceRelationship>[] = [
  { label: "Supervisor", value: "Supervisor" },
  { label: "Co-worker", value: "CoWorker" },
  { label: "Other", value: "Other" },
];

export const likertScaleRadio: RadioButtonWrapper<Components.Schemas.LikertScale>[] = [
  { label: "Yes", value: "Yes" },
  { label: "No", value: "No" },
];

export const childcareAgeRangesCheckBox: CheckBoxWrapper<Components.Schemas.ChildcareAgeRanges>[] = [
  { label: "0 to 12 Months", value: "From0to12Months" },
  { label: "12 to 24 Months", value: "From12to24Months" },
  { label: "25 to 30 Months", value: "From25to30Months" },
  { label: "31 to 36 Months", value: "From31to36Months" },
  { label: "In preschool", value: "Preschool" },
  { label: "In grade 1", value: "Grade1" },
];

export const lengthOfAcquaintanceDropdown: DropdownWrapper<Components.Schemas.ReferenceKnownTime>[] = [
  { title: "less than 6 months", value: "Lessthan6months" },
  { title: "6 months to 1 year", value: "From6monthsto1year" },
  { title: "1 to 2 years", value: "From1to2years" },
  { title: "2 to 5 years", value: "From2to5years" },
  { title: "5 or more years", value: "Morethan5years" },
];
