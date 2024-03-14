import type EceAddress from "@/components/inputs/EceAddress.vue";
import type EceAddresses from "@/components/inputs/EceAddresses.vue";
import type EceCertificateType from "@/components/inputs/EceCertificateType.vue";
import type EceCertificationTypePreview from "@/components/inputs/EceCertificationTypePreview.vue";
import type EceCharacterReference from "@/components/inputs/EceCharacterReference.vue";
import type EceCharacterReferencePreview from "@/components/inputs/EceCharacterReferencePreview.vue";
import type EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import type EceContactInformationPreview from "@/components/inputs/EceContactInformationPreview.vue";
import type EceEducation from "@/components/inputs/EceEducation.vue";
import type EceEducationPreview from "@/components/inputs/EceEducationPreview.vue";
import type EceRadio from "@/components/inputs/EceRadio.vue";
import type EceTextField from "@/components/inputs/EceTextField.vue";
import type EceWorkExperienceReferencePreview from "@/components/inputs/EceWorkExperienceReferencePreview.vue";

interface EceTextFieldProps {
  rules: readonly ValidationRule$1[];
  label: string;
  type?: string;
  isNumeric?: boolean;
  disabled?: boolean;
  readonly?: boolean;
  prependInnerIcon?: string;
  maxLength?: number;
}

interface EceEducationProps {}

interface EceAddressProps {
  addressLabel: string;
}

interface EceAddressesProps {
  residential: EceAddressProps;
  mailing: EceAddressProps;
}

interface EceCheckboxProps {
  rules?: readonly ValidationRule$1[];
  label: string;
  disabled?: boolean;
  checkableOnce?: boolean;
}

interface EceCertificateTypeProps {
  options: ExpandSelectOption[];
}

interface EcePreviewProps {}

interface EceCharacterReferenceProps {}

interface Input {
  id: string;
  component:
    | EceCheckbox
    | EceRadio
    | EceTextField
    | EceAddress
    | EceAddresses
    | EceCertificateType
    | EceEducation
    | EceCertificationTypePreview
    | EceContactInformationPreview
    | EceEducationPreview
    | EceWorkExperienceReferencePreview
    | EceCharacterReferencePreview
    | EceCharacterReference;
  props:
    | EceTextFieldProps
    | EceAddressProps
    | EceAddressesProps
    | EceEducationProps
    | EceCheckboxProps
    | EceCertificateTypeProps
    | EcePreviewProps
    | EceCharacterReferenceProps;
  cols: {
    md: number;
    lg: number;
    xl: number;
  };
}
