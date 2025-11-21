import { getClient } from "@/api/client";
import type { EducationInstitution } from "@/types/openapi";
import ApiResultHandler from "@/utils/apiResultHandler";
const apiResultHandler = new ApiResultHandler();

const getEducationInstitution = async (): Promise<EducationInstitution | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({ request: client.education_institution_get(), key: "education_institution_get", suppressErrorToast: true });
  return response?.data ?? null;
};

export { getEducationInstitution };
