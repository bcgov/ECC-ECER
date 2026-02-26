import { getClient } from "@/api/client";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";
import type { Components, Paths } from "@/types/openapi";
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

const withdrawProgramApplication = async (
  application: Components.Schemas.ProgramApplication,
): Promise<ApiResponse<string | null | undefined>> => {
  const client = await getClient();
  const pathParameters: Paths.ProgramApplicationPut.PathParameters = {
    id: application.id || "",
  };
  const body: Paths.ProgramApplicationPut.RequestBody = {
    ...application,
    status: "Withdrawn",
  };

  return apiResultHandler.execute<string | null | undefined>({
    request: client.program_application_put(pathParameters, body),
    key: "program_application_put",
  });
};

const getProgramApplicationById = async (
  id: string,
): Promise<
  ApiResponse<Components.Schemas.ProgramApplication | null | undefined>
> => {
  const client = await getClient();
  const result =
    await apiResultHandler.execute<Components.Schemas.GetProgramApplicationResponse | null>(
      {
        request: client.program_application_get(
          { id, byStatus: [] },
          null,
          {} as AxiosRequestConfig,
        ),
        key: "program_application_get",
      },
    );
  if (result.error) return { data: null, error: result.error };
  const application = result.data?.applications?.[0] ?? null;
  return { data: application, error: null };
};

const createProgramApplication = async (
  request: Components.Schemas.CreateProgramApplicationRequest,
): Promise<
  ApiResponse<
    Components.Schemas.CreateProgramApplicationResponse | null | undefined
  >
> => {
  const client = await getClient();

  return apiResultHandler.execute<Components.Schemas.CreateProgramApplicationResponse | null>(
    {
      request: client.program_application_post(null, request),
      key: "program_application_post",
    },
  );
};

const getComponentGroupMetadata = async (
  id: string,
): Promise<ApiResponse<Components.Schemas.ComponentGroupMetadata[]>> => {
  const client = await getClient();
  const pathParameters: Paths.ProgramApplicationComponentsGet.PathParameters = {
    id: id,
  };

  return apiResultHandler.execute<Components.Schemas.ComponentGroupMetadata[]>({
    request: client.program_application_components_get(pathParameters),
    key: "program_application_components_get",
  });
};

const getComponentGroupComponents = async (
  programApplicationId: string,
  componentGroupId: string,
): Promise<
  ApiResponse<
    Components.Schemas.ComponentGroupWithComponents | null | undefined
  >
> => {
  const client = await getClient();
  const pathParameters: Paths.ProgramApplicationComponentGroupComponentsGet.PathParameters =
    {
      id: programApplicationId,
      componentGroupId,
    };

  return apiResultHandler.execute<
    Components.Schemas.ComponentGroupWithComponents | null | undefined
  >({
    request:
      client.program_application_component_group_components_get(pathParameters),
    key: "program_application_component_group_components_get",
  });
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
    case "NewCampusatRecognizedPrivateInstitution":
      return "New campus";
    case "AddOnlineorHybridDeliveryMethod":
      return "Add online or hybrid delivery method";
    case "NewBasicECEPostBasicProgram":
      return "New Basic ECE / Post Basic Program";
    case "SatelliteProgram":
      return "Satellite program";
    case "CurriculumRevisionsatRecognizedInstitution":
      return "Curriculum revisions at recognized institution";
    case "WorkIntegratedLearningProgram":
      return "Work integrated learning program";
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
    case "Basic":
      return "ECE (Basic)";
    case "ITE":
      return "ITE";
    case "SNE":
      return "SNE";
    default:
      return "-";
  }
};

export {
  createProgramApplication,
  getProgramApplicationById,
  getProgramApplications,
  getComponentGroupComponents,
  getComponentGroupMetadata,
  mapProgramStatus,
  mapApplicationType,
  mapDeliveryType,
  mapProgramType,
  withdrawProgramApplication,
};
