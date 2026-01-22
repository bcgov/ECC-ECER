import { getClient } from "@/api/client";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";
import type { Components, Paths } from "@/types/openapi";

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
): Promise<ApiResponse<Components.Schemas.Program[] | null | undefined>> => {
  const client = await getClient();
  return apiResultHandler.execute<
    Components.Schemas.Program[] | null | undefined
  >({
    request: client.program_get({
      id: id,
      byStatus: statuses,
    }),
    key: "program_get",
  });
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

const updateCourse = async (
  programId: string,
  courses: Components.Schemas.Course[],
): Promise<ApiResponse<string | null | undefined>> => {
  const client = await getClient();
  const pathParameters: Paths.CoursePut.PathParameters = {
    id: programId,
  };
  const body: Paths.CoursePut.RequestBody = {
    courses: courses,
  };

  return apiResultHandler.execute<string | null | undefined>({
    request: client.course_put(pathParameters, body),
    key: "course_put",
  });
};

const submitDraftProgramApplication = async (
  programId: string,
): Promise<ApiResponse<string | null | undefined>> => {
  const client = await getClient();
  const body: Components.Schemas.SubmitProgramRequest = {
    programId: programId,
  };

  return apiResultHandler.execute<string | null | undefined>({
    request: client.program_post(null, body),
    key: "program_post",
  });
};

export {
  createOrUpdateDraftApplication,
  getPrograms,
  submitDraftProgramApplication,
  updateCourse,
};
