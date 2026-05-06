import type { AxiosProgressEvent } from "axios";

import { getClient } from "@/api/client";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";
import type { Components } from "@/types/openapi";

const apiResultHandler = new ApiResultHandler();

export type ApplicationFileInfo = Components.Schemas.ApplicationFileInfo;

const uploadProgramApplicationFile = async (
  programApplicationId: string,
  componentGroupId: string,
  componentId: string,
  fileId: string,
  file: File,
  onUploadProgress?: (progressEvent: AxiosProgressEvent) => void,
): Promise<ApiResponse<ApplicationFileInfo>> => {
  const client = await getClient();
  const formData = new FormData();
  formData.append("file", file);

  return apiResultHandler.execute<ApplicationFileInfo>({
    request: client.post<ApplicationFileInfo>(
      `/api/programApplications/${programApplicationId}/componentGroups/${componentGroupId}/components/${componentId}/files/${fileId}`,
      formData,
      {
        headers: { "Content-Type": "multipart/form-data" },
        onUploadProgress,
      },
    ),
  });
};

const getProgramApplicationFiles = async (
  programApplicationId: string,
): Promise<ApiResponse<ApplicationFileInfo[]>> => {
  const client = await getClient();
  return apiResultHandler.execute<ApplicationFileInfo[]>({
    request: client.get<ApplicationFileInfo[]>(
      `/api/programApplications/${programApplicationId}/files`,
    ),
  });
};

const getApplicationDocumentUrls = async (
  programApplicationId: string,
): Promise<ApiResponse<ApplicationFileInfo[]>> => {
  const client = await getClient();
  return apiResultHandler.execute<ApplicationFileInfo[]>({
    request: client.get<ApplicationFileInfo[]>(
      `/api/programApplications/${programApplicationId}/documentUrls`,
    ),
  });
};

const shareExistingDocument = async (
  programApplicationId: string,
  componentGroupId: string,
  componentId: string,
  documentUrlId: string,
): Promise<ApiResponse<ApplicationFileInfo>> => {
  const client = await getClient();
  return apiResultHandler.execute<ApplicationFileInfo>({
    request: client.post<ApplicationFileInfo>(
      `/api/programApplications/${programApplicationId}/componentGroups/${componentGroupId}/components/${componentId}/files/${documentUrlId}/share`,
    ),
  });
};

const deleteProgramApplicationFile = async (
  programApplicationId: string,
  shareDocumentUrlId: string,
): Promise<ApiResponse<void>> => {
  const client = await getClient();
  return apiResultHandler.execute<void>({
    request: client.delete(
      `/api/programApplications/${programApplicationId}/files/${shareDocumentUrlId}`,
    ),
  });
};

export {
  deleteProgramApplicationFile,
  getApplicationDocumentUrls,
  getProgramApplicationFiles,
  shareExistingDocument,
  uploadProgramApplicationFile,
};
