import { getClient } from "@/api/client";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";
import type { Components } from "@/types/openapi";
import type { AxiosRequestConfig } from "axios";

const apiResultHandler = new ApiResultHandler();

const getProgramApplications = async (
  params: {
    page: number;
    pageSize: number;
  },
  id: string = "",
  statuses: Components.Schemas.ApplicationStatus[] = [],
): Promise<
  ApiResponse<
    Components.Schemas.GetProgramApplicationResponse | null | undefined
  >
> => {
  const client = await getClient();

  const config: AxiosRequestConfig = {
    params: params,
  };

  return apiResultHandler.execute<Components.Schemas.GetProgramApplicationResponse | null>(
    {
      request: client.program_application_get(
        {
          id: id,
          byStatus: statuses,
        },
        null,
        config,
      ),
      key: "program_application_get",
    },
  );
};

const mapProgramStatus = (status: string = ""): string => {
  switch (status) {
    case "Draft":
      return "Draft";
    case "InterimRecognition":
      return "Interim recognition";
    case "OnGoingRecognition":
      return "On-going recognition";
    case "PendingReview":
      return "Pending review";
    case "RefusetoApprove":
      return "Refuse to approve";
    case "ReviewAnalysis":
      return "Under ECE Registry review";
    case "RFAI":
      return "Additional information requested";
    case "Submitted":
      return "Submitted";
    case "Withdrawn":
      return "Withdrawn";
    default:
      return "-";
  }
};

const mapApplicationType = (type: string = ""): string => {
  switch (type) {
    case "PrivateNewCampusLocation":
      return "New campus";
    case "NewDeliveryMethod":
      return "New delivery method";
    case "NewBasicPostBasicProgramHybridOnline":
      return "Online or Hybrid Program";
    case "NewBasicPostBasicProgramInperson":
      return "New Basic / Post-basic Program";
    case "SatelliteProgram":
      return "Satellite program";
    default:
      return "-";
  }
};

const mapDeliveryType = (type: string = ""): string => {
  switch (type) {
    case "Hybrid":
      return "Hybrid";
    case "Inperson":
      return "In-person";
    case "Online":
      return "Online";
    case "Satellite":
      return "Satellite";
    case "WorkIntegratedLearning":
      return "Work Integrated Learning";
    default:
      return "-";
  }
};

const mapProgramType = (type: string = ""): string => {
  switch (type) {
    case "ECEBasic":
      return "ECE (Basic)";
    case "ITE":
      return "ITE";
    case "ITESNE":
      return "ITE & SNE";
    case "SNE":
      return "SNE";
    default:
      return "-";
  }
};

export {
  getProgramApplications,
  mapProgramStatus,
  mapApplicationType,
  mapDeliveryType,
  mapProgramType,
};
