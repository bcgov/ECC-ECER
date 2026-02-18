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

export enum IntervalTime {
  INTERVAL_10_SECONDS = 10000,
}

// Earliest year for program profiles to be displayed
export const EARLIEST_PROFILE_YEAR = 2023;

// Minimum total course hours required for ITE and SNE program types
export const MIN_HOURS_ITE_SNE = 450;
