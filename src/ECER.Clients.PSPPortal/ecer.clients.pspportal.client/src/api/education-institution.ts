import { getClient } from "@/api/client";
import type {
  CreateCampusRequest,
  EducationInstitution,
  ProblemDetails,
  UpdateCampusRequest,
} from "@/types/openapi";
import ApiResultHandler from "@/utils/apiResultHandler";
const apiResultHandler = new ApiResultHandler();

const getEducationInstitution =
  async (): Promise<EducationInstitution | null> => {
    const client = await getClient();
    const response = await apiResultHandler.execute({
      request: client.education_institution_get(),
      key: "education_institution_get",
      suppressErrorToast: true,
    });
    return response?.data ?? null;
  };

const updateEducationInstitution = async (
  educationInstitution: EducationInstitution,
  suppressErrorToast: boolean = false,
): Promise<boolean> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({
    request: client.education_institution_put({}, educationInstitution),
    key: "education_institution_put",
    suppressErrorToast: suppressErrorToast,
  });
  return response != null;
};

const createCampus = async (
  campus: CreateCampusRequest,
): Promise<string | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({
    request: client.campus_post({}, campus),
    key: "campus_post",
  });
  return response?.data ?? null;
};

const updateCampus = async (
  campusId: string,
  campus: UpdateCampusRequest,
): Promise<ProblemDetails | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({
    request: client.campus_put({ campusId }, campus),
    key: "campus_put",
    suppressErrorToast: true,
  });
  return response;
};

export {
  getEducationInstitution,
  updateEducationInstitution,
  createCampus,
  updateCampus,
};
