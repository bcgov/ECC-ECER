import { getClient } from "@/api/client";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";
import type { Components, Paths } from "@/types/openapi";
import type { AxiosRequestConfig } from "axios";

const apiResultHandler = new ApiResultHandler();

const getPrograms = async (
  id: string = "",
  statuses: Components.Schemas.ProgramStatus[] = [
    "Draft",
    "Denied",
    "Approved",
    "UnderReview",
    "ChangeRequestInProgress",
  ],
  {
    page = 0,
    pageSize = 0
  } = {},
): Promise<ApiResponse<Components.Schemas.GetProgramsResponse | null | undefined>> => {
  const client = await getClient();

  const config: AxiosRequestConfig = {
    params: {page, pageSize},
  };

  return apiResultHandler.execute<Components.Schemas.GetProgramsResponse | null>(
    { request: client.program_get({
      id: id,
      byStatus: statuses,
    }, null, config), key: "program_get" },
  );
};

const createOrUpdateDraftApplication = async (
  program: Components.Schemas.Program,
): Promise<
  ApiResponse<Components.Schemas.DraftProgramResponse | null | undefined>
> => {
  const client = await getClient();
  const body: Paths.DraftprogramPut.RequestBody = {
    program: program,
  };
  const pathParameters: Paths.DraftprogramPut.PathParameters = {
    id: program.id || "",
  };

  return apiResultHandler.execute<
    Components.Schemas.DraftProgramResponse | null | undefined
  >({
    request: client.draftprogram_put(pathParameters, body),
    key: "draftprogram_put",
  });
};

export { createOrUpdateDraftApplication, getPrograms };
