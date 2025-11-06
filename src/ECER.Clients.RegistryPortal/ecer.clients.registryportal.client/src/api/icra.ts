import { getClient } from "@/api/client";
import type { Components, Paths } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getIcraEligibilities = async (): Promise<ApiResponse<Components.Schemas.ICRAEligibility[] | null | undefined>> => {
  const client = await getClient();

  return apiResultHandler.execute<Components.Schemas.ICRAEligibility[] | null | undefined>({
    request: client.icra_get({ id: "" }),
    key: "icra_get",
  });
};

const createOrUpdateDraftIcraEligibility = async (
  icraEligibility: Components.Schemas.ICRAEligibility,
): Promise<ApiResponse<Components.Schemas.DraftICRAEligibilityResponse | null | undefined>> => {
  const client = await getClient();
  const body: Paths.IcraPut.RequestBody = {
    eligibility: icraEligibility,
  };
  const pathParameters: Paths.IcraPut.PathParameters = {
    id: icraEligibility.id || "",
  };

  return apiResultHandler.execute<Components.Schemas.DraftICRAEligibilityResponse | null | undefined>({
    request: client.icra_put(pathParameters, body),
    key: "icra_put",
  });
};

const getIcraEligibilityStatus = async (eligibilityId: string): Promise<ApiResponse<Components.Schemas.ICRAEligibilityStatus | null | undefined>> => {
  const client = await getClient();
  const pathParameters = {
    id: eligibilityId,
  };

  return await apiResultHandler.execute<Components.Schemas.ICRAEligibilityStatus | null | undefined>({
    request: client.icra_status_get(pathParameters),
  });
};

const submitIcraEligibilityApplication = async (
  eligibilityId: string,
): Promise<ApiResponse<Components.Schemas.SubmitICRAEligibilityResponse | null | undefined>> => {
  const client = await getClient();
  const body: Components.Schemas.ICRAEligibilitySubmissionRequest = {
    id: eligibilityId,
  };

  return apiResultHandler.execute<Components.Schemas.SubmitICRAEligibilityResponse | null | undefined>({
    request: client.icra_post(null, body),
    key: "icra_post",
  });
};

export { createOrUpdateDraftIcraEligibility, getIcraEligibilities, getIcraEligibilityStatus, submitIcraEligibilityApplication };
