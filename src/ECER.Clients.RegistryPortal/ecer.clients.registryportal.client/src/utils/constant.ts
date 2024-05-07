import type { DropdownWrapper } from "@/types/form";
import type { RadioButtonWrapper } from "@/types/form";
import type { CheckBoxWrapper } from "@/types/form";
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

export const referenceRelationshipDropdown: DropdownWrapper<Components.Schemas.ReferenceRelationship>[] = [
  { title: "Co Worker", value: "CoWorker" },
  { title: "Other", value: "Other" },
  { title: "Parent Guardian of Child in Care", value: "ParentGuardianofChildinCare" },
  { title: "Supervisor", value: "Supervisor" },
  { title: "Teacher", value: "Teacher" },
];

export const childrenProgramTypeDropdown: DropdownWrapper<Components.Schemas.ChildrenProgramType>[] = [
  { title: "Child minding", value: "Childminding" },
  { title: "Family child care", value: "Familychildcare" },
  { title: "In home multi age child care", value: "InHomeMultiAgechildcare" },
  { title: "Multi age child care", value: "MultiAgechildcare" },
  { title: "Occasional child care", value: "Occasionalchildcare" },
  { title: "Preschool", value: "Preschool" },
  { title: "Other", value: "Other" },
];

export const workHoursTypeRadio: RadioButtonWrapper<Components.Schemas.WorkHoursType>[] = [
  { label: "Full Time", value: "FullTime" },
  { label: "Part Time", value: "PartTime" },
];

export const workReferenceRelationshipRadio: RadioButtonWrapper<Components.Schemas.ReferenceRelationship>[] = [
  { label: "Supervisor", value: "Supervisor" },
  { label: "Co-Worker", value: "CoWorker" },
  { label: "Other", value: "Other" },
];

export const childcareAgeRangesCheckBox: CheckBoxWrapper<Components.Schemas.ChildcareAgeRanges>[] = [
  { label: "0 to 12 Months", value: "From0to12Months" },
  { label: "12 to 24 Months", value: "From12to24Months" },
  { label: "25 to 30 Months", value: "From25to30Months" },
  { label: "31 to 36 Months", value: "From31to36Months" },
  { label: "Grade 1", value: "Grade1" },
  { label: "Preschool", value: "Preschool" },
];
