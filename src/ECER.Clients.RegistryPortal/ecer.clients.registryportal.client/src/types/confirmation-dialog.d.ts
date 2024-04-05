import { VBtn } from "vuetify/components";
type TVariant = VBtn["$props"]["variant"];
export interface ConfirmationDialogProps {
  cancelButtonText?: string;
  acceptButtonText?: string;
  title?: string;
  customButtonVariant?: TVariant;
}
