import { getClient } from "@/api/client";
import type {
  PspUserListItem,
  PspUserProfile,
  NewPspUserResponse,
  Paths,
} from "@/types/openapi";
import ApiResultHandler, { type ApiResponse } from "@/utils/apiResultHandler";
const apiResultHandler = new ApiResultHandler();

const getUsers = async (): Promise<PspUserListItem[] | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({
    request: client.psp_user_manage_get(),
    key: "psp_user_manage_get",
    suppressErrorToast: true,
  });
  return response?.data ?? null;
};

const deactivateUser = async (programRepId: string): Promise<void> => {
  const client = await getClient();
  await apiResultHandler.execute({
    request: client.psp_user_manage_deactivate_post({ programRepId }),
    key: "psp_user_manage_deactivate_post",
  });
};

const reactivateUser = async (programRepId: string): Promise<void> => {
  const client = await getClient();
  await apiResultHandler.execute({
    request: client.psp_user_manage_reactivate_post({ programRepId }),
    key: "psp_user_manage_reactivate_post",
  });
};

const resendInviteUser = async (
  programRepId: string,
): Promise<
  ApiResponse<Paths.PspUserManageResendInvitationPut.Responses.$200>
> => {
  const client = await getClient();
  return apiResultHandler.execute({
    request: client.psp_user_manage_resend_invitation_put({
      programRepId,
    }),
    suppressErrorToast: true,
    key: "psp_user_manage_resend_invitation_put",
  });
};

const setPrimaryUser = async (programRepId: string): Promise<void> => {
  const client = await getClient();
  await apiResultHandler.execute({
    request: client.psp_user_manage_set_primary_post({ programRepId }),
    key: "psp_user_manage_set_primary_post",
  });
};

const addUser = async (
  userProfile: PspUserProfile,
): Promise<NewPspUserResponse | null> => {
  const client = await getClient();
  const response = await apiResultHandler.execute({
    request: client.psp_user_add(null, userProfile),
    key: "psp_user_add",
  });
  return response?.data ?? null;
};

export {
  getUsers,
  deactivateUser,
  setPrimaryUser,
  addUser,
  reactivateUser,
  resendInviteUser,
};
