import type { Component } from "vue";

import type { Components } from "@/types/openapi";

export type ExpandSelectOption = {
  id: string;
  title: string;
  contentComponent: Component;
  hasSubSelection: boolean;
};

export type ExpandSelectCertificateTypeOption = {
  id: Components.Schemas.CertificationType;
  title: string;
  contentComponent: Component;
  hasSubSelection: boolean;
};
