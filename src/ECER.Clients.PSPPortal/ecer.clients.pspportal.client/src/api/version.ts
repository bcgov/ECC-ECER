// import { getClient } from "@/api/client";
// import type { Components } from "@/types/openapi";
// import ApiResultHandler from "@/utils/apiResultHandler";
// const apiResultHandler = new ApiResultHandler();

// const getVersionInfo = async (): Promise<Components.Schemas.VersionMetadata | null> => {
//   const client = await getClient();
//   const response = await apiResultHandler.execute({ request: client.version_get(), key: "version_get", suppressErrorToast: true });
//   return response?.data ?? null;
// };

// export { getVersionInfo };
