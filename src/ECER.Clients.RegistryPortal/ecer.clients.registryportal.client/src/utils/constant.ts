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
