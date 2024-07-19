import type { AxiosRequestConfig } from "axios";

import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const getCertifications = async (): Promise<ApiResponse<Components.Schemas.Certification[] | null>> => {
  const client = await getClient();
  return apiResultHandler.execute<Components.Schemas.Certification[]>({ request: client.certification_get({ id: "" }) });
};

const getCertificateFileById = async (id: string): Promise<ApiResponse<any>> => {
  const client = await getClient();
  const config: AxiosRequestConfig = { responseType: "blob" };
  return apiResultHandler.execute<any>({ request: client.files_certificate_get({ certificateId: id }, null, config) });
};

export { getCertificateFileById, getCertifications };
