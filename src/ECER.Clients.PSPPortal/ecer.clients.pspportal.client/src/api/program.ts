import { getClient } from "@/api/client";
import type { Paths, Program, ProgramStatus } from "@/types/openapi";
import ApiResultHandler from "@/utils/apiResultHandler";
const apiResultHandler = new ApiResultHandler();

const getPrograms = async (statuses?: ProgramStatus[]): Promise<Program[] | null> => {
  const client = await getClient();
  const queryParameters: Paths.ProgramGet.QueryParameters = {
    byStatus: statuses,
  };

  const response = await apiResultHandler.execute({
    request: client.program_get(queryParameters),
    key: "program_get",
    suppressErrorToast: true,
  });
  return response?.data ?? null;
};

export { getPrograms };
