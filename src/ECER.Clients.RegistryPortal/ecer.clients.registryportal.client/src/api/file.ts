import { type AxiosProgressEvent, type AxiosRequestConfig } from "axios";

import { getClient } from "@/api/client";
import type { Components, Paths } from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";

const apiResultHandler = new ApiResultHandler();

const uploadFile = async (
  fileId: string,
  file: File,
  fileClassification: string,
  fileTag?: string,
  onUploadProgress?: (progressEvent: AxiosProgressEvent) => void,
): Promise<ApiResponse<Components.Schemas.FileResponse>> => {
  const client = await getClient();

  const parameters: Paths.UploadFile.PathParameters = {
    fileId,
  };

  const formData = new FormData();
  formData.append("file", file);

  const config: AxiosRequestConfig = {
    headers: {
      "file-classification": fileClassification,
      ...(fileTag && { "file-tag": fileTag }),
      "Content-Type": "multipart/form-data",
    },
    onUploadProgress,
  };
  return apiResultHandler.execute<Components.Schemas.FileResponse>({
    request: client.upload_file(
      parameters,
      formData as unknown as Paths.UploadFile.RequestBody,
      config,
    ),
  });
};

const deleteFile = async (
  fileId: string,
): Promise<ApiResponse<Paths.DeleteFile.Responses.$200>> => {
  const client = await getClient();

  const parameters: Paths.DeleteFile.PathParameters = {
    fileId,
  };

  const config: AxiosRequestConfig = {
    headers: {
      "Content-Type": "application/json",
    },
  };

  return apiResultHandler.execute<Paths.DeleteFile.Responses.$200>({
    request: client.delete_file(parameters, null, config),
  });
};

export { deleteFile, uploadFile };
