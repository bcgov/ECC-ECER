import { getClient } from "@/api/client";
import type { Components } from "@/types/openapi";

const getConfiguration = async (): Promise<Components.Schemas.ApplicationConfiguration | null | undefined> => {
  const client = await getClient(false);
  return (await client.configuration_get()).data;
};

const getProvinceList = async (): Promise<Components.Schemas.Province[] | null | undefined> => {
  const client = await getClient(false);
  return (await client.province_get()).data;
};

const getCountryList = async (): Promise<Components.Schemas.Country[] | null | undefined> => {
  const client = await getClient(false);
  return (await client.country_get()).data;
};

const getPostSecondaryInstitutionList = async (provinceId?: string): Promise<Components.Schemas.PostSecondaryInstitution[] | null | undefined> => {
  const client = await getClient(false);
  return (await client.psi_get()).data;
};

const getSystemMessages = async (): Promise<Components.Schemas.SystemMessage[] | null | undefined> => {
  const client = await getClient(false);
  return (await client.systemMessage_get()).data;
};

const getRecaptchaSiteKey = async (): Promise<string | null | undefined> => {
  const client = await getClient(false);
  return (await client.recaptcha_site_key_get()).data;
};

const getIdentificationTypes = async (): Promise<Components.Schemas.IdentificationType[] | null | undefined> => {
  const client = await getClient(false);
  return (await client.identificationTypes_get()).data;
};

export { getConfiguration, getPostSecondaryInstitutionList, getProvinceList, getCountryList, getRecaptchaSiteKey, getSystemMessages, getIdentificationTypes };
