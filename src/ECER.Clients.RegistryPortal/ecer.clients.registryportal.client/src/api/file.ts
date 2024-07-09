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
): Promise<ApiResponse<Components.Schemas.UploadFileResponse>> => {
  const client = await getClient();

  const parameters: Paths.UploadFile.PathParameters & Paths.UploadFile.HeaderParameters = {
    fileId,
    "file-classification": fileClassification,
    "file-tag": fileTag,
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

  return apiResultHandler.execute<Components.Schemas.UploadFileResponse>({ request: client.upload_file(parameters, formData as unknown as string, config) });
};

export { uploadFile };
