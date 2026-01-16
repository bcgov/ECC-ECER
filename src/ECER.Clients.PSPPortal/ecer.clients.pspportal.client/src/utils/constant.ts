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
  PSIProgramRepresentative = "PSIProgramRepresentative",
}

export enum WorkExperienceType {
  IS_400_Hours = "Is400Hours",
  IS_500_Hours = "Is500Hours",
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

export enum ProgramType {
  BASIC = "Basic",
  ITE = "ITE",
  SNE = "SNE",
}
