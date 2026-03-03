import { getClient } from "@/api/client";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";
import type { Components, Paths } from "@/types/openapi";

const apiResultHandler = new ApiResultHandler();

const getCourses = async (
  id: string,
  type: Components.Schemas.FunctionType,
  programTypes: Components.Schemas.ProgramTypes[] = [],
): Promise<Components.Schemas.Course[] | undefined> => {
  const client = await getClient();

  const queryParams: Paths.CoursesGet.QueryParameters = {
    id,
    type,
    programTypes: programTypes.length > 0 ? programTypes : undefined,
  };

  return (
    await apiResultHandler.execute<Components.Schemas.Course[]>({
      request: client.courses_get(queryParams, null),
      key: "courses_get",
    })
  ).data;
};

const updateCourse = async (
  programId: string,
  course: Components.Schemas.Course,
  type: Components.Schemas.FunctionType,
): Promise<ApiResponse<string | null | undefined>> => {
  const client = await getClient();
  const pathParameters: Paths.CoursePut.PathParameters = {
    courseId: course.courseId,
  };
  const body: Paths.CoursePut.RequestBody = {
    course: course,
    type: type,
    id: programId,
  };

  return apiResultHandler.execute<string | null | undefined>({
    request: client.course_put(pathParameters, body),
    key: "course_put",
  });
};

export { getCourses, updateCourse };
