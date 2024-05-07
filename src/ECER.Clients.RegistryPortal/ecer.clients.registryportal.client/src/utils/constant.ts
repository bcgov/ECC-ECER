import type { DropdownWrapper } from "@/types/form";
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
  { title: "Supervisor", value: "Supervisor" },
  { title: "Co Worker", value: "CoWorker" },
  { title: "Teacher", value: "Teacher" },
  { title: "Parent Guardian of Child in Care", value: "ParentGuardianofChildinCare" },
  { title: "Other", value: "Other" },
];
